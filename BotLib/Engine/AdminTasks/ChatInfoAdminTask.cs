using System;

namespace BotLib.Engine.AdminTasks
{
    public class ChatInfoAdminTask : AdminTask
    {
        public ChatInfoAdminTask(long ChatId) : base(ChatId, AdminTaskType.ChatInfo)
        {
        }

        public ChatInfoAdminTask(long ChatId, DateTime DelayUntil) : base(ChatId, AdminTaskType.ChatInfo, DelayUntil)
        {
        }

        public string InviteLink { get; private set; }

        public string Title { get; private set; }
        public string Username { get; private set; }

        internal void SetChatInfo(string Title, string InviteLink, string Username)
        {
            this.Title = Title;
            this.InviteLink = InviteLink;
            this.Username = Username;
            Complete = true;
        }
    }
}