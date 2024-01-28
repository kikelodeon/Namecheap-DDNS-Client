using System.Net;
using System.Text.Json;

class DDNSUpdater
{
    private static readonly HttpClient HttpClient = new HttpClient();
    private static readonly List<DDNSConfig> ConfigList = new List<DDNSConfig>();

    public static void Run(string configFolderPath)
    {
        try
        {
            Log("Start");

            string[] configFiles = Directory.GetFiles(configFolderPath, "*.json");

            if (configFiles.Length == 0)
            {
                Log("No config files found in the specified folder." + configFolderPath);
                return;
            }

            foreach (string configFile in configFiles)
            {
                try
                {
                    DDNSConfig config = ReadConfig(configFile);
                    Log($"Adding config for {config.Host}.{config.Domain}");

                    // Add the config to the list
                    ConfigList.Add(config);
                }
                catch (Exception ex)
                {
                    LogCritical($"Error reading configuration from {configFile}: {ex.Message}");
                    // Continue to the next config file if there's an error with the current one
                }
            }
            // Start the main processing loop
            if (ConfigList.Count > 0)
            {
                ProcessConfigs();
            }
            else
            {
                Log("No config files to process");
            }
        }
        catch (Exception ex)
        {
            LogCritical($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static void ProcessConfigs()
    {
        while (true)
        {
            Log("Checking configurations...");

            foreach (var config in ConfigList)
            {
                if (DateTime.Now >= config.NextRun)
                {
                    try
                    {
                        Log($"Running with config params: {GetConfigLog(config)}");
                        MakeHttpRequest(config);
                    }
                    catch (Exception ex)
                    {
                        LogWarning($"Error processing config for {config.Host}.{config.Domain}: {ex.Message}");
                    }
                    finally
                    {
                        config.NextRun = DateTime.Now.AddMinutes(config.TTL).AddSeconds(-DateTime.Now.Second);
                        Log($"Next run for {config.Host}.{config.Domain} will be at {config.NextRun}");
                    }
                }
            }
            var nextRunDate = ConfigList.Min(c => c.NextRun);
            List<DDNSConfig> nextUpdateHosts = ConfigList.FindAll(x =>
                    x.NextRun.Year == nextRunDate.Year &&
                    x.NextRun.Month == nextRunDate.Month &&
                    x.NextRun.Day == nextRunDate.Day &&
                    x.NextRun.Hour == nextRunDate.Hour &&
                    x.NextRun.Minute == nextRunDate.Minute
                );

            var nextUpdateHostsString = string.Join(", ", nextUpdateHosts.Select(c => $"{c.Host}.{c.Domain}"));

            var remainingTime = nextRunDate - DateTime.Now.AddSeconds(-1);
            var remainingSeconds = remainingTime.TotalSeconds;

            Log($"Debug: {Math.Floor(remainingTime.TotalMinutes)} minutes and {Math.Floor(remainingTime.TotalSeconds % 60)} seconds left for next update. (Updated hosts will be [{nextUpdateHostsString}])");

            // Sleep for a minute before checking again
            Thread.Sleep((int)remainingSeconds * 1000);
        }
    }

    private static DDNSConfig ReadConfig(string filePath)
    {
        string configJson = File.ReadAllText(filePath);

        try
        {
            DDNSConfig? config = JsonSerializer.Deserialize<DDNSConfig>(configJson);

            // Check if the result is null and handle it accordingly
            if (config == null)
            {
                throw new Exception("Failed to deserialize JSON or the result is null.");
            }

            // Validate if required parameters are present
            if (string.IsNullOrEmpty(config.Host) || string.IsNullOrEmpty(config.Domain) ||
                string.IsNullOrEmpty(config.Password) ||
                config.TTL <= 0)
            {
                throw new Exception("Invalid or incomplete configuration file.");
            }

            return config;
        }
        catch (JsonException ex)
        {
            throw new Exception($"Error parsing configuration file: {ex.Message}");
        }
    }

    private static void MakeHttpRequest(DDNSConfig config)
    {
        // Construct the URL for DDNS update
        string[] providers = {
            "https://api.ipify.org/?format=json",
            "https://api.my-ip.io/v2/ip.json",
            "https://api.myip.com/",
        };

        string? publicIp = GetPublicIp(providers);

        if (publicIp != null)
        {

            if (publicIp != config.IP)
            {
                // Construct the URL for DDNS update with the detected public IP
                string url = $"https://dynamicdns.park-your-domain.com/update?host={config.Host}&domain={config.Domain}&password={config.Password}&ip={publicIp}";
                
                // Make the HTTPS request using HttpClient
                HttpResponseMessage response = HttpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                // Log success or error based on the response
                LogResponse(responseBody, config, publicIp);
            }
            else
            {
                Log($"Config has same ip than actual ip, no need to update.");
            }

        }
        else
        {
            LogWarning("Failed to detect public IP from all providers.");
        }
    }

    static string? GetPublicIp(string[] providerUrls)
    {
        string? validIp = null;

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(30)); // Set the timeout to 30 seconds

        foreach (var apiUrl in providerUrls)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string responseJson = client.GetStringAsync(apiUrl, cancellationTokenSource.Token).Result;
                    var result = JsonSerializer.Deserialize<PublicIpResponse>(responseJson);

                    // Check if the IP is valid
                    if (IsValidIpAddress(result?.ip))
                    {
                        validIp = result?.ip;
                        Log($"Successfully received IP ({validIp}) from provider: {apiUrl}");
                        break; // Break out of the loop if a valid IP is obtained
                    }
                    else
                    {
                        LogWarning($"Invalid IP format received from {apiUrl}: {result?.ip}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogWarning($"Timeout exceeded while getting public IP from {apiUrl}");
            }
            catch (Exception ex)
            {
                LogWarning($"Error getting public IP from {apiUrl}: {ex.Message}");
            }
        }

        return validIp;
    }


    private static bool IsValidIpAddress(string? ipAddress)
    {
        // Add your IP address validation logic here
        // For a simple example, you can check if the string is not null or empty
        // and use IPAddress.TryParse to check the format
        return !string.IsNullOrEmpty(ipAddress) && IPAddress.TryParse(ipAddress, out _);
    }

    private static string GetConfigLog(DDNSConfig config)
    {
        return $"Host={config.Host}, Domain={config.Domain}, Password=****, IP={config.IP}, Interval={config.TTL} minutes";
    }

    private static void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - {message}");
    }

    private static void LogResponse(string response, DDNSConfig config, string ip)
    {
        try
        {
            // Parse the XML response
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(response);

            var errCount = int.Parse(xmlDoc.SelectSingleNode("//ErrCount")?.InnerText ?? "0");

            if (errCount > 0)
            {
                // Log error details
                var errorDescription = xmlDoc.SelectSingleNode("//errors/Err1")?.InnerText;
                Log($"Error: {errorDescription}");
            }
            else
            {
                // Additional log for DDNS A+ Record setup
                config.IP = ip;
                Log($"Successfully setted DDNS A+ Record on {config.Host}.{config.Domain} ip: {ip} on your namecheap.com account");
            }
        }
        catch (Exception ex)
        {
            // Log any parsing or unexpected errors
            LogWarning($"Error parsing response: {ex.Message}");
        }
    }
    public static void LogWarning(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - WARNING - {message}");
    }
    public static void LogCritical(string message)
    {
        // Add code here to log critical messages
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - CRITICAL - {message}");
    }
}
