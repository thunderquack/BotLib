using System.Linq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLib.Engine.Messages
{
    public class TelegramTextMessageWithKeyboard : TelegramTextMessage
    {
        internal InlineKeyboardButton[][] Keyboard;
        private bool MessageIdIsSet = false;

        public TelegramTextMessageWithKeyboard(long chatId, string text, ParseMode parseMode = ParseMode.Markdown, bool disableWebPagePreview = false) : base(chatId, text, parseMode)
        {
            Keyboard = new InlineKeyboardButton[0][];
            this.DisableWebPagePreview = disableWebPagePreview;
        }

        public int MessageId { get; private set; }

        public InlineKeyboardMarkup ReplyMarkup => new InlineKeyboardMarkup(Keyboard);

        public TelegramTextMessageWithKeyboard AddCallbackButton(string text, string inlineCommand, bool toBottom = false)
        {
            InlineKeyboardButton keyboardButton = InlineKeyboardButton.WithCallbackData(text, inlineCommand);
            AddButton(keyboardButton, toBottom);
            return this;
        }

        public TelegramTextMessageWithKeyboard AddCallbackWebButton(string text, string url, bool toBottom = false)
        {
            InlineKeyboardButton keyboardButton = InlineKeyboardButton.WithUrl(text, url);
            AddButton(keyboardButton, toBottom);
            return this;
        }

        public TelegramMessage GetMessageToHide()
        {
            if (MessageIdIsSet)
                return new TelegramTextMessageWithKeyboardHide(MessageId, this);
            else
                return new TelegramEmptyMessage(ChatId);
        }

        public void SetMessageId(int messageId)
        {
            this.MessageId = messageId;
            this.MessageIdIsSet = true;
        }

        /// <summary>
        /// Clears keyboard of the message
        /// </summary>
        protected TelegramTextMessageWithKeyboard ClearKeyboard()
        {
            Keyboard = new InlineKeyboardButton[0][];
            return this;
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.TextWithKeyboard;
        }

        private void AddButton(InlineKeyboardButton key, bool toBottom = false)
        {
            if (Keyboard.Length == 0)
            {
                InlineKeyboardButton[] kb = new InlineKeyboardButton[1];
                kb[0] = key;
                InlineKeyboardButton[][] keyb = new InlineKeyboardButton[1][];
                keyb[0] = kb;
                Keyboard = keyb;
                return;
            }
            else
            {
                if (!toBottom)
                {
                    if (Keyboard.Last().Length < MaximumButtonsInRow())
                    {
                        var l = Keyboard.Last().ToList();
                        l.Add(key);
                        Keyboard[Keyboard.Length - 1] = l.ToArray();
                        return;
                    }
                    else
                    {
                        InlineKeyboardButton[] kb = new InlineKeyboardButton[1];
                        kb[0] = key;
                        var k = Keyboard.ToList();
                        k.Add(kb);
                        Keyboard = k.ToArray();
                        return;
                    }
                }
                else
                {
                    InlineKeyboardButton[] kb = new InlineKeyboardButton[1];
                    kb[0] = key;
                    var k = Keyboard.ToList();
                    k.Add(kb);
                    Keyboard = k.ToArray();
                    return;
                }
            }
        }

        private int MaximumButtonsInRow()
        {
            return 3;
        }
    }
}