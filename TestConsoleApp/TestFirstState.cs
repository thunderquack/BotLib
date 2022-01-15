using BotLib.Engine.Commands;
using BotLib.Engine.Messages;
using BotLib.FSM;

namespace TestConsoleApp
{
    internal class TestFirstState : BotState
    {
        public const string Callback = "ButtonCallback";
        public int Increment = 1;
        public string MessageTemplate = "Message:\n{0}";
        private TelegramTextMessageWithKeyboard ActiveMessage;

        public TestFirstState(long UserId, BotMachine Machine) : base(UserId, Machine)
        {
        }

        public override void Init()
        {
            ActiveMessage = new TelegramTextMessageWithKeyboard(UserId, string.Format(MessageTemplate, Increment));
            ActiveMessage.AddCallbackButton("Button", Callback);
            PostMessage(ActiveMessage);
        }

        public override void ProcessCommand(TelegramCommand command)
        {
            if (command.CommandType == TelegramCommandType.ButtonPressed)
            {
                if ((command as TelegramButtonPressedCommand).CallbackData == Callback)
                {
                    if (ActiveMessage.MessageId > 0)
                    {
                        Increment++;
                        ActiveMessage = new TelegramTextMessageWithKeyboardEdited(ActiveMessage.MessageId, ActiveMessage, string.Format(MessageTemplate, Increment));
                        PostMessage(ActiveMessage);
                    }
                }
            }
            else if (command.CommandType == TelegramCommandType.Geo)
            {
                PostMessage(ActiveMessage.GetMessageToHide());
                double Latitude = (command as TelegramGeoCommand).Latitude;
                double Longitude = (command as TelegramGeoCommand).Longitude;
                TelegramTextMessage m = new TelegramTextMessage(UserId, string.Format("{0} {1}", Latitude, Longitude));
                PostMessage(m);
            }
            else
                Machine.Bot.terminate = true;
        }
    }
}