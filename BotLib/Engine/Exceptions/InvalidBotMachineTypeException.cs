using System;

namespace BotLib.Engine.Exceptions
{
    public class InvalidBotMachineTypeException : Exception
    {
        public Type InvalidType { get; }

        public InvalidBotMachineTypeException(string message, Type InvalidType) : base(message)
        {
            this.InvalidType = InvalidType;
        }
    }
}