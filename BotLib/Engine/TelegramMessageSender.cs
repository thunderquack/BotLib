using BotLib.Engine.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BotLib.Engine
{
    public class TelegramMessageSender
    {
        private const int SENDING_INTERVAL = 50;
        private TelegramFSMBot bot;
        private LinkedList<TelegramMessage> Queue;
        private Timer SendTimer;

        /// <summary>
        /// Creates an instance of Sender
        /// </summary>
        /// <param name="bot">Reference to the bot</param>
        public TelegramMessageSender(TelegramFSMBot bot)
        {
            this.bot = bot;
            Queue = new LinkedList<TelegramMessage>();
            SendTimer = new Timer(SENDING_INTERVAL);
            SendTimer.Elapsed += TimeToSend;
            SendTimer.Start();
        }

        /// <summary>
        /// Creates an instance of Sender
        /// </summary>
        /// <param name="bot">Reference to the bot</param>
        /// <param name="SendingInterval">Interval of sending in milliseconds</param>
        public TelegramMessageSender(TelegramFSMBot bot, int SendingInterval = 50)
        {
            this.bot = bot;
            Queue = new LinkedList<TelegramMessage>();
            SendTimer = new Timer(SendingInterval);
            SendTimer.Elapsed += TimeToSend;
            SendTimer.Start();
        }

        public double Interval
        {
            get
            {
                return SendTimer.Interval;
            }
        }

        public void Add(TelegramMessage telegramMessage)
        {
            Queue.AddFirst(telegramMessage);
        }

        public void Enqueue(TelegramMessage telegramMessage)
        {
            Queue.AddLast(telegramMessage);
        }

        public void SetNewInterval(double interval)
        {
            SendTimer.Interval = interval;
        }

        private void TimeToSend(object sender, ElapsedEventArgs e)
        {
            SendTimer.Stop();
            if (Queue.Count > 0)
            {
                TelegramMessage message;
                message = Queue.First.Value;
                Queue.RemoveFirst();
                switch (message.MessageType)
                {
                    case TelegramMessageType.Empty:
                        break;

                    case TelegramMessageType.Text:
                        {
                            TelegramTextMessage msg = message as TelegramTextMessage;
                            int result;
                            try
                            {
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, msg.ParseMode, disableWebPagePreview: msg.DisableWebPagePreview).Result.MessageId;
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                                try
                                {
                                    result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, ParseMode.Default).Result.MessageId;
                                    bot.SetLastMessageId(msg.ChatId, result);
                                }
                                catch (Exception internalError)
                                {
                                    BotUtils.LogException(internalError);
                                }
                            }
                        }
                        break;

                    case TelegramMessageType.TextWithKeyboard:
                        {
                            TelegramTextMessageWithKeyboard msg = message as TelegramTextMessageWithKeyboard;
                            int result;
                            try
                            {
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, msg.ParseMode, replyMarkup: msg.ReplyMarkup, disableWebPagePreview: msg.DisableWebPagePreview).Result.MessageId;
                                msg.SetMessageId(result);
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, ParseMode.Default, replyMarkup: msg.ReplyMarkup, disableWebPagePreview: msg.DisableWebPagePreview).Result.MessageId;
                            }
                        }
                        break;

                    case TelegramMessageType.TextWithKeyboardHide:
                        {
                            TelegramTextMessageWithKeyboardHide msg = message as TelegramTextMessageWithKeyboardHide;
                            int result;
                            try
                            {
                                result = bot.EditMessageReplyMarkupAsync(msg.ChatId, msg.OriginalMessageId).Result.MessageId;
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                            }
                        }
                        break;

                    case TelegramMessageType.TextWithLink:
                        {
                            TelegramTextMessageWithLink msg = message as TelegramTextMessageWithLink;
                            int result;
                            try
                            {
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, msg.ParseMode, replyMarkup: msg.ReplyMarkup).Result.MessageId;
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, ParseMode.Default, replyMarkup: msg.ReplyMarkup).Result.MessageId;
                            }
                        }
                        break;

                    case TelegramMessageType.Payment:
                        {
                            TelegramPaymentMessage msg = message as TelegramPaymentMessage;
                            if (!string.IsNullOrEmpty(bot.PaymentsKey))
                            {
                                try
                                {
                                    int result = bot.SendInvoiceAsync(msg.ChatId, msg.Title, msg.Description, msg.InvoiceId, bot.PaymentsKey, "startParameter", "RUB", msg.Prices).Result.MessageId;
                                    bot.SetLastMessageId(msg.ChatId, result);
                                }
                                catch (Exception error)
                                {
                                    BotUtils.LogException(error);
                                }
                            }
                        }
                        break;

                    case TelegramMessageType.Typing:
                        {
                            try
                            {
                                bot.SendChatActionAsync(message.ChatId, ChatAction.Typing);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                            }
                        }
                        break;

                    case TelegramMessageType.File:
                        {
                            TelegramFileMessage msg = message as TelegramFileMessage;
                            try
                            {
                                int result = bot.SendDocumentAsync(message.ChatId, new InputOnlineFile(new MemoryStream(msg.Data), msg.FileName), msg.Caption).Result.MessageId;
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                            }
                        }
                        break;

                    case TelegramMessageType.Picture:
                        {
                            TelegramPictureMessage msg = message as TelegramPictureMessage;
                            try
                            {
                                int result = bot.SendPhotoAsync(message.ChatId, new InputOnlineFile(new MemoryStream(msg.Data)), msg.Caption).Result.MessageId;
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                            }
                        }
                        break;

                    case TelegramMessageType.TextWithKeyboardEdited:
                        {
                            TelegramTextMessageWithKeyboardEdited msg = message as TelegramTextMessageWithKeyboardEdited;
                            try
                            {
                                int result = bot.EditMessageTextAsync(msg.ChatId, msg.OriginalMessageId, msg.Text, msg.ParseMode, msg.DisableWebPagePreview, msg.Keyboard).Result.MessageId;
                                msg.SetMessageId(result);
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                            }
                        }
                        break;
                }
            }
            SendTimer.Start();
        }
    }
}