using System.IO;
using System.Threading;

namespace TestConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {


            TestBot Bot = new TestBot(File.ReadAllText("apikey.txt"));

            while (!Bot.Terminate)
            {
                Thread.Sleep(1000);
            }
        }
    }
}