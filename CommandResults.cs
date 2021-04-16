using System;

namespace NuciCLI.Menus
{
    internal sealed class CommandResult
    {
        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

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

        public CommandResult(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public CommandResult(DateTime startTime, DateTime endTime, Exception exception)
            : this (startTime, endTime)
        {
            Exception = exception;
        }
    }
}
