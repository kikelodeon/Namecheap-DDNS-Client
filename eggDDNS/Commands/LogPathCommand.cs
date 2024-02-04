namespace eggDDNS
{
    class LogPathCommand : Command
    {
        public override string[] Commands => new[] { "log-path" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing LogPathCommand...");
            // Add logic for the 'log-path' command here
        }
    }
}
