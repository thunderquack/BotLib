using BotLib.Engine.Exceptions;

namespace BotLib.Engine.Commands
{
    public class TelegramGeoCommand : TelegramCommand
    {
        public TelegramGeoCommand(long UserId, object Argument) : base(TelegramCommandType.Geo, UserId, Argument)
        {
            if (Argument.GetType() != typeof(double[])) throw new InvalidCommandArgumentException("Invalid type of command's argument", Argument.GetType());
            if (((double[])Argument).Length != 2) throw new InvalidCommandArgumentException("The length of array is not 2", typeof(double[]));
        }

        public double Latitude => ((double[])Argument)[0];
        public double Longitude => ((double[])Argument)[1];
    }
}