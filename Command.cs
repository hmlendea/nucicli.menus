using System;

namespace NuciCLI.Menus
{
    /// <summary>
    /// Represents a command that can be executed.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <param name="description">The description of the command.</param>
    /// <param name="action">The action to execute when the command is invoked.</param>
    internal sealed class Command(string name, string description, Action action)
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        public string Description { get; } = description;

        Action action = action;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/> representing the result of the command execution.</returns>
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
