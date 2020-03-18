using BotLib.Engine.Commands;
using BotLib.Engine.Messages;
using BotLib.FSM.FSMEventArgs;
using System;

namespace BotLib.FSM
{
    public abstract class BotState
    {
        public int LastMessageId;
        protected bool Go = false;
        protected BotMachine Machine;

        public BotState(int UserId, BotMachine Machine)
        {
            this.UserId = UserId;
            this.Machine = Machine;
        }

        public event EventHandler<TelegramMessageEventArgs> MessageGenerated;

        public event EventHandler<NewStateEventArgs> StateIsChanged;

        public int UserId { get; }

        /// <summary>
        /// This function is being called by Machine after events are re-subscribed
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Returns true if something is changed and it is required to get new state and new message
        /// </summary>
        public abstract void ProcessCommand(TelegramCommand Command);

        public void SetLastMessageId(int MessageId)
        {
            this.LastMessageId = MessageId;
        }

        protected void ActivateState(BotState state)
        {
            ChangeState(new NewStateEventArgs() { BotState = state });
        }

        protected void PostMessage(TelegramMessage message)
        {
            GenerateMessage(new TelegramMessageEventArgs() { TelegramMessage = message });
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

        private void GenerateMessage(TelegramMessageEventArgs e)
        {
            EventHandler<TelegramMessageEventArgs> handler = MessageGenerated;
            handler?.Invoke(this, e);
        }
    }
}