namespace eggDDNS
{
    class DisableCommand : Command
    {
        public override string[] Commands => new[] { "disable" };

      public override void Execute(string[]? args=null)
        {
            Logger.Debug("Executing DisableCommand...");
            // Add logic for the 'disable' command here
        }
    }
}
