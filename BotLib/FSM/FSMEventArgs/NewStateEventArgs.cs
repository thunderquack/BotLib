namespace BotLib.FSM.FSMEventArgs
{
    public class NewStateEventArgs : System.EventArgs
    {
        public BotState BotState { get; set; }
    }
}