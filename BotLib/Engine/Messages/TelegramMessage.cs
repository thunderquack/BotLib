namespace BotLib.Engine.Messages
{
    public abstract class TelegramMessage
    {
        public TelegramMessage(long ChatId, bool DisableWebPagePreview = false)
        {
            this.ChatId = ChatId;
            this.DisableWebPagePreview = DisableWebPagePreview;
            SetMessageType();
        }

        public long ChatId { get; }
        public bool DisableWebPagePreview { get; protected set; }
        public TelegramMessageType MessageType { get; protected set; }

        protected abstract void SetMessageType();
    }
}