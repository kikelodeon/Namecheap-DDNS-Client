using System.Diagnostics;

namespace eggDDNS
{
    class StartCommand : Command
    {
        public override string[] Triggers => new[] { "start" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing StartCommand...");
            // Execute platform-dependent command to start service
            if (OperatingSystem.IsWindows())
            {
                Process.Start("sc", "start " + Constants.ApplicationName);
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("systemctl", "start " + Constants.ApplicationName);
            }
            else
            {
                Logger.Critical("Your platform not support this command.");
            }
        }
    }
}
