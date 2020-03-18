using System;

namespace BotLib.Engine.AdminTasks
{
    public abstract class AdminTask
    {
        private int Id;

        public AdminTask(long ChatId, AdminTaskType TaskType)
        {
            this.ChatId = ChatId;
            this.TaskType = TaskType;
            this.DelayUntil = DateTime.MinValue;
            SetId();
        }

        public AdminTask(long ChatId, AdminTaskType TaskType, DateTime DelayUntil)
        {
            this.ChatId = ChatId;
            this.TaskType = TaskType;
            this.DelayUntil = DelayUntil;
            SetId();
        }

        public long ChatId { get; }

        public bool Complete { get; protected set; } = false;

        public DateTime DelayUntil { get; }

        public bool Error { get; private set; } = false;

        public AdminTaskType TaskType { get; }

        public void SetError()
        {
            Error = true;
        }

        internal void SetComplete()
        {
            Complete = true;
        }

        private void SetId()
        {
            Random r = new Random();
            Id = r.Next();
        }
    }
}