using System.Net;
using System.Text.Json;
using System.Xml;

namespace eggDDNS
{
    class Updater
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        public static readonly List<HostSettings> HostConfigList =  Config.GetAllConfigFilesList<HostSettings>();
        public static readonly List<IpProviderSettings> IpProviderConfigList = Config.GetAllConfigFilesList<IpProviderSettings>();
        public static void Run()
        {
            try
            {
                Logger.Info("Running...");
                if (IpProviderConfigList.Count <= 0)
                {
                    Logger.Info("No Ip Provider config files to process");
                    return;
                }
              
                if (HostConfigList.Count <= 0)
                {
                    Logger.Info("No Host config files to process");
                    return;
                }
                ProcessConfigs();
            }
            catch (Exception ex)
            {
                Logger.Critical("Error: {ex}", ex.Message);
                Environment.Exit(1);
            }
        }

        private static void ProcessConfigs()
        {
            while (true)
            {
                Logger.Info("Processing...");

                string? publicIp = GetPublicIp();

                while (HostConfigList.Min(c => c.NextRun) <= DateTime.Now.AddSeconds(10))
                {
                    foreach (var config in HostConfigList)
                    {
                        if (config.NextRun <= DateTime.Now)
                        {
                            try
                            {
                                Logger.Info("Running: {config}", config.ToArray());
                                MakeHttpRequest(config, publicIp);
                            }
                            catch (Exception ex)
                            {
                                Logger.Warning("Error processing {config}: {ex}", config.ToArray(), ex);
                            }
                            finally
                            {
                                DateTime next = config.NextRun.AddMinutes(config.TTL);
                                config.NextRun = new DateTime(next.Year, next.Month, next.Day, next.Hour, next.Minute, 0);
                                Logger.Info("Next run for {Host}.{Domain} will be at {NextRun}", config.Host, config.Domain, config.NextRun);
                            }
                        }
                    }
                }


                var nextRunDate = HostConfigList.Min(c => c.NextRun);
                var nextUpdateHosts = HostConfigList.FindAll(x =>
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
                Logger.Info("Debug: {remainingMinutes} minutes and {remaininSeconds} seconds left for next update. (Updated hosts will be {nextUpdateHostsString})", remainingMinutes, remainingSeconds, nextUpdateHosts.Select(c => $"{c.Host}.{c.Domain}"));

                Thread.Sleep((int)remainingTime.TotalMilliseconds < 1000 ? 1000 : (int)remainingTime.TotalMilliseconds);
            }
        }
        private static void ParseResponse(string response, HostSettings config, string ip)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);

                var errCount = int.Parse(xmlDoc.SelectSingleNode("//ErrCount")?.InnerText ?? "0");

                if (errCount > 0)
                {
                    var errorDescription = xmlDoc.SelectSingleNode("//errors/Err1")?.InnerText;
                    Logger.Warning($"{errorDescription}");
                }
                else
                {
                    config.IP = ip;
                    Logger.Info("Successfully set DDNS A+ Record on {Host}.{Domain} IP: {ip} on your namecheap.com account", config.Host, config.Domain, ip);
                }
            }
            catch (Exception ex)
            {
                Logger.Critical("Error parsing response: {ex}", ex.Message);
            }
        }
        private static void MakeHttpRequest(HostSettings config, string? publicIp)
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
                Logger.Info($"Config has the same IP as the actual IP, no need to update.");
            }
        }
        private static string? GetPublicIp()
        {
            string? validIp = null;

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30));

            foreach (var cf in IpProviderConfigList)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string responseJson = client.GetStringAsync(cf.Url, cancellationTokenSource.Token).Result;
                        var result = JsonSerializer.Deserialize<PublicIp>(responseJson);

                        if (IsValidIpAddress(result?.ip))
                        {
                            validIp = result?.ip;
                            Logger.Info("Successfully received IP ({validIp}) from provider: {apiUrl}", validIp, cf.Url);
                            break;
                        }
                        else
                        {
                            Logger.Warning("Invalid IP format received from {apiUrl}: {result?.ip}", cf.Url, result?.ip);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Logger.Warning("Timeout exceeded while getting public IP from {apiUrl}", cf.Url);
                }
                catch (Exception ex)
                {
                    Logger.Warning("Error getting public IP from {apiUrl}: {ex}", cf.Url, ex.Message);
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