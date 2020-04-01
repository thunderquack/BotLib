namespace BotLib.Engine.Messages
{
    public class TelegramTextMessageWithKeyboardEdited : TelegramTextMessageWithKeyboard
    {
        public TelegramTextMessageWithKeyboardEdited(int originalMessageId, TelegramTextMessageWithKeyboard originalMessage, bool disableWebPagePreview = false) : base(originalMessage.ChatId, originalMessage.Text, originalMessage.ParseMode, disableWebPagePreview)
        {
            OriginalMessageId = originalMessageId;
            Keyboard = originalMessage.Keyboard;
            SetText(originalMessage.Text);
        }

        public TelegramTextMessageWithKeyboardEdited(int originalMessageId, TelegramTextMessageWithKeyboard originalMessage, string newText, bool disableWebPagePreview = false) : base(originalMessage.ChatId, originalMessage.Text, originalMessage.ParseMode, disableWebPagePreview)
        {
            OriginalMessageId = originalMessageId;
            Keyboard = originalMessage.Keyboard;
            SetText(newText);
        }

        public int OriginalMessageId { get; }

        public void SetText(string text)
        {
            Text = text;
            CutText();
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.TextWithKeyboardEdited;
        }
    }
}