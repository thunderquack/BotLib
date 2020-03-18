using BotLib.Engine.AdminTasks;
using System;

namespace BotLib.FSM.FSMEventArgs
{
    public class AdminTaskCompleteEventArgs : EventArgs
    {
        public AdminTaskCompleteEventArgs(AdminTask task)
        {
            this.AdminTask = task;
        }

        public AdminTask AdminTask { get; }
    }
}