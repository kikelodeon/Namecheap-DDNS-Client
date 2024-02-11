namespace eggDDNS
{
    class LogPathCommand : Command
    {
        public override string[] Triggers => new[] { "log-path" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing LogPathCommand...");
            Logger.Info("Last log file path: {log}", Logger.lastWritedFilename);
        }
    }
}
