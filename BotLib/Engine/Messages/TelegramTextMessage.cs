using Telegram.Bot.Types.Enums;

namespace BotLib.Engine.Messages
{
    public class TelegramTextMessage : TelegramMessage
    {
        public static int TEXT_LENGTH = 2048;

        public TelegramTextMessage(long chatId, string text, ParseMode parseMode = ParseMode.Markdown) : base(chatId)
        {
            ParseMode = parseMode;
            Text = text;
            CutText();
        }

        public ParseMode ParseMode { get; }

        public string Text { get; protected set; }

        protected void CutText()
        {
            if (Text.Length > TEXT_LENGTH)
                Text = Text.Substring(0, TEXT_LENGTH);
            else
                Text = Text;
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.Text;
        }
    }
}