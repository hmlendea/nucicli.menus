using System;

namespace NuciCLI.Menus
{
    internal sealed class Command(string name, string description, Action action)
    {
        public string Name { get; } = name;

        public string Description { get; } = description;

        Action action = action;

        public CommandResult Execute()
        {
            CommandResult result;
            DateTime startTime = DateTime.Now;

            try
            {
                action();
                result = new CommandResult(startTime, DateTime.Now);
            }
            catch (InputCancellationException ex)
            {
                result = new CommandResult(startTime, DateTime.Now, ex);
            }
            catch (Exception ex)
            {
                result = new CommandResult(startTime, DateTime.Now, ex);
                throw;
            }

            return result;
        }
    }
}
