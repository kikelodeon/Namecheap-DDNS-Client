namespace eggDDNS
{
    class ListHostsCommand : Command
    {
        public override string[] Triggers => new[] { "list-hosts" };

        public override void Execute(string[]? args=null)
        {
            Logger.Debug("Executing ListHostsCommand...");
            var cfPath = Config.GetConfigPathLocation(Constants.HostsFolderName);
            Logger.Info("Searching files in {cfPath}...", cfPath);
            
            var configs = Config.GetAllConfigFilesList<HostSettings>();
            Logger.Info("{number} host config files found", configs.Count);
        }
    }
}
