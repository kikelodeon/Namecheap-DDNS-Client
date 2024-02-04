namespace eggDDNS
{
    class RemoveIpProviderCommand : Command
    {
        public override string[] Commands => new[] { "remove-ip-provider" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing RemoveIpProviderCommand...");
            // Add logic for the 'remove-ip-provider' command here
        }
    }
}
