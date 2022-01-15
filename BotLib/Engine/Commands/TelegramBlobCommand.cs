using BotLib.Engine.Exceptions;
using System;

namespace BotLib.Engine.Commands
{
    public abstract class TelegramBlobCommand : TelegramCommand
    {
        public TelegramBlobCommand(TelegramCommandType CommandType, long UserId, object Argument) : base(CommandType, UserId, Argument)
        {
            if (Argument.GetType() != typeof(byte[]))
                throw new InvalidCommandArgumentException("Should be byte[]", Argument.GetType());
        }

        public string FileText => Convert.ToBase64String(Argument as byte[]);

        public byte[] RawBytes => (Argument as byte[]);
    }
}