namespace BotLib.Engine.Messages
{
    internal class TelegramTypingMessage : TelegramMessage
    {
        public TelegramTypingMessage(long ChatId) : base(ChatId)
        {
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.Typing;
        }
    }
}