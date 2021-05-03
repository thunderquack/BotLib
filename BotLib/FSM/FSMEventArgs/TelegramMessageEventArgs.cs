using BotLib.Engine.Messages;

namespace BotLib.FSM.FSMEventArgs
{
    public class TelegramMessageEventArgs : System.EventArgs
    {
        public bool Immediately { get; set; }
        public TelegramMessage TelegramMessage { get; set; }
    }
}