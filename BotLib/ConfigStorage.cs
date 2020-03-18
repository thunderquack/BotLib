using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BotLib
{
    public abstract class ConfigStorage
    {
        private readonly string FileName;
        private Dictionary<string, string> Parameters = new Dictionary<string, string>();

        protected ConfigStorage(string FileName)
        {
            this.FileName = FileName;
            string content = "{}";
            if (File.Exists(FileName))
                content = File.ReadAllText(FileName);
            try
            {
                Parameters = JsonConvert.DeserializeObject(content, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
            }
            catch
            {
                Save();
            }
        }

        public string this[string index]
        {
            get
            {
                return Parameters.ContainsKey(index) ? Parameters[index] : null;
            }
            set
            {
                if (!Parameters.ContainsKey(index))
                {
                    Parameters.Add(key: index, value: value);
                }
                else
                {
                    Parameters[index] = value;
                }
                Save();
            }
        }

        private void Save()
        {
            File.WriteAllText(FileName, JsonConvert.SerializeObject(Parameters));
        }
    }
}