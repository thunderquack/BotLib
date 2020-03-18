using Telegram.Bot.Types.Enums;

namespace BotLib.Engine.Messages
{
    public class TelegramTextMessageWithKeyboardHide : TelegramTextMessage
    {
        public int OriginalMessageId { get; }

        public TelegramTextMessageWithKeyboardHide(int OriginalMessageId, long ChatId, string Text, ParseMode ParseMode = ParseMode.Markdown) : base(ChatId, Text, ParseMode)
        {
            this.OriginalMessageId = OriginalMessageId;
        }

        public TelegramTextMessageWithKeyboardHide(int OriginalMessageId, TelegramTextMessageWithKeyboard OriginalMessage) : base(OriginalMessage.ChatId, OriginalMessage.Text, OriginalMessage.ParseMode)
        {
            this.OriginalMessageId = OriginalMessageId;
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.TextWithKeyboardHide;
        }
    }
}