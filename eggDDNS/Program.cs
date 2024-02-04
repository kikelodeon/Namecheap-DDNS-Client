using eggDDNS;

class Program
{
    private static readonly List<Command> Commands = new List<Command>
    {
        new AddHostCommand(),
        new AddIpProviderCommand(),
        new DisableCommand(),
        new EnableCommand(),
        new HelpCommand(),
        new LogCommand(),
        new LogPathCommand(),
        new MainCommand(),
        new RemoveHostCommand(),
        new RemoveIpProviderCommand(),
        new RestartCommand(),
        new RunCommand(),
        new StartCommand(),
        new StatusCommand(),
        new StopCommand()
    };
    
    static void Main(string[] args)
    {
        Command? requestedCommand = Commands.FirstOrDefault(x=>x.IsRequested(args));

        if(requestedCommand !=null)
        {
            requestedCommand.Execute(args);
            return;
        }

        new MainCommand().Execute(args);
        return;
    }
}
