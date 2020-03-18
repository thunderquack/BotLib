using System;

namespace BotLib.Engine.AdminTasks
{
    public class ChangeChatLinkAdminTask : AdminTask
    {
        public ChangeChatLinkAdminTask(long ChatId) : base(ChatId, AdminTaskType.ChatlinkChange)
        {
        }

        public ChangeChatLinkAdminTask(long ChatId, DateTime DelayUntil) : base(ChatId, AdminTaskType.ChatlinkChange, DelayUntil)
        {
        }

        public string ChatLink { get; private set; }

        public void SetChatLink(string Link)
        {
            ChatLink = Link;
            Complete = true;
        }
    }
}