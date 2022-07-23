using System;

namespace BotLib.FSM.FSMEventArgs
{
    public class FileTooBigEventArgs : EventArgs
    {
        public FileTooBigEventArgs(long chatId, long userId, int messageId, string fileName, string fileId, long? size)
        {
            FileName = fileName;
            Size = size;
            ChatId = chatId;
            MessageId = messageId;
            FileId = fileId;
            UserId = userId;
        }

        public long ChatId { get; }
        public string FileId { get; }
        public string FileName { get; }
        public int MessageId { get; }
        public long? Size { get; }
        public long UserId { get; }
    }
}