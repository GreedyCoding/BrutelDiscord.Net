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
using BrutelDiscord.Clients.Interfaces;

namespace BrutelDiscord.Clients
{
    public class DiscordSocketClient : IDiscordSocketClient
    {
        //Properties
        public CancellationTokenSource TokenSource { get; private set; }
        public bool IsConnected => this._socketClient.IsConnected;
        
        //Fields
        private readonly Logger _logger;
        private SocketConfig _socketConfig;
        private ISocketClient _socketClient;
        private string _uri;

        public DiscordSocketClient(string uri, ISocketClient socket)
        {
            this._logger = LogManager.GetCurrentClassLogger();
            this.TokenSource = new CancellationTokenSource();
            this._socketClient = socket;          
            this._uri = uri;
        }

        public async Task StartAsync()
        {
            this._socketClient.ReceivedMessage += this.MessageReceived;

            if(await this._socketClient.StartAsync(this._uri, TimeSpan.FromMilliseconds(300), this.TokenSource.Token))
            {
                this._logger?.Info("Connection established");
            }
            else
            {
                throw new Exception("Comnection to server failed");
            }
        }
        public async Task StopAsync(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            this.TokenSource.Cancel();
            await this._socketClient.StopAsync();
            this._socketClient.ReceivedMessage -= this.MessageReceived;

            this._logger?.Info($"Connection to the websocket is now {_socketClient.WebSocket.State.ToString()}");
        }

        private async void MessageReceived(object sender, Dto.EventArg.SocketMessageEventArgs e)
        {
            this._logger?.Debug("Received Message");
            var payload = JsonConvert.DeserializeObject<GatewayPayload>(e.Message);
            this._logger?.Debug($"{payload.Opcode}{payload.Data}{payload.SequenceNumber}{payload.EventName}");
            await this.HandleGatewayPayload(payload);
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

            this._socketClient.StartHeartbeatTimer(helloResume.HeartbeatInterval);

            GatewayIdentify gatewayIdentify = new GatewayIdentify();
            _socketConfig = JsonStorage.GetToken();
            gatewayIdentify.Token = _socketConfig.Token;
            await this._socketClient.SendAsync(OpCodes.Identity, gatewayIdentify);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._socketClient.Dispose();
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
