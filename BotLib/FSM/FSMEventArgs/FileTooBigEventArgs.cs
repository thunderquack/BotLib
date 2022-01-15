using System;

namespace BotLib.FSM.FSMEventArgs
{
    public class FileTooBigEventArgs : EventArgs
    {
        public FileTooBigEventArgs(long ChatId, long UserId, int MessageId, string FileName, string FileId, int? Size)
        {
            this.FileName = FileName;
            this.Size = Size;
            this.ChatId = ChatId;
            this.MessageId = MessageId;
            this.FileId = FileId;
            this.UserId = UserId;
        }

        public long ChatId { get; }
        public string FileId { get; }
        public string FileName { get; }
        public int MessageId { get; }
        public int? Size { get; }
        public long UserId { get; }
    }
}