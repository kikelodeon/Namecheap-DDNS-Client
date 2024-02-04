namespace eggDDNS
{
    class RestartCommand : Command
    {
        public override string[] Commands => new[] { "restart" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing RestartCommand...");
            // Add logic for the 'restart' command here
        }
    }
}
