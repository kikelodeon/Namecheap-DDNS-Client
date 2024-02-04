namespace eggDDNS
{
    class RunCommand : Command
    {
        public override string[] Commands => new[] { "run" };

         public override void Execute(string[] args)
        {
            Logger.Debug("Executing RunCommand...");
            const string mutexName = "Global\\DDNSUpdaterMutex";
            using (Mutex mutex = new Mutex(true, mutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    Logger.Critical("Another instance of DDNSUpdater is already running. Exiting...");
                    return;
                }

                string configFolderPath = args[0];

                try
                {
                    Updater.Run(configFolderPath);
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
