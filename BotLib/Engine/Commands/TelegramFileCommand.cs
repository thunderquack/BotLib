namespace BotLib.Engine.Commands
{
    public class TelegramFileCommand : TelegramBlobCommand
    {
        public string FileName { get; }

        public TelegramFileCommand(long UserId, object Argument, string FileName) : base(TelegramCommandType.FileSent, UserId, Argument)
        {
            this.FileName = FileName;
        }
    }
}