using System;
using System.Collections.Generic;
using System.Linq;

using NuciExtensions;

namespace NuciCLI.Menus
{
    internal static class MenuPrinter
    {
        /// <summary>
        /// Prints the menu header.
        /// </summary>
        /// <param name="menu">The menu to print the header for.</param>
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
                NuciConsole.WriteLine($"{command.Name.PadRight(commandColumnWidth)} {command.Description}");
            }
        }

        /// <summary>
        /// Prints the results of a command execution.
        /// </summary>
        /// <param name="result">The result of the command execution.</param>
        public static void PrintCommandResults(CommandResult result)
        {
            NuciConsole.WriteLine();
            NuciConsole.Write("Command finished with status ");

            string durationString = GetHumanFriendlyDurationString(result.Duration);

            if (result.Status is CommandStatus.Success)
            {
                NuciConsole.Write("Success", NuciConsoleColour.Green);
            }
            else if (result.Status.Equals(CommandStatus.Failure))
            {
                NuciConsole.Write("Failed", NuciConsoleColour.Red);
            }
            else if (result.Status.Equals(CommandStatus.Cancelled))
            {
                NuciConsole.Write("Cancelled", NuciConsoleColour.Yellow);
            }

            NuciConsole.Write($" after {durationString}");

            if (result.Status.Equals(CommandStatus.Failure))
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
