using System.Diagnostics;

namespace eggDDNS
{
    class DisableCommand : Command
    {
        public override string[] Triggers => new[] { "disable" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing DisableCommand...");
            // Execute platform-dependent command to disable service
            if (OperatingSystem.IsWindows())
            {
                Process.Start("sc", "config " + Constants.ApplicationName + " start=disabled");
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("systemctl", "disable " + Constants.ApplicationName);
            }
            else
            {
                Logger.Critical("Your platform not support this command.");
            }
        }
    }
}
