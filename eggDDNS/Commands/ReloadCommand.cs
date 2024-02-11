using System.Diagnostics;

namespace eggDDNS
{
    class ReloadCommand : Command
    {
        public override string[] Triggers => new[] { "reload" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing ReloadCommand...");
            // Execute platform-dependent command to reload service
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                Process.Start("systemctl", "reload " + Constants.ApplicationName);
            }
            else
            {
                Logger.Critical("Your platform not support this command.");
            }
        }
    }
}
