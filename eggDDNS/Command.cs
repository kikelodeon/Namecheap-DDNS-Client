
using System.Diagnostics;

namespace eggDDNS
{
    public abstract class Command
    {
        public abstract string[] Triggers { get; }
        public abstract void Execute(string[] args);
        // Common implementation for IsRequested
        public virtual bool IsRequested(string[] args)
        {
            return args.Length > 0 && Array.Exists(Triggers, arg => arg.Equals(args[0], StringComparison.OrdinalIgnoreCase));
        }
        public static bool RunShellCommand(string command, bool print = false)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();

                    // Read the standard output and error streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Wait for the command to finish
                    process.WaitForExit();

                    // Print output if requested
                    if (print)
                    {
                        Console.WriteLine(output);
                        Console.WriteLine(error);
                    }

                    // Return true if the exit code is 0 (success), otherwise false
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                // Handle exceptions (e.g., process start failure)
                // You can log the exception or take appropriate action based on your requirements
                return false;
            }
        }
    }
}