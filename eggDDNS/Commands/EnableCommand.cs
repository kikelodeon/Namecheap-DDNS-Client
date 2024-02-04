namespace eggDDNS
{
    class EnableCommand : Command
    {
        public override string[] Commands => new[] { "enable" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing EnableCommand...");
            // Add logic for the 'enable' command here
        }
    }
}
