using System;
using System.Collections.Generic;
using System.Linq;

using NuciExtensions;

namespace NuciCLI.Menus
{
    internal static class MenuPrinter
    {
        private const int CommandColumnSpacing = 4;
        private const string SuccessStatusText = "Success";
        private const string FailureStatusText = "Failed";
        private const string CancelledStatusText = "Cancelled";

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
            int commandColumnWidth = commands.Keys.Max(name => name.Length) + CommandColumnSpacing;

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
            PrintStatus(result.Status);
            NuciConsole.Write($" after {GetHumanFriendlyDurationString(result.Duration)}");

            if (result.Status is CommandStatus.Failure)
            {
                NuciConsole.WriteLine($"Error message: {result.Exception?.Message}", NuciConsoleColour.Red);
            }

            NuciConsole.WriteLine();
        }

        private static void PrintStatus(CommandStatus status)
        {
            if (status is CommandStatus.Success)
            {
                NuciConsole.Write(SuccessStatusText, NuciConsoleColour.Green);
            }
            else if (status is CommandStatus.Failure)
            {
                NuciConsole.Write(FailureStatusText, NuciConsoleColour.Red);
            }
            else if (status is CommandStatus.Cancelled)
            {
                NuciConsole.Write(CancelledStatusText, NuciConsoleColour.Yellow);
            }
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
