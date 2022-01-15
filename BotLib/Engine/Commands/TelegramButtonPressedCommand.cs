using System;

namespace BotLib.Engine.Commands
{
    public class TelegramButtonPressedCommand : TelegramCommand
    {
        public TelegramButtonPressedCommand(long UserId, string CallbackData) : base(TelegramCommandType.ButtonPressed, UserId, CallbackData)
        {
        }

        public string CallbackData => Convert.ToString(Argument);
    }
}