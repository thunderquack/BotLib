namespace BotLib.Engine.Commands
{
    public class TelegramPhotoCommand : TelegramBlobCommand
    {
        public TelegramPhotoCommand(int UserId, object Argument) : base(TelegramCommandType.PhotoSent, UserId, Argument)
        {
        }
    }
}