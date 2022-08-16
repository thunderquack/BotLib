using Telegram.Bot.Types;

namespace BotLib.Engine.Commands
{
    public abstract class TelegramCommand
    {
        private Message _message = null;
        public TelegramCommandType CommandType { get; }
        public long UserId { get; }
        public object Argument { get; }

        public Message Message
        {
            get
            {
                return _message;
            }
        }

        public TelegramCommand(TelegramCommandType commandType, long UserId, object Argument)
        {
            CommandType = commandType;
            this.UserId = UserId;
            this.Argument = Argument;
        }

        internal void SetMessage(Message message)
        {
            _message = message;
        }
    }
}