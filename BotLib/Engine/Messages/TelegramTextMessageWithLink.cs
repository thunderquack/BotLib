using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLib.Engine.Messages
{
    public class TelegramTextMessageWithLink : TelegramTextMessage
    {
        private InlineKeyboardButton Link;

        public TelegramTextMessageWithLink(long ChatId, string Text, ParseMode ParseMode = ParseMode.Markdown) : base(ChatId, Text, ParseMode)
        {
        }

        public InlineKeyboardMarkup ReplyMarkup => new InlineKeyboardMarkup(Link);

        public void SetLink(string Caption, string Link)
        {
            this.Link = InlineKeyboardButton.WithUrl(Caption, Link);
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.TextWithLink;
        }
    }
}