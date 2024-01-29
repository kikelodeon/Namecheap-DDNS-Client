class DDNSLogger
{
    public static void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - {message}");
    }

    public static void LogWarning(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - WARNING - {message}");
    }

    public static void LogCritical(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} - CRITICAL - {message}");
    }
    public static void LogResponse(string response, DDNSConfig config, string ip)
    {
        try
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(response);

            var errCount = int.Parse(xmlDoc.SelectSingleNode("//ErrCount")?.InnerText ?? "0");

            if (errCount > 0)
            {
                var errorDescription = xmlDoc.SelectSingleNode("//errors/Err1")?.InnerText;
                DDNSLogger.Log($"Error: {errorDescription}");
            }
            else
            {
                config.IP = ip;
                DDNSLogger.Log($"Successfully set DDNS A+ Record on {config.Host}.{config.Domain} IP: {ip} on your namecheap.com account");
            }
        }
        catch (Exception ex)
        {
            DDNSLogger.LogWarning($"Error parsing response: {ex.Message}");
        }
    }
}
