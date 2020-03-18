using Newtonsoft.Json;
using System;

namespace BotLib.Engine
{
    internal class FSMBotConfig : ConfigStorage
    {
        public int MainAdminId => Convert.ToInt32(this["MainAdminId"]);

        public DateTime StartTime
        {
            get
            {
                if (this["StartTime"] == null)
                    this["StartTime"] = BotUtils.DateTimeToJsonString(DateTime.UtcNow);
                return BotUtils.JsonStringToDateTime(this["StartTime"]);
            }
            set => this["StartTime"] = JsonConvert.SerializeObject(value);
        }

        public FSMBotConfig(string FileName) : base(FileName)
        {
        }
    }
}