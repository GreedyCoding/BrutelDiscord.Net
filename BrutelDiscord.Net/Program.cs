using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BrutelDiscord.Net
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Trying to connect to websocket server");

            Uri websocketUri = new Uri("wss://gateway.discord.gg/?v=6&encoding=json");
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            ClientWebSocket webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(websocketUri, token);
            Console.WriteLine($"The websocket status is {webSocket.State.ToString()}");

            var buffer = new byte[1024];
            ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);

            WebSocketReceiveResult result = await webSocket.ReceiveAsync(arraySegment, token);
            string resultMessage = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
            Console.WriteLine(resultMessage);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", token);
            Console.WriteLine($"Connection to the websocket is now {webSocket.State.ToString()}");

            Console.Read();
        }
    }
}
