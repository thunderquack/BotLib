namespace BotLib.Engine.Commands
{
    public class TelegramPhotoCommand : TelegramBlobCommand
    {
        public TelegramPhotoCommand(long UserId, object Argument, string caption) : base(TelegramCommandType.PhotoSent, UserId, Argument)
        {
            Caption = caption;
        }

        public string Caption { get; }
    }
}