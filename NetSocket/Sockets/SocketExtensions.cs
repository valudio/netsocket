using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetSocket.Sockets
{
    public static class SocketExtensions
    {
        public static async Task SendAsync(this WebSocket ws, string message)
        {
            try
            {
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text,
                    true, CancellationToken.None);
            }
            catch (Exception)
            {
                //swallow this, most probable cause is a disposed client by another thread.
            }
            
        }

        public static string GetResponse(this byte[] buffer)
        {
            var payloadData = buffer.Where(b => b != 0).ToArray();
            return Encoding.UTF8.GetString(payloadData, 0, payloadData.Length);
        }
    }
}
