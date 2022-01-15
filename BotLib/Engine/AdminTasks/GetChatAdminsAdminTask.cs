using System;
using System.Collections.Generic;
using System.Text;

namespace BotLib.Engine.AdminTasks
{
    public class GetChatAdminsAdminTask : AdminTask
    {
        public GetChatAdminsAdminTask(long ChatId) : base(ChatId, AdminTaskType.GetChatAdmins)
        {
        }

        public GetChatAdminsAdminTask(long ChatId, DateTime DelayUntil) : base(ChatId, AdminTaskType.GetChatAdmins, DelayUntil)
        {
        }

        public List<long> Admins { get; private set; }

        internal void SetAdmins(List<long> Admins)
        {
            this.Admins = Admins;
            SetComplete();
        }
    }
}