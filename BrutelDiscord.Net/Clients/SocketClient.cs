using BrutelDiscord.Abstractions;
using BrutelDiscord.Abstractions.Gateway;
using BrutelDiscord.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BrutelDiscord.Clients
{
    class SocketClient : ISocketClient
    {
        public ClientWebSocket WebSocket { get; private set; }
        public CancellationTokenSource TokenSource { get;  private set; }
        public CancellationToken CancelToken { get; private set; }
        public Uri WebSocketUri { get; set; }
        public bool IsConnected { get; private set; }
        public int HeartbeatInterval { get; private set; }
        public int? LastSequenceNumber { get; private set; } = null;

        public SocketClient(string uri)
        {
            TokenSource = new CancellationTokenSource();
            CancelToken = TokenSource.Token;
            WebSocket = new ClientWebSocket();
            WebSocketUri = new Uri(uri);
        }

        public async Task StartAsync()
        {
            await WebSocket.ConnectAsync(this.WebSocketUri, this.CancelToken);
            if(WebSocket.State == WebSocketState.Open)
            {
                Console.WriteLine("Connection is now open");
                IsConnected = true;
                await HandleSocketMessage();
            }
            else
            {
                Console.WriteLine("Comnection to server failed");
            }
        }

        public async Task StopAsync(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            await WebSocket.CloseAsync(closeStatus, statusDescription, this.CancelToken);
            Console.WriteLine($"Connection to the websocket is now {WebSocket.State.ToString()}");
        }

        public async Task SendAsync(OpCodes opCode, object data, int? sequence = null, string eventName = null)
        {
            GatewayPayload payload = new GatewayPayload();
            payload.Opcode = opCode;
            payload.Data = data;
            payload.SequenceNumber = sequence;
            payload.EventName = eventName;

            string jsonString = JsonConvert.SerializeObject(payload);
            var buffer = UTF8Encoding.UTF8.GetBytes(jsonString);
            ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);

            await WebSocket.SendAsync(arraySegment, WebSocketMessageType.Binary, true, this.CancelToken);
            Console.WriteLine("Sent payload to the server");
        }

        public async Task<string> ReceiveMessage()
        {
            var buffer = new byte[1024];
            ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);

            WebSocketReceiveResult result = await WebSocket.ReceiveAsync(arraySegment, this.CancelToken);

            string resultMessage = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
            Console.WriteLine(resultMessage);
            return resultMessage;
        }

        private async Task HandleSocketMessage()
        {
            string message = await ReceiveMessage();
            var payload = JsonConvert.DeserializeObject<GatewayPayload>(message);
            var opcode = payload.Opcode;

            switch (opcode)
            {
                case OpCodes.Dispatch:
                    break;
                case OpCodes.Heartbeat:
                    break;
                case OpCodes.Identity:
                    break;
                case OpCodes.StatusUpdate:
                    break;
                case OpCodes.VoiceStateUpdate:
                    break;
                case OpCodes.Resume:
                    break;
                case OpCodes.Reconnect:
                    break;
                case OpCodes.RequestGuildMembers:
                    break;
                case OpCodes.InvalidSession:
                    break;
                case OpCodes.Hello:
                    await HelloMessageHandlerAsync((payload.Data as JObject).ToObject<GatewayHelloResume>());
                    break;
                case OpCodes.HeartbeatAcknowledge:
                    break;
                default:
                    break;
            }
        }

        private async Task HelloMessageHandlerAsync(GatewayHelloResume helloResume)
        {
            HeartbeatHandler heartbeatHandler = new HeartbeatHandler(helloResume, this);
            heartbeatHandler.Start();

            GatewayIdentify gatewayIdentify = new GatewayIdentify();
            gatewayIdentify.Token = "NTkxMzY0NTAyMjA1ODkwNTY3.XRU0CQ.wGz55H0CIb1ss7zhIFoRhFpfduU";
            await SendAsync(OpCodes.Identity, gatewayIdentify);
            await ReceiveMessage();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
