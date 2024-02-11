using System.Text.Json;

namespace eggDDNS
{
     public class IpProviderSettings : Config
    {

        protected override string FolderName => Constants.IpProvidersFolderName;
        public string Name { get; set; }
        public string Url { get; set; }

        public IpProviderSettings()
        {
            Url = string.Empty;
            Name = string.Empty;
        }

        protected override bool IsValid()
        {
            return !string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(Name);
        }

        public override string ToString()
        {
            return $"Name={Name}, Url={Url}, {base.ToString()}";
        }
         public override string[] ToArray()
    {
        // Create an array with formatted string representations of properties specific to IpProviderConfig
        string[] result = new string[]
        {
            $"FilePath={FilePath}",
            $"Name={Name}",
            $"Url={Url}"
        };

        return result;
    }
    }
}
