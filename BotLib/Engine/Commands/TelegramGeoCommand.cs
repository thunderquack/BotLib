using BotLib.Engine.Exceptions;

namespace BotLib.Engine.Commands
{
    public class TelegramGeoCommand : TelegramCommand
    {
        public TelegramGeoCommand(int UserId, object Argument) : base(TelegramCommandType.Geo, UserId, Argument)
        {
            if (Argument.GetType() != typeof(float[])) throw new InvalidCommandArgumentException("Invalid type of command's argument", Argument.GetType());
            if (((float[])Argument).Length != 2) throw new InvalidCommandArgumentException("The length of array is not 2", typeof(float[]));
        }

        public float Latitude => ((float[])Argument)[0];
        public float Longitude => ((float[])Argument)[1];
    }
}