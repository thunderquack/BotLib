namespace BotLib.Engine.Messages
{
    public class TelegramPictureMessage : TelegramMessage
    {
        public TelegramPictureMessage(long ChatId, byte[] Data, string Caption = null) : base(ChatId)
        {
            this.Data = Data;
            this.Caption = Caption;
        }

        public string Caption { get; }
        public byte[] Data { get; }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.Picture;
        }
    }
}