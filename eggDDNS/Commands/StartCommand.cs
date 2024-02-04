namespace eggDDNS
{
    class StartCommand : Command
    {
        public override string[] Commands => new[] { "start" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing StartCommand...");
            // Add logic for the 'start' command here
        }
    }
}
