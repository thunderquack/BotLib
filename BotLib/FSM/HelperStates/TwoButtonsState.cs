using BotLib.Engine.Messages;
using Telegram.Bot.Types.Enums;

namespace BotLib.FSM.HelperStates
{
    public abstract class TwoButtonsState : BotState
    {
        protected abstract string ButtonAText { get; }
        protected abstract string ButtonBText { get; }
        protected abstract string ButtonACommand { get; }
        protected abstract string ButtonBCommand { get; }
        protected abstract string Message { get; }
        protected abstract ParseMode MessageParseMode {get;}
        protected abstract bool DisableWebPreview { get; }
        private TelegramTextMessageWithKeyboard ActiveMessage;
        protected TwoButtonsState(int UserId, BotMachine Machine) : base(UserId, Machine)
        {
           
        }
        public override void Init()
        {
            ActiveMessage = new TelegramTextMessageWithKeyboard(UserId, Message, MessageParseMode, DisableWebPreview);
            ActiveMessage.AddCallbackButton(ButtonAText, ButtonACommand);
            ActiveMessage.AddCallbackButton(ButtonBText, ButtonBCommand);
            PostMessage(ActiveMessage);
        }

    }
}
