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
using NLog;
using BrutelDiscord.Interfaces;

namespace BrutelDiscord.Clients
{
    public class SocketClient : ISocketClient
    {
        public ClientWebSocket WebSocket { get; private set; }

        private readonly Logger _logger;

        public CancellationTokenSource TokenSource { get;  private set; }
        public Uri WebSocketUri { get; set; }

        public bool IsConnected => this._socket.IsConnected;

        public int HeartbeatInterval { get; private set; }

   

        public int? LastSequenceNumber { get; private set; } = null;

        private Task _heartbeatHandlerTask;
        private Task _socketMessageHandlerTask;

        private SocketConfig _socketConfig;

        private List<GatewayPayload> _receivedPayloads = new List<GatewayPayload>();

        private ISocket _socket;
        private string _uri;

        public SocketClient(string uri, ISocket socket)
        {
            this._logger = LogManager.GetCurrentClassLogger();
            TokenSource = new CancellationTokenSource();
            this._socket = socket;          
            this._uri = uri;
            this._logger?.Debug("Hallo");
        }

        public async Task StartAsync()
        {
            this._socket.ReceivedMessage += this.MessageReceived;
            if(await this._socket.Connect(this._uri, TimeSpan.FromMilliseconds(300),this.TokenSource.Token))
            {
                this._logger?.Info("Verbunden");
              
            }
            else
            {
                throw new  Exception("Comnection to server failed");
            }
        }

        private async void MessageReceived(object sender, Dto.EventArg.SocketMessageEventArgs e)
        {
            this._logger?.Debug("Received Message");
            var payload = JsonConvert.DeserializeObject<GatewayPayload>(e.Message);
            await this.HandleGatewayPayload(payload);
        }

        public void Stop(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            this.TokenSource.Cancel();
            this._socket.Stop();
            this._socket.ReceivedMessage -= this.MessageReceived;
            
            Console.WriteLine($"Connection to the websocket is now {WebSocket.State.ToString()}");
        }   

        private async Task HandleGatewayPayload(GatewayPayload payload)
        {
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

            this._socket.StartHeartBeat(helloResume.HeartbeatInterval);

            GatewayIdentify gatewayIdentify = new GatewayIdentify();
            _socketConfig = JsonStorage.GetToken();
            gatewayIdentify.Token = _socketConfig.Token;
            await this._socket.SendAsync(OpCodes.Identity, gatewayIdentify);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._socket.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

        }
        #endregion


    }
}
