namespace eggDDNS
{
    class ListIpProvidersCommand : Command
    {
        public override string[] Triggers => new[] { "list-ip-providers" };

        public override void Execute(string[]? args=null)
        {
            Logger.Debug("Executing ListIpProvidersCommand...");

            var cfPath = Config.GetConfigPathLocation(Constants.IpProvidersFolderName);
            Logger.Info("Searching files in {cfPath}...", cfPath);
            
            var configs = Config.GetAllConfigFilesList<IpProviderSettings>();
            Logger.Info("{number} ip provider config files found", configs.Count);
        }
    }
}
