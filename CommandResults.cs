using System;

namespace NuciCLI.Menus
{
    internal sealed class CommandResult(DateTime startTime, DateTime endTime)
    {
        public DateTime StartTime { get; } = startTime;

        public DateTime EndTime { get; } = endTime;

        public TimeSpan Duration => EndTime - StartTime;

        public Exception Exception { get; }

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

        public CommandResult(
            DateTime startTime,
            DateTime endTime,
            Exception exception) : this (startTime, endTime)
            => Exception = exception;
    }
}
