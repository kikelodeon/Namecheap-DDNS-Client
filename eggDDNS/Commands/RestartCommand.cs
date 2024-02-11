using System.Diagnostics;

namespace eggDDNS
{
    class RestartCommand : Command
    {
        public override string[] Triggers => new[] { "restart" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing RestartCommand...");
            // Execute platform-dependent command to restart service
            if (OperatingSystem.IsWindows())
            {
                Process.Start("sc", "restart " + Constants.ApplicationName);
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("systemctl", "restart " + Constants.ApplicationName);
            }
            else
            {
                Logger.Critical("Your platform not support this command.");
            }
        }
    }
}
