using System;
using Telegram.Bot.Types.Enums;

namespace BotLib.Engine.AdminTasks
{
    public class ChatmemberInfoAdminTask : AdminTask
    {
        public ChatmemberInfoAdminTask(long ChatId, int UserId) : base(ChatId, AdminTaskType.ChatmemberInfo)
        {
            this.UserId = UserId;
        }

        public ChatmemberInfoAdminTask(long ChatId, int UserId, DateTime DelayUntil) : base(ChatId, AdminTaskType.ChatmemberInfo, DelayUntil)
        {
            this.UserId = UserId;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public ChatMemberStatus Status { get; private set; }
        public int UserId { get; }
        public string Username { get; private set; }

        internal void FillUserInfo(string FirstName, string LastName, string Username, ChatMemberStatus Status)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Username = Username;
            this.Status = Status;
            Complete = true;
        }
    }
}