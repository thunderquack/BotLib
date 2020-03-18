namespace BotLib.Engine.Messages
{
    public class TelegramEmptyMessage : TelegramMessage
    {
        public TelegramEmptyMessage(long ChatId) : base(ChatId)
        {
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.Empty;
        }
    }
}