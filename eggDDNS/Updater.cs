using System.Net;
using System.Text.Json;
using System.Xml;
namespace eggDDNS{
class DDNSUpdater
{
    private static readonly HttpClient HttpClient = new HttpClient();
    public static readonly List<DDNSConfig> ConfigList = new List<DDNSConfig>();

    public static void Run(string configFolderPath)
    {
        try
        {
            DDNSLogger.Info("Running...");
            string[] configFiles = Directory.GetFiles(configFolderPath, "*.json");
            if (configFiles.Length == 0)
            {
                DDNSLogger.Info("No config files found in the specified folder: {configFolderPath}",configFolderPath);
                return;
            }

            foreach (string configFile in configFiles)
            {
                try
                {
                    DDNSConfig config = DDNSConfig.Read(configFile);
                    ConfigList.Add(config);
                    DDNSLogger.Info("Successfully added config for {Host}.{Domain}",config.Host,config.Domain);
                }
                catch (Exception ex)
                {
                    DDNSLogger.Critical("Error reading configuration from {configFile}: {ex}",configFile,ex.Message);
                }
            }

            if (ConfigList.Count > 0)
            {
                ProcessConfigs();
            }
            else
            {
                DDNSLogger.Info("No config files to process");
            }
        }
        catch (Exception ex)
        {
            DDNSLogger.Critical("Error: {ex}",ex.Message);
            Environment.Exit(1);
        }
    }

    private static void ProcessConfigs()
    {
        while (true)
        {
            DDNSLogger.Info("Checking configuration files...");
            string[] providers = {
            "https://api.ipify.org/?format=json",
            "https://api.my-ip.io/v2/ip.json",
            "https://api.myip.com/",
            };

            string? publicIp = GetPublicIp(providers);

            while(ConfigList.Min(c => c.NextRun)<=DateTime.Now.AddSeconds(10))
            {
                foreach (var config in ConfigList)
                {
                    if (config.NextRun<=DateTime.Now)
                    {
                        try
                        {
                            DDNSLogger.Info("Running: {config}",config);
                            MakeHttpRequest(config, publicIp);
                        }
                        catch (Exception ex)
                        {
                            DDNSLogger.Warning("Error processing {config}: {ex}",config,ex);
                        }
                        finally
                        {
                            DateTime next = config.NextRun.AddMinutes(config.TTL);
                            config.NextRun = new DateTime(next.Year,next.Month,next.Day,next.Hour,next.Minute,0);
                            DDNSLogger.Info("Next run for {Host}.{Domain} will be at {NextRun}",config.Host,config.Domain,config.NextRun);
                        }
                    }
                }
            }
                

            var nextRunDate = ConfigList.Min(c => c.NextRun);
            var nextUpdateHosts = ConfigList.FindAll(x =>
                x.NextRun.Year == nextRunDate.Year &&
                x.NextRun.Month == nextRunDate.Month &&
                x.NextRun.Day == nextRunDate.Day &&
                x.NextRun.Hour == nextRunDate.Hour &&
                x.NextRun.Minute == nextRunDate.Minute
            );

            var nextUpdateHostsString = string.Join(", ", nextUpdateHosts.Select(c => $"{c.Host}.{c.Domain}"));
            var remainingTime = nextRunDate - DateTime.Now;

            var remainingMinutes = Math.Floor(remainingTime.TotalMinutes);
            var remainingSeconds = Math.Floor(remainingTime.TotalSeconds % 60);
            DDNSLogger.Info("Debug: {remainingMinutes} minutes and {remaininSeconds} seconds left for next update. (Updated hosts will be [{nextUpdateHostsString}])",remainingMinutes, remainingSeconds, nextUpdateHostsString);

            Thread.Sleep((int)remainingTime.TotalMilliseconds<1000?1000:(int)remainingTime.TotalMilliseconds);
        }
    }
    private static void ParseResponse(string response, DDNSConfig config, string ip)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            var errCount = int.Parse(xmlDoc.SelectSingleNode("//ErrCount")?.InnerText ?? "0");

            if (errCount > 0)
            {
                var errorDescription = xmlDoc.SelectSingleNode("//errors/Err1")?.InnerText;
                DDNSLogger.Warning($"{errorDescription}");
            }
            else
            {
                config.IP = ip;
                DDNSLogger.Info("Successfully set DDNS A+ Record on {Host}.{Domain} IP: {ip} on your namecheap.com account",config.Host, config.Domain, ip);
            }
        }
        catch (Exception ex)
        {
           DDNSLogger.Critical("Error parsing response: {ex}",ex.Message);
        }
    }
    private static void MakeHttpRequest(DDNSConfig config, string? publicIp)
    {
        if (publicIp != null && publicIp != config.IP)
        {
            string url = $"https://dynamicdns.park-your-domain.com/update?host={config.Host}&domain={config.Domain}&password={config.Password}&ip={publicIp}";
            HttpResponseMessage response = HttpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            ParseResponse(responseBody, config, publicIp);
        }
        else
        {
            DDNSLogger.Info($"Config has the same IP as the actual IP, no need to update.");
        }
    }
    private static string? GetPublicIp(string[] providerUrls)
    {
        string? validIp = null;

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

        foreach (var apiUrl in providerUrls)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string responseJson = client.GetStringAsync(apiUrl, cancellationTokenSource.Token).Result;
                    var result = JsonSerializer.Deserialize<PublicIpResponse>(responseJson);

                    if (IsValidIpAddress(result?.ip))
                    {
                        validIp = result?.ip;
                        DDNSLogger.Info("Successfully received IP ({validIp}) from provider: {apiUrl}",validIp,apiUrl);
                        break;
                    }
                    else
                    {
                        DDNSLogger.Warning("Invalid IP format received from {apiUrl}: {result?.ip}",apiUrl,result?.ip);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                DDNSLogger.Warning("Timeout exceeded while getting public IP from {apiUrl}",apiUrl);
            }
            catch (Exception ex)
            {
                DDNSLogger.Warning("Error getting public IP from {apiUrl}: {ex}",apiUrl,ex.Message);
            }
        }

        return validIp;
    }
    private static bool IsValidIpAddress(string? ipAddress)
    {
        return !string.IsNullOrEmpty(ipAddress) && IPAddress.TryParse(ipAddress, out _);
    }
}

}