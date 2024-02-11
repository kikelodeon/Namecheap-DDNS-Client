using System.Text.Json;

namespace eggDDNS
{
    public abstract class Config
    {
        protected abstract string FolderName { get; }
        protected string _filePath { get; set; } = null!;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        public static string GetConfigPathLocation(string FolderName)
        {
            return Path.Combine("/", Constants.RootAppDataFolderName, Constants.ApplicationName, Constants.AppDataFolderName, FolderName);
        }

        public static List<T> GetAllConfigFilesList<T>() where T : Config, new()
        {
            T instance = new T();
            string folderName = instance.FolderName;
            string configPath = GetConfigPathLocation(folderName);
            if (!Directory.Exists(configPath))
            {
                Logger.Warning("Config directory '{configPath}' not found.", configPath);
                return new List<T>();
            }

            string[] files = Directory.GetFiles(configPath);

            List<T> configs = new List<T>();
            foreach (var file in files)
            {
                var config = Read<T>(file);
                if (config != null)
                {
                    config.FilePath = file;
                    configs.Add(config);
                }
            }
            return configs;
        }

        protected static T? Read<T>(string filePath) where T : Config
        {
            string configJson = File.ReadAllText(filePath);

            try
            {
                T? config = JsonSerializer.Deserialize<T>(configJson);

                if (config == null || !config.IsValid())
                {
                    throw new Exception("Invalid or incomplete configuration file.");
                }

                Logger.Info("Config {type} : {config} has been loaded successfully.", typeof(T).Name, config.ToArray());
                return config;
            }
            catch (Exception ex)
            {
                Logger.Critical("Error parsing configuration file: {ex});", ex.Message);
                return null;
            }
        }

        protected abstract bool IsValid();

        public override string ToString()
        {
            // Override ToString to provide a formatted string representation of the configuration
            return $"FilePath={FilePath}";
        }
        public virtual string[] ToArray()
        {
            // Return an array with a single element containing the formatted string representation of the configuration
            return [$"FilePath={FilePath}"];
        }
    }
}