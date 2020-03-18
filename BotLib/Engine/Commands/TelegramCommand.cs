namespace BotLib.Engine.Commands
{
    public abstract class TelegramCommand
    {
        public TelegramCommandType CommandType { get; }
        public int UserId { get; }
        public object Argument { get; }

        public TelegramCommand(TelegramCommandType commandType, int UserId, object Argument)
        {
            this.CommandType = commandType;
            this.UserId = UserId;
            this.Argument = Argument;
        }
    }
}