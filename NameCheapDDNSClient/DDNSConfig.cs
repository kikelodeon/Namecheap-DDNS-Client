using System.Text.Json;

public class DDNSConfig
{
    private int _ttl;
    private DateTime _nextRun = DateTime.Now;

    public DDNSConfig()
    {
        Host = string.Empty;
        Domain = string.Empty;
        Password = string.Empty;
        IP = string.Empty;
        TTL = 15;
    }
    

    public static DDNSConfig Read(string filePath)
    {
        string configJson = File.ReadAllText(filePath);

        try
        {
            DDNSConfig? config = JsonSerializer.Deserialize<DDNSConfig>(configJson);

            if (config == null || string.IsNullOrEmpty(config.Host) || string.IsNullOrEmpty(config.Domain) ||
                string.IsNullOrEmpty(config.Password) || config.TTL <= 0)
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
   public override string ToString()
    {
        // Override ToString to provide a formatted string representation of DDNSConfig
        return $"Host={Host}, Domain={Domain}, Password=****, IP={IP}, Interval={TTL} minutes";
    }
    // Properties
    public string Host { get; set; }
    public string Domain { get; set; }
    public string Password { get; set; }
    public string IP { get; set; }

    // TTL property with clamping logic
    public int TTL
    {
        get => _ttl;
        set
        {
            int boundariesValue =  ForceBounds( value);
            if (value != boundariesValue)
            {
                DDNSLogger.Warning("Warning: TTL value {value} is outside the allowed range [5, 60]. Clamped to {boundariesValue}.",value,boundariesValue);
            }
            // Ensure that the value is within the specified range
            int clampedValue = ClampedToMultipleOf5(boundariesValue);

            // Log a warning if clamping occurred
            if (boundariesValue != clampedValue)
            {
                DDNSLogger.Warning("Warning: TTL value {value} not multiple of 5. Clamped to {clampedValue}.",value,clampedValue);
            }
            _ttl = clampedValue;
        }
    }

    // NextRun property
    public DateTime NextRun
    {
        get => _nextRun;
        set => _nextRun = value;
    }

    // Method to clamp a value to the nearest multiple of 5 within the range [5, 60]
    private int ForceBounds(int value)
    {
        int lowerBound = 5;
        int upperBound = 60;

        // Ensure that the value is within the limits
        value = Math.Clamp(value, lowerBound, upperBound);
        return value;
    }

    private int ClampedToMultipleOf5(int value)
    {
        // Calculate the nearest multiple of 5
        int closestMultiple = (int)(Math.Round(value / 5.0) * 5);

        return closestMultiple;
    }
}
