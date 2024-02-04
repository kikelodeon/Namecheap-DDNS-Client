namespace eggDDNS
{
    class AddHostCommand : Command
    {
        public override string[] Commands => new[] { "add-host" };

         public override void Execute(string[] args)
        {
             Logger.Debug("Executing AddHostCommand...");
            // Add logic for the 'add-host' command here
        }
    }
}
