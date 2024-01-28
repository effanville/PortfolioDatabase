namespace Common.Console;

public sealed class ConsoleCommandArgs
{
    public string[] Args { get; set; }

    public ConsoleCommandArgs(string[] args)
    {
        Args = args;
    }
}