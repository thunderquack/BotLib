namespace BotLib.Engine.Messages
{
    public abstract class TelegramMessage
    {
        public TelegramMessage(long chatId, bool disableWebPagePreview = false)
        {
            this.ChatId = chatId;
            this.DisableWebPagePreview = disableWebPagePreview;
            SetMessageType();
        }

        public long ChatId { get; }
        public bool DisableWebPagePreview { get; protected set; }
        public TelegramMessageType MessageType { get; protected set; }

        protected abstract void SetMessageType();
    }
}