using BrutelDiscord.Abstractions.Gateway;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrutelDiscord.Clients
{
    class DiscordSocketClient : IDisposable
    {
        public ClientWebSocket WebSocket { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public CancellationToken CancelToken { get; set; }

        public Uri WebSocketUri { get; set; }

        private async Task StartAsync()
        {
            await WebSocket.ConnectAsync(WebSocketUri, CancelToken);

            Console.WriteLine($"The websocket status is {WebSocket.State.ToString()}");

            var buffer = new byte[1024];
            ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);

            WebSocketReceiveResult result = await WebSocket.ReceiveAsync(arraySegment, CancelToken);

            string resultMessage = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
            Console.WriteLine(resultMessage);

            HandleSocketMessage(resultMessage);
          

        }
        void HandleSocketMessage(string message)
        {
            var payload = JsonConvert.DeserializeObject<Payload>(message);
            var opcode = payload.Opcode;

            switch (opcode)
            {
                case Enums.OpCodes.Dispatch:
                    break;
                case Enums.OpCodes.Heartbeat:
                    break;
                case Enums.OpCodes.Identity:
                    break;
                case Enums.OpCodes.StatusUpdate:
                    break;
                case Enums.OpCodes.VoiceStateUpdate:
                    break;
                case Enums.OpCodes.Resume:
                    break;
                case Enums.OpCodes.Reconnect:
                    break;
                case Enums.OpCodes.RequestGuildMembers:
                    break;
                case Enums.OpCodes.InvalidSession:
                    break;
                case Enums.OpCodes.Hello:
                    break;
                case Enums.OpCodes.HeartbeatAcknowledge:
                    break;
                default:
                    break;
            }
        }

        private async Task StopAsync()
        {
            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancelToken);
            Console.WriteLine($"Connection to the websocket is now {WebSocket.State.ToString()}");
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
