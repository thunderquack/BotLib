namespace BotLib.FSM.FSMEventArgs
{
    public class PaymentEventArgs : System.EventArgs
    {
        public PaymentEventArgs(string InvoiceId)
        {
            this.InvoiceId = InvoiceId;
        }

        public string InvoiceId { get; private set; }
    }
}