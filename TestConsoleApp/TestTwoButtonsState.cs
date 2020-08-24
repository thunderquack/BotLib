using BotLib.Engine.Commands;
using BotLib.FSM;
using BotLib.FSM.HelperStates;
using Telegram.Bot.Types.Enums;

namespace TestConsoleApp
{
    class TestTwoButtonsState : TwoButtonsState
    {
        public TestTwoButtonsState(int UserId, BotMachine Machine) : base(UserId, Machine)
        {
        }

        protected override string ButtonAText => "Yes";

        protected override string ButtonBText => "No";

        protected override string ButtonACommand => "CommandA";

        protected override string ButtonBCommand => "CommandB";

        protected override string Message => "Would you like to continue?";

        protected override ParseMode MessageParseMode => ParseMode.Default;

        protected override bool DisableWebPreview => false;

        public override void ProcessCommand(TelegramCommand command)
        {
            Machine.Bot.Terminate = true;
        }
    }
}
