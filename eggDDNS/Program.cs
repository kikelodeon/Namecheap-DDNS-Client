using eggDDNS;

class Program
{
    private static readonly List<Command> Commands = new List<Command>
    {
        new ListHostsCommand(),
        new ListIpProvidersCommand(),
        new DisableCommand(),
        new EnableCommand(),
        new HelpCommand(),
        new LogCommand(),
        new LogPathCommand(),
        new MainCommand(),
        new RestartCommand(),
        new RunCommand(),
        new StartCommand(),
        new StatusCommand(),
        new StopCommand()
    };

    static void Main(string[] args)
    {
        Logger.Init();
        Command? requestedCommand = Commands.FirstOrDefault(x => x.IsRequested(args));

        if (requestedCommand != null)
        {
            requestedCommand.Execute(args);
            return;
        }

        new MainCommand().Execute(args);
        return;
    }
}
