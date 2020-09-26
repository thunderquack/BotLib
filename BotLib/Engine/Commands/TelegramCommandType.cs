namespace BotLib.Engine.Commands
{
    public enum TelegramCommandType
    {
        TextEntered,
        PhotoSent,
        FileSent,
        ButtonPressed,
        PaymentReceived,
        Empty,
        Geo
    }
}