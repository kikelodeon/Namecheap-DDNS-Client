namespace eggDDNS
{
    class RunCommand : Command
    {
        public override string[] Triggers => new[] { "run" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing RunCommand...");
            const string mutexName = "Global\\eggDDDNS\\Run";
            using (Mutex mutex = new Mutex(true, mutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    Logger.Critical("Another instance of DDNSUpdater is already running. Exiting...");
                    return;
                }

                try
                {
                    Updater.Run();
                }
                catch (Exception ex)
                {
                    Logger.Critical($"Error: {ex.Message}");
                    Environment.Exit(1);
                }
            }
        }
    }
}
