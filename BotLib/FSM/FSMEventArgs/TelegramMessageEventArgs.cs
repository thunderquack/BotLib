using BotLib.Engine.Messages;

namespace BotLib.FSM.FSMEventArgs
{
    public class TelegramMessageEventArgs : System.EventArgs
    {
        public TelegramMessage TelegramMessage { get; set; }
    }
}