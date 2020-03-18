using System;

namespace BotLib.Engine.Exceptions
{
    public class MessageIsNotSentException : Exception
    {
        public MessageIsNotSentException(long ChatId, string message) : base(message)
        {
            this.ChatId = ChatId;
        }

        public long ChatId { get; }
    }
}