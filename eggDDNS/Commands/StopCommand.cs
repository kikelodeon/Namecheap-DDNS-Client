namespace eggDDNS
{
    class StopCommand : Command
    {
        public override string[] Commands => new[] { "stop" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing StopCommand...");
            // Add logic for the 'stop' command here
        }
    }
}
