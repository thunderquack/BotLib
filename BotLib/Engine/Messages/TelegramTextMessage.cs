using Telegram.Bot.Types.Enums;

namespace BotLib.Engine.Messages
{
    public class TelegramTextMessage : TelegramMessage
    {
        public static int TEXT_LENGTH = 2048;

        public TelegramTextMessage(long ChatId, string Text, ParseMode ParseMode = ParseMode.Markdown) : base(ChatId)
        {
            this.ParseMode = ParseMode;
            if (Text.Length > TEXT_LENGTH)
                this.Text = Text.Substring(0, TEXT_LENGTH);
            else
                this.Text = Text;
        }

        public ParseMode ParseMode { get; }
        public string Text { get; }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.Text;
        }
    }
}