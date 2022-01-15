using BotLib.Engine.Commands;
using BotLib.Engine.Messages;
using System;
using System.Collections.Generic;

namespace BotLib.FSM.HelperStates
{
    public abstract class MultipleCheckBoxButtonsState : MultipleButtonsState
    {
        private const string CHECKED_BULLET_SYMBOL = "▪️";
        private const string UNCHECKED_BULLET_SYMBOL = "▫️";

        private List<string> CheckedOptions;

        private string MainButtonCommand;
        private Random Random = new Random();

        public MultipleCheckBoxButtonsState(long UserId, BotMachine Machine) : base(UserId, Machine)
        {
            CheckedOptions = new List<string>();
            GenerateMainButtonCommand();
        }

        protected abstract string MainButton { get; }

        public override void Init()
        {
            Type();
            ActiveMessage = new TelegramTextMessageWithKeyboard(UserId, Message, MessageParseMode, DisableWebPreview);
            foreach (var pair in ButtonList)
            {
                ActiveMessage.AddCallbackButton(GetUncheckedString(pair.Key), pair.Value);
            }
            ActiveMessage.AddCallbackButton(MainButton, MainButtonCommand, true);
            PostMessage(ActiveMessage);
        }

        public override sealed void ProcessCommand(TelegramCommand command)
        {
            if (command.CommandType == TelegramCommandType.ButtonPressed)
            {
                var c = command as TelegramButtonPressedCommand;
                if (c.CallbackData == MainButtonCommand)
                {
                    Done(CheckedOptions);
                    return;
                }
                ActiveMessage = new TelegramTextMessageWithKeyboardEdited(LastMessageId, ActiveMessage, clearKeyboard: true);
                if (CheckedOptions.Contains(c.CallbackData))
                    CheckedOptions.Remove(c.CallbackData);
                else
                    CheckedOptions.Add(c.CallbackData);
                foreach (var pair in ButtonList)
                    if (CheckedOptions.Contains(pair.Value))
                        ActiveMessage.AddCallbackButton(GetCheckedString(pair.Key), pair.Value);
                    else
                        ActiveMessage.AddCallbackButton(GetUncheckedString(pair.Key), pair.Value);
                GenerateMainButtonCommand();
                ActiveMessage.AddCallbackButton(MainButton, MainButtonCommand, true);
                PostMessage(ActiveMessage);
            }
        }

        /// <summary>
        /// Here should be specified a move to an another state or some behaviour
        /// </summary>
        /// <param name="chosenValues">List of the chosen values, can be empty</param>
        protected abstract void Done(List<string> chosenValues);

        private void GenerateMainButtonCommand()
        {
            MainButtonCommand = Random.Next().ToString() + "_" + Random.Next().ToString();
        }

        private string GetCheckedString(string text) => string.Format("{0} {1}", CHECKED_BULLET_SYMBOL, text);

        private string GetUncheckedString(string text) => string.Format("{0} {1}", UNCHECKED_BULLET_SYMBOL, text);
    }
}