class Program
{
  static void Main(string[] args)
    {
        const string mutexName = "Global\\DDNSUpdaterMutex";

        using (Mutex mutex = new Mutex(true, mutexName, out bool createdNew))
        {
            if (!createdNew)
            {
                Console.WriteLine("Another instance of DDNSUpdater is already running. Exiting...");
                return;
            }

            if (args.Length == 0)
            {
                DisplayHelp();
                return;
            }

            string configFolderPath = args[0];

            try
            {
                DDNSUpdater.Run(configFolderPath);
            }
            catch (Exception ex)
            {
                DDNSUpdater.LogCritical($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }


        static void DisplayHelp()
    {

        Console.WriteLine(@"
         ╔════════════════════════════════════════════════════╗
         ║               Namecheap DDNSUpdater                ║
         ║════════════════════════════════════════════════════║
         ║    Dynamic DNS Ip Updater for Namecheap Domains    ║
         ╚════════════════════════════════════════════════════╝

                Usage:
                    DDNSUpdater <config_folder_path>

         ╔════════════════════════════════════════════════════╗
         ║                  Contact: kikelodeon@eggmedia.net  ║
         ║                        Release date: January 2024  ║
         ║                                      Version: 1.0  ║
         ╚════════════════════════════════════════════════════╝");

    }
}
