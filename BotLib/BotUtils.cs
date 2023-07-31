using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
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

        public static long DemaskString(string Value)
        {
            BigInteger N = BigInteger.Parse(Value, NumberStyles.HexNumber);
            byte[] byteArray = N.ToByteArray();
            BitArray bitArray = new BitArray(byteArray);
            List<bool> boolList = new List<bool>();
            for (int i = 0; i < 64; i++)
            {
                boolList.Add(bitArray[2 * i]);
            }
            bitArray = new BitArray(boolList.ToArray());
            byteArray = BitArrayToByteArray(bitArray);

            return BitConverter.ToInt64(byteArray);
        }

        public static DateTime JsonStringToDateTime(string JsonString)
        {
            return (DateTime)JsonConvert.DeserializeObject(JsonString, typeof(DateTime));
        }

        public static void LogException(Exception exception)
        {
            Exception ex = exception;
            string output = "";
            output += $"-----------------------------------------------------------------------------{Environment.NewLine}";
            output += "Date : " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss") + Environment.NewLine;
            while (ex != null)
            {
                output = output + ex.GetType().FullName + Environment.NewLine;
                output = output + "Message : " + ex.Message + Environment.NewLine;
                output = output + "StackTrace : " + ex.StackTrace + Environment.NewLine;
                ex = ex.InnerException;
            }
            File.AppendAllText(Path.Combine("botconfig", "botlog.txt"), output, Encoding.UTF8);
            Console.WriteLine(output);
        }

        public static string MaskInt(int Value)
        {
            Random r = new Random();
            int Mask = r.Next();
            bool[] v = ConvertLongToBoolArray(Value);
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

        public static string MaskLong(long Value)
        {
            Random r = new Random();
            long Mask = r.NextInt64();

            byte[] byteArray = BitConverter.GetBytes(Value);
            BitArray v = new BitArray(byteArray);
            byteArray = BitConverter.GetBytes(Mask);
            BitArray m = new BitArray(byteArray);
            bool[] l = new bool[128];
            for (int i = 0; i < 64; i++)
            {
                l[i * 2] = v[i];
                l[i * 2 + 1] = m[i];
            }
            BitArray b = new BitArray(l);
            byte[] vs = BitArrayToByteArray(b);
            BigInteger N = new BigInteger(vs);
            return N.ToString("X");
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

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
    }
}