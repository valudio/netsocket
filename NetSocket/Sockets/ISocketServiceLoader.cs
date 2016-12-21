namespace PhoneNotifier.WS.Core.Sockets
{
    public interface ISocketServiceLoader
    {
        void LoadServices(ISocketManager socketManager);
    }
}