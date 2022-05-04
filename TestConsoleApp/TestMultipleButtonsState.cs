using BotLib.Engine.Messages;
using BotLib.FSM;
using BotLib.FSM.HelperStates;
using System.Collections.Generic;

namespace TestConsoleApp
{
    internal class TestMultipleButtonsState : MultipleCheckBoxButtonsState
    {
        public TestMultipleButtonsState(long UserId, BotMachine Machine) : base(UserId, Machine)
        {
        }

        protected override Dictionary<string, string> ButtonList
        {
            get
            {
                Dictionary<string, string> bl = new Dictionary<string, string>();
                bl.Add("Ведущий", "option_1");
                bl.Add("Фотосъемка", "option_2");
                bl.Add("Видеосъемка", "option_3");
                bl.Add("Украшение", "option_4");
                bl.Add("Торты", "option_5");
                bl.Add("Рестораны", "option_6");
                bl.Add("Аренда машин", "option_7");
                bl.Add("Музыкальное оборудование", "option_8");
                bl.Add("Диджеи", "option_9");
                bl.Add("Сценарии и конкурсы", "option_10");
                bl.Add("Шоу-программы", "option_11");
                return bl;
            }
        }

        protected override bool DisableWebPreview => false;
        protected override string MainButton => "Продолжить";
        protected override string Message => "Что вам ещё потребуется?";
        protected override ParseMode MessageParseMode => ParseMode.Markdown;

        protected override void Done(List<string> chosenValues)
        {
            TelegramTextMessage message = new TelegramTextMessage(UserId, string.Join("\n", chosenValues), ParseMode.Markdown);
            PostMessage(message);
            Machine.Bot.terminate = true;
        }
    }
}