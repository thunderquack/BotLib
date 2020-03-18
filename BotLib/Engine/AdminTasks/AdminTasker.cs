using BotLib.FSM.FSMEventArgs;
using System;
using System.Collections.Generic;
using System.Timers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotLib.Engine.AdminTasks
{
    public class AdminTasker
    {
        private Queue<AdminTask> Tasks = new Queue<AdminTask>();

        private Timer TasksTimer = new Timer(500);

        public AdminTasker(TelegramFSMBot Bot)
        {
            this.Bot = Bot;
            TasksTimer.Elapsed += AdminTaskerTimerElapsed;
            TasksTimer.Start();
        }

        public event EventHandler<AdminTaskCompleteEventArgs> AdminTaskComplete;

        public TelegramFSMBot Bot { get; }

        public void Enqueue(AdminTask task)
        {
            Tasks.Enqueue(task);
        }

        private void AdminTaskerTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TasksTimer.Stop();
            if (Tasks.Count > 0)
            {
                AdminTask task = Tasks.Dequeue();
                if (task.DelayUntil < DateTime.UtcNow)
                    PerformTask(task);
                else
                    Enqueue(task);
            }
            TasksTimer.Start();
        }

        private void PerformTask(AdminTask task)
        {
            try
            {
                switch (task.TaskType)
                {
                    case AdminTaskType.ChatlinkChange:
                        (task as ChangeChatLinkAdminTask).SetChatLink(Bot.ExportChatInviteLinkAsync(task.ChatId).Result);
                        break;

                    case AdminTaskType.ChatmemberCount:
                        (task as ChatmemberCountAdminTask).SetCount(Bot.GetChatMembersCountAsync(task.ChatId).Result);
                        break;

                    case AdminTaskType.ChatmemberInfo:
                        {
                            var cm = Bot.GetChatMemberAsync(task.ChatId, (task as ChatmemberInfoAdminTask).UserId).Result;
                            (task as ChatmemberInfoAdminTask).FillUserInfo(cm.User.FirstName, cm.User.LastName, cm.User.Username, cm.Status);
                        }
                        break;

                    case AdminTaskType.ChatmemberKick:
                        Bot.KickChatMemberAsync(task.ChatId, (task as ChatmemberKickAdminTask).UserId);
                        break;

                    case AdminTaskType.ChatmemberStatus:
                        {
                            var cm = Bot.GetChatMemberAsync(task.ChatId, (task as ChatmemberMembershipAdminTask).UserId).Result;
                            if (cm.Status == ChatMemberStatus.Administrator ||
                                cm.Status == ChatMemberStatus.Creator ||
                                cm.Status == ChatMemberStatus.Member ||
                                (cm.Status == ChatMemberStatus.Restricted && cm.IsMember.Value))
                                (task as ChatmemberMembershipAdminTask).SetMember();
                        }
                        break;

                    case AdminTaskType.UnrestrictChatmember:
                        Bot.UnbanChatMemberAsync(task.ChatId, (task as UnrestrictChatmemberAdminTask).UserId);
                        break;

                    case AdminTaskType.ChatInfo:
                        {
                            Chat c = Bot.GetChatAsync(task.ChatId).Result;
                            (task as ChatInfoAdminTask).SetChatInfo(c.Title, c.InviteLink, c.Username);
                        }
                        break;

                    case AdminTaskType.GetChatAdmins:
                        {
                            ChatMember[] Members = Bot.GetChatAdministratorsAsync(task.ChatId).Result;
                            List<int> AdminList = new List<int>();
                            foreach (ChatMember member in Members)
                            {
                                AdminList.Add(member.User.Id);
                            }
                            (task as GetChatAdminsAdminTask).SetAdmins(AdminList);
                        }
                        break;
                }
                task.SetComplete();
            }
            catch
            (Exception err)
            {
                task.SetError();
                BotUtils.LogException(err);
            }
            {
                EventHandler<AdminTaskCompleteEventArgs> handler = AdminTaskComplete;
                handler.Invoke(this, new AdminTaskCompleteEventArgs(task));
            }
        }
    }
}