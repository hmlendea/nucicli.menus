using System;

namespace NuciCLI.Menus
{
    internal sealed class Command
    {
        public string Name { get; }

        public string Description { get; }

        Action action;

        public Command(string name, string description, Action action)
        {
            Name = name;
            Description = description;

            this.action = action;
        }

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
