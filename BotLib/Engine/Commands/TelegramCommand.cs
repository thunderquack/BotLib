namespace BotLib.Engine.Commands
{
    public abstract class TelegramCommand
    {
        public TelegramCommandType CommandType { get; }
        public long UserId { get; }
        public object Argument { get; }

        public TelegramCommand(TelegramCommandType commandType, long UserId, object Argument)
        {
            this.CommandType = commandType;
            this.UserId = UserId;
            this.Argument = Argument;
        }
    }
}