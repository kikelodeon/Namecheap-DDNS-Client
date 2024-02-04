namespace eggDDNS
{
    class RemoveHostCommand : Command
    {
        public override string[] Commands => new[] { "remove-host" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing RemoveHostCommand...");
            // Add logic for the 'remove-host' command here
        }
    }
}
