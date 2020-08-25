using BotLib.Engine.Messages;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;

namespace BotLib.FSM.HelperStates
{
    public abstract class MultipleButtonsState : BotState
    {
        protected TelegramTextMessageWithKeyboard ActiveMessage;

        public MultipleButtonsState(int UserId, BotMachine Machine) : base(UserId, Machine)
        {
        }

        protected abstract Dictionary<string, string> ButtonList { get; }
        protected abstract bool DisableWebPreview { get; }
        protected abstract string Message { get; }
        protected abstract ParseMode MessageParseMode { get; }

        public override void Init()
        {
            Type();
            ActiveMessage = new TelegramTextMessageWithKeyboard(UserId, Message, MessageParseMode, DisableWebPreview);
            foreach (var pair in ButtonList)
            {
                ActiveMessage.AddCallbackButton(pair.Key, pair.Value);
            }
            PostMessage(ActiveMessage);
        }
    }
}