using Telegram.Bot.Types.Enums;

namespace BotLib.Engine.Messages
{
    public class TelegramTextMessageWithKeyboardHide : TelegramTextMessage
    {
        public TelegramTextMessageWithKeyboardHide(int originalMessageId, long chatId, string text, ParseMode parseMode = ParseMode.Markdown) : base(chatId, text, parseMode)
        {
            OriginalMessageId = originalMessageId;
        }

        public TelegramTextMessageWithKeyboardHide(int originalMessageId, TelegramTextMessageWithKeyboard originalMessage) : base(originalMessage.ChatId, originalMessage.Text, originalMessage.ParseMode)
        {
            OriginalMessageId = originalMessageId;
        }

        public int OriginalMessageId { get; }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.TextWithKeyboardHide;
        }
    }
}