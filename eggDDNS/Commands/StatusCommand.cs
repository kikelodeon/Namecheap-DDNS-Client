using System.Diagnostics;

namespace eggDDNS
{
    class StatusCommand : Command
    {
        public override string[] Triggers => new[] { "status" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing StatusCommand...");
            // Execute platform-dependent command to check service status
            if (OperatingSystem.IsWindows())
            {
                Process.Start("sc", "query " + Constants.ApplicationName);
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("systemctl", "status " + Constants.ApplicationName);
            }
            else
            {
                Logger.Critical("Your platform not support this command.");
            }
        }
    }
}
