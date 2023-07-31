using System;
using System.Collections.Generic;
using Telegram.Bot.Types.Payments;

namespace BotLib.Engine.Messages
{
    public class TelegramPaymentMessage : TelegramMessage
    {
        public List<LabeledPrice> Prices { get; }
        public string InvoiceId { get; }
        public string Title { get; }
        public string Description { get; }
        public new int ChatId { get; }

        public TelegramPaymentMessage(long ChatId, string Title, string Description, List<LabeledPrice> Prices, string InvoiceId) : base(ChatId)
        {
            this.Prices = Prices;
            this.InvoiceId = InvoiceId;
            this.Title = Title;
            this.Description = Description;
            this.ChatId = Convert.ToInt32(ChatId);
        }

        protected override void SetMessageType()
        {
            this.MessageType = TelegramMessageType.Payment;
        }
    }
}