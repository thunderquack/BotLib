using System.Linq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLib.Engine.Messages
{
    public class TelegramTextMessageWithKeyboard : TelegramTextMessage
    {
        internal InlineKeyboardButton[][] Keyboard;
        private bool MessageIdIsSet = false;

        public TelegramTextMessageWithKeyboard(long ChatId, string Text, ParseMode ParseMode = ParseMode.Markdown, bool DisableWebPagePreview = false) : base(ChatId, Text, ParseMode)
        {
            Keyboard = new InlineKeyboardButton[0][];
            this.DisableWebPagePreview = DisableWebPagePreview;
        }

        public int MessageId { get; private set; }

        public InlineKeyboardMarkup ReplyMarkup => new InlineKeyboardMarkup(Keyboard);

        public void AddCallbackButton(string Text, string InlineCommand, bool ToBottom = false)
        {
            if (Keyboard.Length == 0)
            {
                InlineKeyboardButton[] kb = new InlineKeyboardButton[1];
                InlineKeyboardButton key = InlineKeyboardButton.WithCallbackData(Text, InlineCommand);
                kb[0] = key;
                InlineKeyboardButton[][] keyb = new InlineKeyboardButton[1][];
                keyb[0] = kb;
                Keyboard = keyb;
                return;
            }
            else
            {
                if (!ToBottom)
                {
                    if (Keyboard.Last().Length < MaximumButtonsInRow())
                    {
                        var l = Keyboard.Last().ToList();
                        l.Add(InlineKeyboardButton.WithCallbackData(Text, InlineCommand));
                        Keyboard[Keyboard.Length - 1] = l.ToArray();
                        return;
                    }
                    else
                    {
                        InlineKeyboardButton[] kb = new InlineKeyboardButton[1];
                        InlineKeyboardButton key = InlineKeyboardButton.WithCallbackData(Text, InlineCommand);
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
                    InlineKeyboardButton key = InlineKeyboardButton.WithCallbackData(Text, InlineCommand);
                    kb[0] = key;
                    var k = Keyboard.ToList();
                    k.Add(kb);
                    Keyboard = k.ToArray();
                    return;
                }
            }
        }

        public TelegramMessage GetMessageToHide()
        {
            if (MessageIdIsSet)
                return new TelegramTextMessageWithKeyboardHide(MessageId, this);
            else
                return new TelegramEmptyMessage(ChatId);
        }

        public void SetMessageId(int MessageId)
        {
            this.MessageId = MessageId;
            this.MessageIdIsSet = true;
        }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.TextWithKeyboard;
        }

        private int MaximumButtonsInRow()
        {
            return 3;
        }
    }
}