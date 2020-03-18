using System;
using System.Collections.Generic;
using System.Text;

namespace BotLib.Engine.Commands
{
    public class TelegramFileCommand : TelegramBlobCommand
    {
        public string FileName { get; }

        public TelegramFileCommand(int UserId, object Argument, string FileName) : base(TelegramCommandType.FileSent, UserId, Argument)
        {
            this.FileName = FileName;
        }
    }
}