using System;

namespace BotLib.Engine.AdminTasks
{
    public class ChatmemberKickAdminTask : AdminTask
    {
        public ChatmemberKickAdminTask(long ChatId, int UserId) : base(ChatId, AdminTaskType.ChatmemberKick)
        {
            this.UserId = UserId;
        }

        public ChatmemberKickAdminTask(long ChatId, int UserId, DateTime DelayUntil) : base(ChatId, AdminTaskType.ChatmemberKick, DelayUntil)
        {
            this.UserId = UserId;
        }

        public int UserId { get; }
    }
}