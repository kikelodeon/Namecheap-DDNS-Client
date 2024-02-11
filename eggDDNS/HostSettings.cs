using System.Text.Json;

namespace eggDDNS
{
    public class HostSettings : Config
    {
        protected override string FolderName => Constants.HostsFolderName;
        public string Host { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
        private int _ttl;
        private DateTime _nextRun = DateTime.Now;

        public HostSettings()
        {
            Host = string.Empty;
            Domain = string.Empty;
            Password = string.Empty;
            IP = string.Empty;
            TTL = 15;
        }

        public int TTL
        {
            get => _ttl;
            set
            {
                int boundariesValue = ForceBounds(value);
                if (value != boundariesValue)
                {
                    Logger.Warning("Warning: TTL value {value} is outside the allowed range [5, 60]. Clamped to {boundariesValue}.", value, boundariesValue);
                }
                int clampedValue = ClampedToMultipleOf5(boundariesValue);
                if (boundariesValue != clampedValue)
                {
                    Logger.Warning("Warning: TTL value {value} not multiple of 5. Clamped to {clampedValue}.", value, clampedValue);
                }
                _ttl = clampedValue;
            }
        }

        public DateTime NextRun
        {
            get => _nextRun;
            set => _nextRun = value;
        }

        protected override bool IsValid()
        {
            return !string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(Domain) &&
                   !string.IsNullOrEmpty(Password) && _ttl > 0;
        }

        private int ForceBounds(int value)
        {
            int lowerBound = 5;
            int upperBound = 60;
            value = Math.Clamp(value, lowerBound, upperBound);
            return value;
        }

        private int ClampedToMultipleOf5(int value)
        {
            int closestMultiple = (int)(Math.Round(value / 5.0) * 5);
            return closestMultiple;
        }

        public override string ToString()
        {
            return $"Host={Host}, Domain={Domain}, Password=****, IP={IP}, Interval={TTL} minutes, {base.ToString()}";
        }
        public override string[] ToArray()
        {
            // Create an array with formatted string representations of properties specific to HostConfig
            string[] result = new string[]
            {
            $"FilePath={FilePath}",
            $"Host={Host}",
            $"Domain={Domain}",
            $"Password=****",
            $"IP={IP}",
            $"Interval={TTL} minutes"
            };

            return result;
        }
    }

}
