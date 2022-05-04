using System;

namespace BotLib.Engine.AdminTasks
{
    public class ChatmemberKickAdminTask : AdminTask
    {
        public ChatmemberKickAdminTask(long ChatId, long UserId) : base(ChatId, AdminTaskType.ChatmemberKick)
        {
            this.UserId = UserId;
        }

        public ChatmemberKickAdminTask(long ChatId, long UserId, DateTime DelayUntil) : base(ChatId, AdminTaskType.ChatmemberKick, DelayUntil)
        {
            this.UserId = UserId;
        }

        public long UserId { get; }
    }
}