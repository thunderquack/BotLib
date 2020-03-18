namespace BotLib.Engine.Commands
{
    internal class TelegramEmptyCommand : TelegramCommand
    {
        public TelegramEmptyCommand(int UserId) : base(TelegramCommandType.Empty, UserId, null)
        {
        }
    }
}