using System;

namespace BotLib.Engine.Exceptions
{
    public class InvalidCommandArgumentException : Exception
    {
        public Type GivenType { get; }

        public InvalidCommandArgumentException(string message, Type GivenType) : base(message)
        {
            this.GivenType = GivenType;
        }
    }
}