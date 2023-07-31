using BotLib.Engine.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotLib.Engine
{
    public class TelegramMessageSender
    {
        private const int SENDING_INTERVAL = 50;
        private readonly TelegramFSMBot bot;
        private readonly LinkedList<TelegramMessage> queue;
        private readonly Timer sendTimer;

        /// <summary>
        /// Creates an instance of Sender
        /// </summary>
        /// <param name="bot">Reference to the bot</param>
        public TelegramMessageSender(TelegramFSMBot bot, bool logMessage = false)
        {
            this.bot = bot;
            queue = new LinkedList<TelegramMessage>();
            sendTimer = new Timer(SENDING_INTERVAL);
            sendTimer.Elapsed += TimeToSend;
            LoggingEnabled = logMessage;
            sendTimer.Start();
        }

        /// <summary>
        /// Creates an instance of Sender
        /// </summary>
        /// <param name="bot">Reference to the bot</param>
        /// <param name="SendingInterval">Interval of sending in milliseconds</param>
        public TelegramMessageSender(TelegramFSMBot bot, int SendingInterval = 50, bool logMessage = false)
        {
            this.bot = bot;
            queue = new LinkedList<TelegramMessage>();
            sendTimer = new Timer(SendingInterval);
            sendTimer.Elapsed += TimeToSend;
            LoggingEnabled = logMessage;
            sendTimer.Start();
        }

        public double Interval
        {
            get
            {
                return sendTimer.Interval;
            }
        }

        public bool LoggingEnabled { get; set; } = false;

        public void Add(TelegramMessage telegramMessage)
        {
            queue.AddFirst(telegramMessage);
        }

        public void Enqueue(TelegramMessage telegramMessage)
        {
            queue.AddLast(telegramMessage);
        }

        public void SetNewInterval(double interval)
        {
            sendTimer.Interval = interval;
        }

        private void TimeToSend(object sender, ElapsedEventArgs e)
        {
            sendTimer.Stop();
            if (queue.Count > 0)
            {
                TelegramMessage message;
                message = queue.First.Value;
                queue.RemoveFirst();
                if (LoggingEnabled)
                    try
                    {
                        Console.WriteLine(DateTime.UtcNow);
                        Console.WriteLine(message.ToString());
                    }
                    catch (Exception err)
                    {
                        BotUtils.LogException(err);
                    }
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
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, parseMode: msg.ParseMode, disableWebPagePreview: msg.DisableWebPagePreview).Result.MessageId;
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                                try
                                {
                                    result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, parseMode: ParseMode.Markdown).Result.MessageId;
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
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, parseMode: msg.ParseMode, replyMarkup: msg.ReplyMarkup, disableWebPagePreview: msg.DisableWebPagePreview).Result.MessageId;
                                msg.SetMessageId(result);
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                                try
                                {
                                    result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, parseMode: ParseMode.Markdown, replyMarkup: msg.ReplyMarkup, disableWebPagePreview: msg.DisableWebPagePreview).Result.MessageId;
                                    msg.SetMessageId(result);
                                    bot.SetLastMessageId(msg.ChatId, result);
                                }
                                catch (Exception internalError)
                                {
                                    BotUtils.LogException(internalError);
                                }
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
                                result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, parseMode: msg.ParseMode, replyMarkup: msg.ReplyMarkup).Result.MessageId;
                                bot.SetLastMessageId(msg.ChatId, result);
                            }
                            catch (Exception error)
                            {
                                BotUtils.LogException(error);
                                try
                                {
                                    result = bot.SendTextMessageAsync(msg.ChatId, msg.Text, parseMode: ParseMode.Markdown, replyMarkup: msg.ReplyMarkup).Result.MessageId;
                                    bot.SetLastMessageId(msg.ChatId, result);
                                }
                                catch (Exception internalError)
                                {
                                    BotUtils.LogException(internalError);
                                }
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
                                    int result = bot.SendInvoiceAsync(msg.ChatId, msg.Title, msg.Description, msg.InvoiceId, bot.PaymentsKey, "RUB", msg.Prices).Result.MessageId;
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
                                int result = bot.SendDocumentAsync(message.ChatId, new InputFileStream(new MemoryStream(msg.Data), msg.FileName), caption: msg.Caption).Result.MessageId;
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
                                int result = bot.SendPhotoAsync(message.ChatId, new InputFileStream(new MemoryStream(msg.Data)), caption: msg.Caption).Result.MessageId;
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
                                int result = bot.EditMessageTextAsync(msg.ChatId, msg.OriginalMessageId, msg.Text, msg.ParseMode, disableWebPagePreview: msg.DisableWebPagePreview, replyMarkup: msg.Keyboard).Result.MessageId;
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
            sendTimer.Start();
        }
    }
}