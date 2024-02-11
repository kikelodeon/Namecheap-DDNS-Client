using System.Diagnostics;

namespace eggDDNS
{
    class StopCommand : Command
    {
        public override string[] Triggers => new[] { "stop" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing StopCommand...");
            // Execute platform-dependent command to stop service
            if (OperatingSystem.IsWindows())
            {
                Process.Start("sc", "stop " + Constants.ApplicationName);
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("systemctl", "stop " + Constants.ApplicationName);
            }
            else
            {
                Logger.Critical("Your platform not support this command.");
            }
        }
    }
}
