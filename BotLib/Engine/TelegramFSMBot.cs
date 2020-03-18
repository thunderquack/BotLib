using BotLib.Engine.AdminTasks;
using BotLib.Engine.Commands;
using BotLib.Engine.Exceptions;
using BotLib.Engine.Messages;
using BotLib.FSM;
using BotLib.FSM.FSMEventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace BotLib.Engine
{
    public abstract class TelegramFSMBot : TelegramBotClient
    {
        public string Me;
        public bool Terminate = false;
        protected AdminTasker AdminTasker;
        protected Dictionary<long, int> LastMessageIds;
        private const string CONFIG_FILE = "botconfig.json";
        private const string PARAMETRIC_COMMAND_PALETTE = "\\/start (.*)";
        private Type BotMachineType;
        private FSMBotConfig FSMConfig = new FSMBotConfig(CONFIG_FILE);
        private Type InitStateType;
        private Dictionary<int, BotMachine> Machines;
        private Type ParametricInitStateType;
        private TelegramMessageSender Sender;

        public TelegramFSMBot(string token, HttpClient httpClient = null) : base(token, httpClient)
        {
#if DEBUG
            DebugMode = true;
#endif
            Sender = new TelegramMessageSender(this);
            Machines = new Dictionary<int, BotMachine>();
            LastMessageIds = new Dictionary<long, int>();
            AdminTasker = new AdminTasker(this);
            SetInitStateType();
            SetParametricInitStateType();
            SetBotMachineType();
            CheckBotMachineType();
            CheckInitStateType();
            Init();
            OnMessage += TelegramMessage;
            OnCallbackQuery += TelegramCallbackQuery;
            OnUpdate += TelegramUpdate;
        }

        public delegate void MessagePostedDelegate(long ChatId, int MessageId);

        public event EventHandler<ChatMembersAddedEventArgs> ChatMembersAdded;

        public event EventHandler<FileTooBigEventArgs> FileTooBig;

        public event MessagePostedDelegate MessagePosted;

        public event EventHandler<ParametricStartEventArgs> ParametricStartReceived;

        public event EventHandler<PaymentEventArgs> PaymentReceived;

        public bool AnswerToFilesWithType { get; protected set; } = false;
        public bool DebugMode { get; } = false;

        public string PaymentsKey { get; private set; }

        public BotMachine CreateMachine(int UserId)
        {
            KillMachine(UserId);
            BotMachine machine = Activator.CreateInstance(BotMachineType, UserId, Sender, InitStateType, this) as BotMachine;
            Machines.Add(UserId, machine);
            return machine;
        }

        public BotMachine CreateMachine(int UserId, Type InitialStateType)
        {
            KillMachine(UserId);
            BotMachine machine = Activator.CreateInstance(BotMachineType, UserId, Sender, InitialStateType, this) as BotMachine;
            Machines.Add(UserId, machine);
            return machine;
        }

        public void KillMachine(int UserId)
        {
            if (MachineExists(UserId))
                Machines.Remove(UserId);
        }

        public abstract bool PerformPreCheckoutQuery(string InvoiceId, ref string OopsAnser);

        public abstract void SetBotMachineType();

        public abstract void SetInitStateType();

        public abstract void SetParametricInitStateType();

        internal void SetLastMessageId(long ChatId, int MessageId)
        {
            if (LastMessageIds.ContainsKey(ChatId))
                LastMessageIds[ChatId] = MessageId;
            else
                LastMessageIds.Add(ChatId, MessageId);
            try
            {
                if (Machines.ContainsKey(Convert.ToInt32(ChatId)))
                    Machines[Convert.ToInt32(ChatId)].SetLastMessageId(MessageId);
            }
            catch
            { }
            MessagePosted?.Invoke(ChatId, MessageId);
        }

        protected abstract bool IsException(TelegramCommand command);

        protected void SendMessageDirectly(TelegramMessage Message)
        {
            Sender.Enqueue(Message);
        }

        protected void SetBotMachineType(Type type)
        {
            BotMachineType = type;
        }

        protected void SetInitStateType(Type type)
        {
            InitStateType = type;
        }

        protected void SetParametricInitStateType(Type type)
        {
            ParametricInitStateType = type;
        }

        protected void SetPaymentsKey(string PaymentsKey)
        {
            this.PaymentsKey = PaymentsKey;
        }

        private void CheckBotMachineType()
        {
            if (!BotMachineType.IsSubclassOf(typeof(BotMachine))) throw new InvalidBotMachineTypeException("Invalid BotMachine type", BotMachineType);
        }

        private void CheckInitStateType()
        {
            if (!InitStateType.IsSubclassOf(typeof(BotState))) throw new InvalidBotStateTypeException("Invalid BotState type", InitStateType);
        }

        private void Dispatch(TelegramCommand command)
        {
            if (!IsException(command))
            {
                if (MachineExists(command.UserId))
                {
                    if (command.CommandType == TelegramCommandType.TextEntered && (command as TelegramTextCommand).Text.ToLower() == "/start")
                    {
                        KillMachine(command.UserId);
                        CreateMachine(command.UserId);
                    }
                    else
                        Machines[command.UserId].ProcessCommand(command);
                }
                else
                {
                    CreateMachine(command.UserId);
                }
            }
        }

        private void DispatchParametricCommand(ParametricStartEventArgs parametricStartEvent)
        {
            CreateMachine(parametricStartEvent.UserId, ParametricInitStateType);
            Machines[parametricStartEvent.UserId].ReceiveParametricStart(parametricStartEvent);
        }

        private void Init()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            Me = GetMeAsync().Result.Username;
            Console.WriteLine(string.Format("Bot {0}, BotLib v.{1} is starting, previous start time is {2}", Me, version, FSMConfig.StartTime));
            FSMConfig.StartTime = DateTime.UtcNow;
        }

        private bool MachineExists(int UserId)
        {
            return Machines.ContainsKey(UserId);
        }

        private void ReceiveParametricStart(ParametricStartEventArgs e)
        {
            EventHandler<ParametricStartEventArgs> handler = ParametricStartReceived;
            handler?.Invoke(this, e);
            DispatchParametricCommand(e);
        }

        private void TelegramCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            int UserId = e.CallbackQuery.From.Id;
            string CallbackData = e.CallbackQuery.Data;
            TelegramButtonPressedCommand command = new TelegramButtonPressedCommand(UserId, CallbackData);
            AnswerCallbackQueryAsync(e.CallbackQuery.Id);
            Dispatch(command);
        }

        private void TelegramMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                if (Regex.IsMatch(e.Message.Text, PARAMETRIC_COMMAND_PALETTE))
                {
                    string ParametricCommand = Regex.Match(e.Message.Text, PARAMETRIC_COMMAND_PALETTE).Groups[1].Value;
                    ReceiveParametricStart(new ParametricStartEventArgs() { ParametricCommand = ParametricCommand, UserId = e.Message.From.Id });
                    return;
                }
                TelegramTextCommand command = new TelegramTextCommand(e.Message.From.Id, e.Message.Text);
                Dispatch(command);
            }
            if (e.Message.Type == MessageType.Photo || e.Message.Type == MessageType.Document)
            {
                MemoryStream ms = new MemoryStream();
                string FileName = null;
                TelegramCommand command = null;
                switch (e.Message.Type)
                {
                    case MessageType.Photo:
                        GetInfoAndDownloadFileAsync(e.Message.Photo.Last().FileId, ms).Wait();
                        ms.Seek(0, SeekOrigin.Begin);
                        command = new TelegramPhotoCommand(e.Message.From.Id, ms.ToArray());
                        break;

                    case MessageType.Document:
                        if (e.Message.Document.FileSize < 19922944)
                        {
                            if (AnswerToFilesWithType) Sender.Enqueue(new TelegramTypingMessage(e.Message.From.Id));
                            GetInfoAndDownloadFileAsync(e.Message.Document.FileId, ms).Wait();
                            ms.Seek(0, SeekOrigin.Begin);
                            FileName = e.Message.Document.FileName;
                            command = new TelegramFileCommand(e.Message.From.Id, ms.ToArray(), FileName);
                        }
                        else
                        {
                            FileTooBigEventArgs ftbea = new FileTooBigEventArgs(e.Message.Chat.Id, e.Message.From.Id, e.Message.MessageId, e.Message.Document.FileName, e.Message.Document.FileId, e.Message.Document.FileSize);
                            EventHandler<FileTooBigEventArgs> handler = FileTooBig;
                            handler.Invoke(this, ftbea);
                            command = new TelegramEmptyCommand(e.Message.From.Id);
                        }
                        break;
                }
                Dispatch(command);
            }
            if (e.Message.Type == MessageType.SuccessfulPayment)
            {
                string InvoiceId = e.Message.SuccessfulPayment.InvoicePayload;
                PaymentEventArgs pea = new PaymentEventArgs(InvoiceId);
                EventHandler<PaymentEventArgs> handler = PaymentReceived;
                handler.Invoke(this, pea);
            }
            if (e.Message.Type == MessageType.ChatMembersAdded)
            {
                Console.WriteLine(e.Message.NewChatMembers);
            }
        }

        private void TelegramUpdate(object sender, UpdateEventArgs e)
        {
            if (e.Update.Type != UpdateType.Message && e.Update.Type != UpdateType.CallbackQuery)
            {
                if (e.Update.Type == UpdateType.PreCheckoutQuery)
                {
                    try
                    {
                        string OopsAnswer = "";
                        bool result = PerformPreCheckoutQuery(e.Update.PreCheckoutQuery.InvoicePayload, ref OopsAnswer);
                        if (result)
                        {
                            AnswerPreCheckoutQueryAsync(e.Update.PreCheckoutQuery.Id);
                        }
                        else
                        {
                            AnswerPreCheckoutQueryAsync(e.Update.PreCheckoutQuery.Id, OopsAnswer);
                        }
                    }
                    catch (Exception err)
                    {
                        BotUtils.LogException(err);
                    }
                }
            }
        }
    }
}