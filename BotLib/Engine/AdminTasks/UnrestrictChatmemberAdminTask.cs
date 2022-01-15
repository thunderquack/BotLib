using System;

namespace BotLib.Engine.AdminTasks
{
    public class UnrestrictChatmemberAdminTask : AdminTask
    {
        public UnrestrictChatmemberAdminTask(long ChatId, long UserId) : base(ChatId, AdminTaskType.UnrestrictChatmember)
        {
            this.UserId = UserId;
        }

        public UnrestrictChatmemberAdminTask(long ChatId, long UserId, DateTime DelayUntil) : base(ChatId, AdminTaskType.UnrestrictChatmember, DelayUntil)
        {
            this.UserId = UserId;
        }

        public long UserId { get; }
    }
}