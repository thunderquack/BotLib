using BotLib.Engine.Commands;
using BotLib.Engine.Messages;
using BotLib.FSM.FSMEventArgs;
using System;

namespace BotLib.FSM
{
    public abstract class BotState
    {
        public int LastMessageId { get; set; }
        protected bool Go = false;

        protected BotState(long UserId, BotMachine Machine)
        {
            this.UserId = UserId;
            this.Machine = Machine;
        }

        public event EventHandler<TelegramMessageEventArgs> MessageGenerated;

        public event EventHandler<TelegramMessageEventArgs> NonPriorityMessageGenerated;

        public event EventHandler<NewStateEventArgs> StateIsChanged;

        public long UserId { get; }
        protected virtual BotMachine Machine { get; }

        /// <summary>
        /// This function is being called by Machine after events are re-subscribed
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Returns true if something is changed and it is required to get new state and new message
        /// </summary>
        public abstract void ProcessCommand(TelegramCommand command);

        public void SetLastMessageId(int messageId)
        {
            LastMessageId = messageId;
        }

        protected void ActivateState(BotState state)
        {
            ChangeState(new NewStateEventArgs() { BotState = state });
        }

        protected void PostMessage(TelegramMessage message, bool immediately = false)
        {
            GenerateMessage(new TelegramMessageEventArgs() { TelegramMessage = message, Immediately = immediately });
        }

        protected void PostNonPriorityMessage(TelegramMessage message)
        {
            GenerateMessage(new TelegramMessageEventArgs() { TelegramMessage = message, Immediately = false }, true);
        }

        protected void Type()
        {
            TelegramTypingMessage typingMessage = new TelegramTypingMessage(UserId);
            PostMessage(typingMessage);
        }

        private void ChangeState(NewStateEventArgs e)
        {
            EventHandler<NewStateEventArgs> handler = StateIsChanged;
            handler?.Invoke(this, e);
        }

        private void GenerateMessage(TelegramMessageEventArgs e, bool nonPriorityQueue = false)
        {
            EventHandler<TelegramMessageEventArgs> handler;
            if (nonPriorityQueue)
                handler = NonPriorityMessageGenerated;
            else
                handler = MessageGenerated;
            handler?.Invoke(this, e);
        }
    }
}