namespace BotLib.Engine.Commands
{
    internal class TelegramEmptyCommand : TelegramCommand
    {
        public TelegramEmptyCommand(long UserId) : base(TelegramCommandType.Empty, UserId, null)
        {
        }
    }
}