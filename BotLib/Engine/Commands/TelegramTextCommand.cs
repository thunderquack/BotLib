using BotLib.Engine.Exceptions;
using System;

namespace BotLib.Engine.Commands
{
    public class TelegramTextCommand : TelegramCommand
    {
        public TelegramTextCommand(long UserId, object Argument) : base(TelegramCommandType.TextEntered, UserId, Argument)
        {
            if (Argument.GetType() != typeof(string)) throw new InvalidCommandArgumentException("Invalid type of command's argument", Argument.GetType());
        }

        public string Text => Convert.ToString(Argument);
    }
}