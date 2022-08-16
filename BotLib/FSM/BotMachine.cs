using BotLib.Engine;
using BotLib.Engine.Commands;
using BotLib.FSM.FSMEventArgs;
using System;

namespace BotLib.FSM
{
    public abstract class BotMachine
    {
        private readonly Type InitStateType;

        protected BotMachine(long UserId, TelegramMessageSender sender, Type InitStateType, TelegramFSMBot Bot)
        {
            this.UserId = UserId;
            this.Sender = sender;
            this.InitStateType = InitStateType;
            this.Bot = Bot;
            BotState state = Activator.CreateInstance(InitStateType, UserId, this) as BotState;
            ActiveState = state;
            SubscribeActiveStateEvents();
        }

        public event EventHandler<ParametricStartEventArgs> ParametricStartReceived;

        public BotState ActiveState { get; private set; }
        public virtual TelegramFSMBot Bot { get; }
        public string BotMe => Bot.me;
        public int LastMessageId { get; private set; }

        public TelegramMessageSender Sender { get; }

        public long UserId { get; }

        public void Die()
        {
            Bot.KillMachine(UserId);
        }

        public void ProcessCommand(TelegramCommand command)
        {
            ActiveState.ProcessCommand(command);
        }

        internal void ReceiveParametricStart(ParametricStartEventArgs e)
        {
            EventHandler<ParametricStartEventArgs> handler = ParametricStartReceived;
            handler?.Invoke(this, e);
        }

        internal void SetLastMessageId(int MessageId)
        {
            this.LastMessageId = MessageId;
            ActiveState.SetLastMessageId(MessageId);
        }

        private void MessageGeneratedEvent(object sender, TelegramMessageEventArgs e)
        {
            if (e.Immediately)
                Sender.Add(e.TelegramMessage);
            else
                Sender.Enqueue(e.TelegramMessage);
        }

        private void NonPriorityMessageGeneratedEvent(object sender, TelegramMessageEventArgs e)
        {
            Bot.NonPriorityMessageSender.Enqueue(e.TelegramMessage);
        }

        private void StateIsChanged(object sender, NewStateEventArgs e)
        {
            this.ActiveState = e.BotState;
            SubscribeActiveStateEvents();
        }

        private void SubscribeActiveStateEvents()
        {
            ActiveState.MessageGenerated += MessageGeneratedEvent;
            ActiveState.StateIsChanged += StateIsChanged;
            ActiveState.NonPriorityMessageGenerated += NonPriorityMessageGeneratedEvent;
            ActiveState.Init();
        }
    }
}