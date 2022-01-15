namespace BotLib.Engine.Commands
{
    public class TelegramPhotoCommand : TelegramBlobCommand
    {
        public TelegramPhotoCommand(long UserId, object Argument) : base(TelegramCommandType.PhotoSent, UserId, Argument)
        {
        }
    }
}