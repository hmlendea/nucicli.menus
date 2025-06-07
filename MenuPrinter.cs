using System;
using System.Collections.Generic;
using System.Linq;

using NuciExtensions;

namespace NuciCLI.Menus
{
    internal static class MenuPrinter
    {
        public static void PrintMenuHeader(Menu menu)
        {
            PrintTitle(menu);
            PrintCommandList(menu.Commands);
            NuciConsole.WriteLine();
        }

        /// <summary>
        /// Prints the title.
        /// </summary>
        public static void PrintTitle(Menu menu)
        {
            NuciConsole.Write(menu.TitleDecoration, menu.TitleDecorationColour);
            NuciConsole.Write(menu.Title, menu.TitleColour);
            NuciConsole.Write(menu.TitleDecoration.Reverse(), menu.TitleDecorationColour);

            NuciConsole.WriteLine();
        }

        /// <summary>
        /// Prints the command list.
        /// </summary>
        public static void PrintCommandList(IDictionary<string, Command> commands)
        {
            int commandColumnWidth = commands.Keys.Max(x => x.Length) + 4;

            foreach (Command command in commands.Values)
            {
                string formattedName = command.Name.PadRight(commandColumnWidth);

                NuciConsole.WriteLine($"{formattedName} {command.Description}");
            }
        }

        public static void PrintCommandResults(CommandResult result)
        {
            NuciConsole.WriteLine();
            NuciConsole.Write("Command finished with status ");

            string durationString = GetHumanFriendlyDurationString(result.Duration);

            if (result.Status == CommandStatus.Success)
            {
                NuciConsole.Write("Success", NuciConsoleColour.Green);
            }
            else if (result.Status == CommandStatus.Failure)
            {
                NuciConsole.Write("Failed", NuciConsoleColour.Red);
            }
            else if (result.Status == CommandStatus.Cancelled)
            {
                NuciConsole.Write("Cancelled", NuciConsoleColour.Yellow);
            }

            NuciConsole.Write($" after {durationString}");

            if (result.Status == CommandStatus.Failure)
            {
                NuciConsole.WriteLine($"Error message: {result.Exception.Message}", NuciConsoleColour.Red);
            }

            NuciConsole.WriteLine();
        }

        private static string GetHumanFriendlyDurationString(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 1)
            {
                return $"{timeSpan.TotalSeconds:0.00}s";
            }


            return $"{timeSpan.TotalMinutes:0.00}m";
        }
    }
}
