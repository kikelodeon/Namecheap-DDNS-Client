
namespace eggDDNS{
    public abstract class Command
    {
        public abstract string[] Commands { get; }
        public abstract void Execute(string[] args);
        // Common implementation for IsRequested
        public virtual bool IsRequested(string[] args)
        {
            return args.Length > 0 && Array.Exists(Commands, arg => arg.Equals(args[0], StringComparison.OrdinalIgnoreCase));
        }
    }
}