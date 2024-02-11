using System;
using System.IO;

namespace eggDDNS
{
    class LogCommand : Command
    {
        public override string[] Triggers => new[] { "log" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing LogCommand...");

            // Default number of lines to display
            int linesToDisplay = 15;

            // Check for parameters like "tail", "full", etc.
            foreach (var arg in args)
            {
                if (arg.ToLower() == "full")
                {
                    // Display the full log file
                    DisplayLogFile();
                    return;
                }
                if (arg.ToLower() == "live")
                {
                    // Display the full log file
                    DisplayLogFileRealTime();
                    return;
                }
            }

            // Default behavior: display the last 15 lines
            DisplayLastLines(linesToDisplay);
        }
        private void DisplayLogFileRealTime()
        {
            Logger.Info("Displaying monitor...");

            string? logFilePath = Logger.lastWritedFilename;
            if (logFilePath != null && File.Exists(logFilePath))
            {
                using (StreamReader reader = new StreamReader(logFilePath))
                {
                    Console.Clear(); // Clear console before displaying real-time log

                    // Store the last 15 lines in a circular buffer
                    int bufferSize = 15;
                    CircularBuffer<string> buffer = new CircularBuffer<string>(bufferSize);

                    // Read and store the last 15 lines
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        buffer.Enqueue(line);
                        if (buffer.Count > bufferSize)
                        {
                            buffer.Dequeue();
                        }
                    }

                    // Display the stored lines
                    foreach (var storedLine in buffer)
                    {
                        Console.WriteLine(storedLine);
                    }

                    // Continue reading and displaying new lines
                    while (true)
                    {
                        // Read new lines from where we left off
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                            // Optionally, you can add a delay here to control the rate of reading
                        }

                        // Wait for new lines to be added before trying to read again
                        Thread.Sleep(1000); // Adjust the sleep duration as needed
                    }
                }
            }
            else
            {
                Logger.Info("Log file not found.");
            }
        }

        private void DisplayLastLines(int lines)
        {
            Logger.Debug("Displaying last {lines} lines:", lines);
            string? logFilePath = Logger.lastWritedFilename;
            if (logFilePath != null && File.Exists(logFilePath))
            {
                string[] allLines = File.ReadAllLines(logFilePath);
                int startIndex = Math.Max(0, allLines.Length - lines);
                for (int i = startIndex; i < allLines.Length; i++)
                {
                    Console.WriteLine(allLines[i]);
                }
            }
            else
            {
                Logger.Info("Log file not found.");
            }
        }

        private void DisplayLogFile()
        {
            Logger.Info("Displaying full log, press spacebar to cancel:");
            string? logFilePath = Logger.lastWritedFilename;
            if (logFilePath != null && File.Exists(logFilePath))
            {
                using (StreamReader reader = new StreamReader(logFilePath))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);

                        // Pause log display if spacebar is pressed
                        if (Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                        {
                            Console.WriteLine("\nPress Spacebar to continue or any other key to exit...");
                            if (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
                                break;
                        }
                    }
                }
            }
            else
            {
                Logger.Info("Log file not found.");
            }
        }
    }
}
