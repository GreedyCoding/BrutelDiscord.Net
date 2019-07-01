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
using BrutelDiscord.Storage.Implementations;

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

        internal Task _heartbeatTask;

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
            IsConnected = false;
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
                    await OnHelloMessageAsync((payload.Data as JObject).ToObject<GatewayHelloResume>());
                    break;
                case OpCodes.HeartbeatAcknowledge:
                    break;
                default:
                    break;
            }
        }

        private async Task OnHelloMessageAsync(GatewayHelloResume helloResume)
        {
            HeartbeatInterval = helloResume.HeartbeatInterval;
            _heartbeatTask = new Task(StartHeartbeating, CancelToken, TaskCreationOptions.LongRunning);
            _heartbeatTask.Start();

            GatewayIdentify gatewayIdentify = new GatewayIdentify();
            gatewayIdentify.Token = JsonStorage.GetToken();

            await SendAsync(OpCodes.Identity, gatewayIdentify);
            await ReceiveMessage();
        }

        private void StartHeartbeating()
        {
            while (IsConnected)
            {
                SendAsync(OpCodes.Heartbeat, LastSequenceNumber).ConfigureAwait(false).GetAwaiter().GetResult();
                Console.WriteLine("Sent Heartbeat");
                Task.Delay(HeartbeatInterval, CancelToken).ConfigureAwait(false).GetAwaiter().GetResult();
            }

        }

        public void Dispose()
        {
            //Remove events in Dispose
            throw new NotImplementedException();
        }
    }
}
