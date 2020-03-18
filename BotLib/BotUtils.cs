using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BotLib
{
    public static class BotUtils
    {
        public static string DateTimeToJsonString(DateTime dateTime)
        {
            return JsonConvert.SerializeObject(dateTime);
        }

        public static int DemaskString(string Value)
        {
            long N = Convert.ToInt64(Value, 16);
            bool[] n = ConvertLongToBoolArray(N);
            bool[] v = new bool[32];
            for (int i = 0; i < 32; i++)
            {
                v[i] = n[2 * i];
            }
            return ConvertBoolArrayToInt(v);
        }

        public static DateTime JsonStringToDateTime(string JsonString)
        {
            return (DateTime)JsonConvert.DeserializeObject(JsonString, typeof(DateTime));
        }

        public static void LogException(Exception exception)
        {
            Exception ex = exception;
            string output = "";
            output = output + "-----------------------------------------------------------------------------\r\n";
            output = output + "Date : " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss") + "\r\n";
            while (ex != null)
            {
                output = output + ex.GetType().FullName + "\r\n";
                output = output + "Message : " + ex.Message + "\r\n";
                output = output + "StackTrace : " + ex.StackTrace + "\r\n";
                ex = ex.InnerException;
            }
            System.IO.File.AppendAllText("botlog.txt", output, Encoding.UTF8);
            Console.WriteLine(output);
        }

        public static string MaskInt(int Value)
        {
            Random r = new Random();
            int Mask = r.Next();
            bool[] v = ConvertIntToBoolArray(Value);
            bool[] m = ConvertIntToBoolArray(Mask);
            bool[] l = new bool[64];
            for (int i = 0; i < 32; i++)
            {
                l[i * 2] = v[i];
                l[i * 2 + 1] = m[i];
            }
            long N = ConvertBoolArrayToLong(l);
            return Convert.ToString(N, 16);
        }

        public static string ShortHash(string rawData, int NumLetters = 10)
        {
            if (NumLetters < 0) throw new Exception("Numletters could't be negative");
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                string res = builder.ToString();
                if (NumLetters > res.Length)
                    return res;
                else
                    return res.Substring(0, NumLetters);
            }
        }

        private static int ConvertBoolArrayToInt(bool[] source)
        {
            string arr = "";
            for (int i = 0; i < 32; i++)
                if (source[i]) arr = arr + "1"; else arr = arr + "0";
            int o = Convert.ToInt32(arr, 2);
            return o;
        }

        private static long ConvertBoolArrayToLong(bool[] source)
        {
            string arr = "";
            for (int i = 0; i < 64; i++)
                if (source[i]) arr = arr + "1"; else arr = arr + "0";
            long o = Convert.ToInt64(arr, 2);
            return o;
        }

        private static bool[] ConvertIntToBoolArray(int Value)
        {
            string arr = Convert.ToString(Value, 2).PadLeft(32, '0');
            bool[] result = new bool[32];
            for (int i = 0; i < 32; i++)
                if (arr[i] == '0') result[i] = false; else result[i] = true;
            return result;
        }

        private static bool[] ConvertLongToBoolArray(long Value)
        {
            string arr = Convert.ToString(Value, 2).PadLeft(64, '0');
            bool[] result = new bool[64];
            for (int i = 0; i < 64; i++)
                if (arr[i] == '0') result[i] = false; else result[i] = true;
            return result;
        }
    }
}