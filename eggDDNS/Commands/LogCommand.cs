namespace eggDDNS
{
    class LogCommand : Command
    {
        public override string[] Commands => new[] { "log" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing LogCommand...");
            // Add logic for the 'log' command here
        }
    }
}
