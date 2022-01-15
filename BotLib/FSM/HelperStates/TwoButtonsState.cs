using System.Collections.Generic;

namespace BotLib.FSM.HelperStates
{
    public abstract class TwoButtonsState : MultipleButtonsState
    {
        protected TwoButtonsState(long UserId, BotMachine Machine) : base(UserId, Machine)
        {
        }

        protected abstract string ButtonACommand { get; }

        protected abstract string ButtonAText { get; }

        protected abstract string ButtonBCommand { get; }

        protected abstract string ButtonBText { get; }

        protected override sealed Dictionary<string, string> ButtonList
        {
            get
            {
                Dictionary<string, string> bl = new Dictionary<string, string>();
                bl.Add(ButtonAText, ButtonACommand);
                bl.Add(ButtonBText, ButtonBCommand);
                return bl;
            }
        }
    }
}