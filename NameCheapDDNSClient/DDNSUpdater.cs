using System.Net;
using System.Text.Json;

class DDNSUpdater
{
    private static readonly HttpClient HttpClient = new HttpClient();
    public static readonly List<DDNSConfig> ConfigList = new List<DDNSConfig>();

    public static void Run(string configFolderPath)
    {
        try
        {
            DDNSLogger.Log("Running...");

            string[] configFiles = Directory.GetFiles(configFolderPath, "*.json");

            if (configFiles.Length == 0)
            {
                DDNSLogger.Log("No config files found in the specified folder: " + configFolderPath);
                return;
            }

            foreach (string configFile in configFiles)
            {
                try
                {
                    DDNSConfig config = DDNSConfig.Read(configFile);
                    DDNSLogger.Log($"Adding config for {config.Host}.{config.Domain}");
                    ConfigList.Add(config);
                }
                catch (Exception ex)
                {
                    DDNSLogger.LogCritical($"Error reading configuration from {configFile}: {ex.Message}");
                }
            }

            if (ConfigList.Count > 0)
            {
                ProcessConfigs();
            }
            else
            {
                DDNSLogger.Log("No config files to process");
            }
        }
        catch (Exception ex)
        {
            DDNSLogger.LogCritical($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static void ProcessConfigs()
    {
        while (true)
        {
            DDNSLogger.Log("Checking configuration files...");
            string[] providers = {
            "https://api.ipify.org/?format=json",
            "https://api.my-ip.io/v2/ip.json",
            "https://api.myip.com/",
        };

            string? publicIp = GetPublicIp(providers);

            foreach (var config in ConfigList)
            {
                if (DateTime.Now >= config.NextRun)
                {
                    try
                    {
                        DDNSLogger.Log($"Running: {config}");
                        MakeHttpRequest(config, publicIp);
                    }
                    catch (Exception ex)
                    {
                        DDNSLogger.LogWarning($"Error processing {config}: {ex}");
                    }
                    finally
                    {
                        DateTime next = config.NextRun.AddMinutes(config.TTL);
                        config.NextRun = new DateTime(next.Year,next.Month,next.Day,next.Hour,next.Minute,0);
                        DDNSLogger.Log($"Next run for {config.Host}.{config.Domain} will be at {config.NextRun}");
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
            var remainingTime = nextRunDate - DateTime.Now.AddSeconds(1);

            DDNSLogger.Log($"Debug: {Math.Floor(remainingTime.TotalMinutes)} minutes and {Math.Floor(remainingTime.TotalSeconds % 60)} seconds left for next update. (Updated hosts will be [{nextUpdateHostsString}])");
            Thread.Sleep((int)remainingTime.TotalMilliseconds<1000?1000:(int)remainingTime.TotalMilliseconds);
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
            DDNSLogger.LogResponse(responseBody, config, publicIp);
        }
        else
        {
            DDNSLogger.Log($"Config has the same IP as the actual IP, no need to update.");
        }
    }

    static string? GetPublicIp(string[] providerUrls)
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
                        DDNSLogger.Log($"Successfully received IP ({validIp}) from provider: {apiUrl}");
                        break;
                    }
                    else
                    {
                        DDNSLogger.LogWarning($"Invalid IP format received from {apiUrl}: {result?.ip}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                DDNSLogger.LogWarning($"Timeout exceeded while getting public IP from {apiUrl}");
            }
            catch (Exception ex)
            {
                DDNSLogger.LogWarning($"Error getting public IP from {apiUrl}: {ex.Message}");
            }
        }

        return validIp;
    }

    private static bool IsValidIpAddress(string? ipAddress)
    {
        return !string.IsNullOrEmpty(ipAddress) && IPAddress.TryParse(ipAddress, out _);
    }


}
