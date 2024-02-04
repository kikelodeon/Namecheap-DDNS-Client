namespace eggDDNS
{
    class HelpCommand : Command
    {
        public override string[] Commands => new[] { "--help", "h", "help", "/help", "/h", "-h", "?" };

        public override void Execute(string[] args)
        {
            Logger.Debug("Executing HelpCommand...");
            if (string.IsNullOrEmpty(helpMessageColors.Trim()))
            {
                Console.WriteLine(helpMessageText);
                return;
            }
            string[] textLines = helpMessageText.Split('\n');
            string[] colorLines = helpMessageColors.Split('\n');

            for (int j = 0; j < textLines.Length; j++)
            {
                ConsoleColor color = ConsoleColor.White;
                for (int i = 0; i < textLines[j].Length; i++)
                {
                    char? number = colorLines.Length > j && colorLines[j].Length > i ? colorLines[j][i] : null;
                    color = number != null && mapping.ContainsKey((char)number) ? mapping[(char)number] : color;
                    Console.ForegroundColor = color;
                    Console.Write(textLines[j][i]);
                }
                Console.ResetColor();
                Console.WriteLine(); // Move to the next line after completing each line.
            }
        }

        private Dictionary<char, ConsoleColor> mapping = new Dictionary<char, ConsoleColor>
        {
            ['0'] = ConsoleColor.Green,
            ['1'] = ConsoleColor.DarkGray,
            ['2'] = ConsoleColor.Cyan,
            ['3'] = ConsoleColor.Blue,
            ['4'] = ConsoleColor.DarkBlue,
            ['5'] = ConsoleColor.DarkRed,
            ['6'] = ConsoleColor.Yellow,
            ['7'] = ConsoleColor.Magenta,
            ['8'] = ConsoleColor.DarkBlue,
            ['9'] = ConsoleColor.White
        };
        private const string helpMessageText = @"
        eggDDNS - Dynamic DNS Updater

        Usage:
        eggDDNS [<command>]

        Commands:
        run                     Runs eggDDNS.
        enable                  Enable service
        disable                 Disable service
        start                   Start service
        restart                 Restart service
        stop                    Stop service
        status                  Display service status         
        main                    Displays the main menu.        
        list-host               Lists the loaded hosts in the config folder.
        add-host                Add host to hosts config folder.
                                > Example: eggDDNS --add-host /path/to/file.json
        remove-host             Remove host from hosts config folder.
                                > Example: eggDDNS --remove-host /path/to/file.json
        add-ip-provider         Add public IP provider.
                                > Example: eggDDNS --add-ip-provider http://example.com/api/ip
        remove-ip-provider      Remove public IP provider.
                                > Example: eggDDNS --remove-ip-provider http://example.com/api/ip

        Logging:
        log                   Displays log. (use -t for tail)
        log-path              Output the log filepath
        help                  Displays this message.
     ";
        private const string helpMessageColors = @"
2

0
        9999999 6

2
        6                    9
        6                    9
        6                    9
        6                    9
        6                    9
        6                    9
        6                    9
        6                    9
        6                    9
        6                    9
                                7          1
        6                    9
                                7          1
        6                    9
                                7          1
        6                    9
                                7          1

2
        6                    9
        6                    9
        6                    9 
     ";

    }
}