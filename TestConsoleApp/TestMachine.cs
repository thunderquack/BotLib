using BotLib.Engine;
using BotLib.FSM;
using System;

namespace TestConsoleApp
{
    internal class TestMachine : BotMachine
    {
        public TestMachine(int UserId, TelegramMessageSender sender, Type InitStateType, TelegramFSMBot Bot) : base(UserId, sender, InitStateType, Bot)
        {
        }
    }
}