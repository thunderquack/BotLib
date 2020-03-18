namespace BotLib.Engine.Messages
{
    public class TelegramFileMessage : TelegramMessage
    {
        public TelegramFileMessage(long ChatId, string FileName, byte[] Data, string Caption = null) : base(ChatId)
        {
            this.FileName = FileName;
            this.Data = Data;
            this.Caption = Caption;
        }

        public string Caption { get; }
        public byte[] Data { get; }
        public string FileName { get; }

        protected override void SetMessageType()
        {
            MessageType = TelegramMessageType.File;
        }
    }
}