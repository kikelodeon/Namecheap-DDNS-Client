public class DDNSConfig
{
    private int _ttl;
    private DateTime _nextRun = DateTime.Now.Date;

    public DDNSConfig()
    {
        Host = string.Empty;
        Domain = string.Empty;
        Password = string.Empty;
        IP = string.Empty;
        TTL = 15;
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
                DDNSUpdater.LogWarning($"Warning: TTL value {value} is outside the allowed range [5, 60]. Clamped to {boundariesValue}.");
            }
            // Ensure that the value is within the specified range
            int clampedValue = ClampedToMultipleOf5(boundariesValue);

            // Log a warning if clamping occurred
            if (boundariesValue != clampedValue)
            {
                DDNSUpdater.LogWarning($"Warning: TTL value {value} not multiple of 5. Clamped to {clampedValue}.");
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
