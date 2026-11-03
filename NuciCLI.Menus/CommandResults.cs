using System;

namespace NuciCLI.Menus
{
    /// <summary>
    /// Represents the result of a command execution.
    /// </summary>
    /// <param name="startTime">The time when the command started executing.</param>
    /// <param name="endTime">The time when the command finished executing.</param>
    internal sealed class CommandResult(DateTime startTime, DateTime endTime)
    {
        /// <summary>
        /// Gets the time when the command started executing.
        /// </summary>
        public DateTime StartTime { get; } = startTime;

        /// <summary>
        /// Gets the time when the command finished executing.
        /// </summary>
        public DateTime EndTime { get; } = endTime;

        /// <summary>
        /// Gets the duration of the command execution.
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Gets the exception that occurred during command execution, if any.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the status of the command execution based on whether an exception occurred.
        /// </summary>
        public CommandStatus Status
        {
            get
            {
                if (Exception is null)
                {
                    return CommandStatus.Success;
                }

                if (Exception is InputCancellationException)
                {
                    return CommandStatus.Cancelled;
                }

                return CommandStatus.Failure;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class with the specified start and end times.
        /// </summary>
        /// <param name="startTime">The time when the command started executing.</param>
        /// <param name="endTime">The time when the command finished executing.</param>
        /// <param name="exception">The exception that occurred during command execution, if any.</param>
        public CommandResult(
            DateTime startTime,
            DateTime endTime,
            Exception exception) : this(startTime, endTime)
            => Exception = exception;
    }
}
