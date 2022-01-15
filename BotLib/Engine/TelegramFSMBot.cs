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
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using System.Threading;
using Telegram.Bot.Types.Payments;

namespace BotLib.Engine
{
    public abstract class TelegramFSMBot : TelegramBotClient
    {
        public string me;
        public bool terminate = false;
        internal TelegramMessageSender NonPriorityMessageSender;
        protected AdminTasker AdminTasker;
        protected Dictionary<long, int> LastMessageIds;
        private const string CONFIG_DIR = "botconfig";
        private const string PARAMETRIC_COMMAND_PALETTE = "\\/start (.*)";
        private Type BotMachineType;
        private readonly string configFile = Path.Combine(CONFIG_DIR, "botconfig.json");
        private readonly FSMBotConfig FSMConfig;
        private Type InitStateType;
        private readonly Dictionary<long, BotMachine> Machines;
        private Type ParametricInitStateType;
        private readonly TelegramMessageSender Sender;

        public TelegramFSMBot(string token, HttpClient httpClient = null, bool DebugMode = false, int sendingInterval = 50, int nonPrioritySendingInterval = 1000) : base(token, httpClient)
        {
            this.DebugMode = DebugMode;
            Sender = new TelegramMessageSender(this, sendingInterval);
            NonPriorityMessageSender = new TelegramMessageSender(this, nonPrioritySendingInterval);
            Machines = new Dictionary<long, BotMachine>();
            LastMessageIds = new Dictionary<long, int>();
            AdminTasker = new AdminTasker(this);
            FSMConfig = new FSMBotConfig(configFile);
            SetInitStateType();
            SetParametricInitStateType();
            SetBotMachineType();
            CheckBotMachineType();
            CheckInitStateType();
            Init();
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public delegate void MessagePostedDelegate(long ChatId, int MessageId);

        public event EventHandler<FileTooBigEventArgs> FileTooBig;

        //TODO: public event EventHandler<ChatMembersAddedEventArgs> ChatMembersAdded;
        public event MessagePostedDelegate MessagePosted;

        public event EventHandler<ParametricStartEventArgs> ParametricStartReceived;

        public event EventHandler<PaymentEventArgs> PaymentReceived;

        public bool AnswerToFilesWithType { get; protected set; } = false;

        public bool DebugMode { get; } = false;

        public bool NonPriorityMessageSenderLoggingEnabled
        {
            get
            {
                return NonPriorityMessageSender.LoggingEnabled;
            }
            set
            {
                NonPriorityMessageSender.LoggingEnabled = value;
            }
        }

        public string PaymentsKey { get; private set; }

        public bool SenderLoggingEnabled
        {
            get
            {
                return Sender.LoggingEnabled;
            }
            set
            {
                Sender.LoggingEnabled = value;
            }
        }

        protected double MessageSenderInterval
        {
            get
            {
                return Sender.Interval;
            }
            set
            {
                Sender.SetNewInterval(value);
            }
        }

        protected double NonPriorityMessageSenderInterval
        {
            get
            {
                return NonPriorityMessageSender.Interval;
            }
            set
            {
                NonPriorityMessageSender.SetNewInterval(value);
            }
        }

        public BotMachine CreateMachine(long UserId)
        {
            KillMachine(UserId);
            BotMachine machine = Activator.CreateInstance(BotMachineType, UserId, Sender, InitStateType, this) as BotMachine;
            Machines.Add(UserId, machine);
            return machine;
        }

        public BotMachine CreateMachine(long UserId, Type InitialStateType)
        {
            KillMachine(UserId);
            BotMachine machine = Activator.CreateInstance(BotMachineType, UserId, Sender, InitialStateType, this) as BotMachine;
            Machines.Add(UserId, machine);
            return machine;
        }

        public void KillMachine(long UserId)
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
            me = this.GetMeAsync().Result.Username;
            Console.WriteLine(string.Format("Bot {0}, BotLib v.{1} is starting, previous start time is {2}", me, version, FSMConfig.StartTime));
            FSMConfig.StartTime = DateTime.UtcNow;
        }

        protected virtual void StartReceiving()
        {
            using var cts = new CancellationTokenSource();
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            this.StartReceiving(HandleUpdateAsync,
                               HandleErrorAsync,
                               receiverOptions,
                               cts.Token);
        }

        private bool MachineExists(long UserId)
        {
            return Machines.ContainsKey(UserId);
        }

        private void ReceiveParametricStart(ParametricStartEventArgs e)
        {
            EventHandler<ParametricStartEventArgs> handler = ParametricStartReceived;
            handler?.Invoke(this, e);
            DispatchParametricCommand(e);
        }

        private static async Task TelegramCallbackQuery(TelegramFSMBot bot, CallbackQuery callbackQuery)
        {
            long UserId = callbackQuery.From.Id;
            string CallbackData = callbackQuery.Data;
            TelegramButtonPressedCommand command = new TelegramButtonPressedCommand(UserId, CallbackData);
            await bot.AnswerCallbackQueryAsync(callbackQuery.Id);
            bot.Dispatch(command);
        }

        private static async Task TelegramMessage(TelegramFSMBot bot, Message message)
        {            
            if (message.Type == MessageType.Text)
            {
                if (Regex.IsMatch(message.Text, PARAMETRIC_COMMAND_PALETTE))
                {
                    string ParametricCommand = Regex.Match(message.Text, PARAMETRIC_COMMAND_PALETTE).Groups[1].Value;
                    bot.ReceiveParametricStart(new ParametricStartEventArgs() { ParametricCommand = ParametricCommand, UserId = message.From.Id });
                    return;
                }
                TelegramTextCommand command = new TelegramTextCommand(message.From.Id, message.Text);
                bot.Dispatch(command);
            }
            if (message.Type == MessageType.Photo || message.Type == MessageType.Document)
            {
                MemoryStream ms = new MemoryStream();
                string FileName = null;
                TelegramCommand command = null;
                switch (message.Type)
                {
                    case MessageType.Photo:
                        bot.GetInfoAndDownloadFileAsync(message.Photo.Last().FileId, ms).Wait();
                        ms.Seek(0, SeekOrigin.Begin);
                        command = new TelegramPhotoCommand(message.From.Id, ms.ToArray());
                        break;

                    case MessageType.Document:
                        if (message.Document.FileSize < 19922944)
                        {
                            if (bot.AnswerToFilesWithType) bot.Sender.Enqueue(new TelegramTypingMessage(message.From.Id));
                            bot.GetInfoAndDownloadFileAsync(message.Document.FileId, ms).Wait();
                            ms.Seek(0, SeekOrigin.Begin);
                            FileName = message.Document.FileName;
                            command = new TelegramFileCommand(message.From.Id, ms.ToArray(), FileName);
                        }
                        else
                        {
                            FileTooBigEventArgs ftbea = new FileTooBigEventArgs(message.Chat.Id, message.From.Id, message.MessageId, message.Document.FileName, message.Document.FileId, message.Document.FileSize);
                            EventHandler<FileTooBigEventArgs> handler = bot.FileTooBig;
                            handler.Invoke(bot, ftbea);
                            command = new TelegramEmptyCommand(message.From.Id);
                        }
                        break;
                }
                bot.Dispatch(command);
            }
            if (message.Type == MessageType.SuccessfulPayment)
            {
                string InvoiceId = message.SuccessfulPayment.InvoicePayload;
                PaymentEventArgs pea = new PaymentEventArgs(InvoiceId);
                EventHandler<PaymentEventArgs> handler = bot.PaymentReceived;
                handler.Invoke(bot, pea);
            }
            if (message.Type == MessageType.ChatMembersAdded)
            {
                Console.WriteLine(message.NewChatMembers);
            }
            if (message.Type == MessageType.Location || (message.Type == MessageType.Venue && message.Location != null))
            {
                double[] coords = new double[2];
                coords[0] = message.Location.Latitude;
                coords[1] = message.Location.Longitude;
                TelegramGeoCommand command = new TelegramGeoCommand(message.From.Id, coords);
                bot.Dispatch(command);
            }
        }

        private static async Task AnswerPreCheckoutQuetry(TelegramFSMBot botClient, PreCheckoutQuery preCheckoutQuery)
        {
            try
            {
                string OopsAnswer = "";
                bool result = botClient.PerformPreCheckoutQuery(preCheckoutQuery.InvoicePayload, ref OopsAnswer);
                if (result)
                {
                    await botClient.AnswerPreCheckoutQueryAsync(preCheckoutQuery.Id);
                }
                else
                {
                    await botClient.AnswerPreCheckoutQueryAsync(preCheckoutQuery.Id, OopsAnswer);
                }
            }
            catch (Exception err)
            {
                BotUtils.LogException(err);
            }
        }

        protected static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                UpdateType.PreCheckoutQuery => AnswerPreCheckoutQuetry(botClient as TelegramFSMBot, update.PreCheckoutQuery!),
                // UpdateType.Poll:
                UpdateType.Message => TelegramMessage(botClient as TelegramFSMBot, update.Message!),
                // UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => TelegramCallbackQuery(botClient as TelegramFSMBot, update.CallbackQuery!),
                //UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

    }
}