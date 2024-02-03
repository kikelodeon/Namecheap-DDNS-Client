class Program
{
    static void Main(string[] args)
    {
        const string mutexName = "Global\\DDNSUpdaterMutex";

        using (Mutex mutex = new Mutex(true, mutexName, out bool createdNew))
        {
            if (args.Length == 0 || args.Length == 1 && args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp();
                return;
            }
            if (!createdNew)
            {
                Console.WriteLine("Another instance of DDNSUpdater is already running. Exiting...");
                return;
            }

            string configFolderPath = args[0];

            try
            {
                DDNSUpdater.Run(configFolderPath);
            }
            catch (Exception ex)
            {
                DDNSLogger.Critical($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine(@"
       ╔══════════════════════════════════════════════════════════╗
       ║                  Namecheap DDNS Updater                  ║
       ║══════════════════════════════════════════════════════════║
       ║       Dynamic DNS Ip Updater for Namecheap Domains       ║
       ╚══════════════════════════════════════════════════════════╝

        Usage:
            NameCheap-DDNS-Updater /path/to/hosts
                    
        Options:
            --help Displays this mesage.

       ╔══════════════════════════════════════════════════════════╗
       ║                                              @kikelodeon ║
       ║                               Release date: January 2024 ║
       ╚══════════════════════════════════════════════════════════╝
                                                          v 1.0.2
                                                                ");

        }
    }
}
