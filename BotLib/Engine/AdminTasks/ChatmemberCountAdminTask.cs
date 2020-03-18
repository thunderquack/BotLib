using System;

namespace BotLib.Engine.AdminTasks
{
    public class ChatmemberCountAdminTask : AdminTask
    {
        public ChatmemberCountAdminTask(long ChatId) : base(ChatId, AdminTaskType.ChatmemberCount)
        {
        }

        public ChatmemberCountAdminTask(long ChatId, DateTime DelayUntil) : base(ChatId, AdminTaskType.ChatmemberCount, DelayUntil)
        {
        }

        public int Count { get; private set; }

        internal void SetCount(int result)
        {
            Count = result;
            Complete = true;
        }
    }
}