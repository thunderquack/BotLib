using Newtonsoft.Json;
using System;

namespace BotLib.Engine.Messages
{
    [Serializable]
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

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected abstract void SetMessageType();
    }
}