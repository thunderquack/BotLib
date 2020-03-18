using System;

namespace BotLib.Engine.Exceptions
{
    public class InvalidBotStateTypeException : Exception
    {
        public Type InvalidType { get; }

        public InvalidBotStateTypeException(string message, Type InvalidType) : base(message)
        {
            this.InvalidType = InvalidType;
        }
    }
}