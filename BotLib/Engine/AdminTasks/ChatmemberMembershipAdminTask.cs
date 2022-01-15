using System;

namespace BotLib.Engine.AdminTasks
{
    public class ChatmemberMembershipAdminTask : AdminTask
    {
        public ChatmemberMembershipAdminTask(long ChatId, long UserId) : base(ChatId, AdminTaskType.ChatmemberStatus)
        {
            this.UserId = UserId;
        }

        public ChatmemberMembershipAdminTask(long ChatId, long UserId, DateTime DelayUntil) : base(ChatId, AdminTaskType.ChatmemberStatus, DelayUntil)
        {
            this.UserId = UserId;
        }

        public bool Membership { get; private set; } = false;
        public long UserId { get; }

        internal void SetMember()
        {
            Membership = true;
            Complete = true;
        }
    }
}