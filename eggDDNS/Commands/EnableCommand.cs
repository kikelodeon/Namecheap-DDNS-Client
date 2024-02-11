using System.Diagnostics;

namespace eggDDNS
{
    class EnableCommand : Command
    {
        public override string[] Triggers => new[] { "enable" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing EnableCommand...");
            // Execute platform-dependent command to enable service
            if (OperatingSystem.IsWindows())
            {
                Process.Start("sc", "config " + Constants.ApplicationName + " start=auto");
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("systemctl", "enable " + Constants.ApplicationName);
            }
            else
            {
                Logger.Critical("Your platform not support this command.");
            }
        }
    }
}
