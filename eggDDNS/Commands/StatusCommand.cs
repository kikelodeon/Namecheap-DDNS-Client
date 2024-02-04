namespace eggDDNS
{
    class StatusCommand : Command
    {
        public override string[] Commands => new[] { "status" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing StatusCommand...");
            // Add logic for the 'status' command here
        }
    }
}
