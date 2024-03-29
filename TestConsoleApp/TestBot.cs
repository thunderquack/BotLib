﻿using BotLib.Engine;
using BotLib.Engine.Commands;
using System.Net.Http;

namespace TestConsoleApp
{
    internal class TestBot : TelegramFSMBot
    {
        public const long TEST_CHAT = 123;

        public TestBot(string token, HttpClient httpClient = null, bool DebugMode = false) : base(
            token, 
            typeof(TestMachine),
            typeof(TestFirstState), 
            typeof(TestMachine),
            httpClient, 
            DebugMode)
        {
            SenderLoggingEnabled = true;
            StartReceiving();
        }

        public override bool PerformPreCheckoutQuery(string InvoiceId, ref string OopsAnser)
        {
            return true;
        }

        public override void SetBotMachineType()
        {
            SetBotMachineType(typeof(TestMachine));
        }

        public override void SetInitStateType()
        {
            SetInitStateType(typeof(TestMultipleButtonsState));
        }

        public override void SetParametricInitStateType()
        {
            SetParametricInitStateType(typeof(TestFirstState));
        }

        protected override bool IsException(TelegramCommand command)
        {
            return false;
        }
    }
}