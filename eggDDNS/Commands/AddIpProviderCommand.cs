namespace eggDDNS
{
    class AddIpProviderCommand : Command
    {
        public override string[] Commands => new[] { "add-ip-provider" };

    public override void Execute(string[]? args=null)
        {
            Logger.Debug("Executing AddIpProviderCommand...");
            // Add logic for the 'add-ip-provider' command here
        }
    }
}
