using BrutelDiscord.Abstractions;
using BrutelDiscord.Abstractions.Gateway;
using BrutelDiscord.Abstractions.Gateway.Commands;
using BrutelDiscord.Abstractions.Gateway.Events;
using BrutelDiscord.Clients.Interfaces;
using BrutelDiscord.Dto.EventArg;
using BrutelDiscord.Enums;
using BrutelDiscord.Storage.Implementations;
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
using NLog;


namespace BrutelDiscord.Clients
{
    public class DiscordSocketClient : IDiscordSocketClient
    {
        //Properties
        public CancellationTokenSource TokenSource { get; private set; }
        public bool IsConnected => this._socketClient.IsConnected;
        public int? LastSequenceNumber => this._socketClient.LastSequenceNumber;
        
        //Fields
        private readonly Logger _logger;
        private SocketConfig _socketConfig;
        private ISocketClient _socketClient;
        private string _uri;

        /// <summary>
        /// Constructor for DiscordSocketClient
        /// </summary>
        /// <param name="uri">Uri of the websocket to connect to as string</param>
        /// <param name="socket">SocketClient the DiscordSocketClient uses for sending and receiving messages</param>
        public DiscordSocketClient(string uri, ISocketClient socket)
        {
            this._logger = LogManager.GetCurrentClassLogger();
            this.TokenSource = new CancellationTokenSource();
            this._socketClient = socket;          
            this._uri = uri;
        }

        /// <summary>
        /// Subscribes to SocketClients ReceivedMessage event and starts the SocketClient
        /// </summary>
        public async Task StartAsync()
        {
            this._socketClient.ReceivedMessage += this.OnMessageReceived;

            if (await this._socketClient.StartAsync(this._uri, TimeSpan.FromMilliseconds(300), this.TokenSource.Token))
            {
                this._logger?.Info("Connection established");
            }
            else
            {
                throw new Exception("Comnection to server failed");
            }
        }

        /// <summary>
        /// Cancels the TokenSource of this class, stops the SocketClient and unsubscribes from SocketClients events
        /// </summary>
        /// <param name="closeStatus">Status why the connection was closed</param>
        /// <param name="statusDescription">Further description of the close status</param>
        public async Task StopAsync(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            this.TokenSource.Cancel();
            await this._socketClient.StopAsync(closeStatus, statusDescription);
            this._socketClient.ReceivedMessage -= this.OnMessageReceived;

            this._logger?.Info($"Connection to the websocket is now {_socketClient.WebSocket.State.ToString()}");
        }

        /// <summary>
        /// Method hooked to the SocketClients MessageReceived Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs">SocketMessageEventArgs contains the received json message</param>
        private async void OnMessageReceived(object sender, SocketMessageEventArgs eventArgs)
        {
            await this.HandleGatewayPayload(eventArgs.Payload);
        }

        /// <summary>
        /// Handles Gateway Payloads received from the SocketClient
        /// </summary>
        /// <param name="payload">Payload received from the SocketClient</param>
        /// <returns></returns>
        private async Task HandleGatewayPayload(GatewayPayload payload)
        {
            var opcode = payload.Opcode;

            switch (opcode)
            {
                case OpCodes.Dispatch:
                    break;
                case OpCodes.Heartbeat:
                    await this._socketClient.SendAsync(OpCodes.Heartbeat, LastSequenceNumber);
                    break;
                case OpCodes.Reconnect:
                    break;
                case OpCodes.InvalidSession:
                    break;
                case OpCodes.Hello:
                    await OnHelloMessageAsync((payload.Data as JObject).ToObject<GatewayHello>());
                    break;
                case OpCodes.HeartbeatAcknowledge:
                    this._socketClient.LastHeartbeatAcknowledge = DateTime.Now;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to handle the first message from the Discord websocket
        /// </summary>
        /// <param name="helloResume">Gateway payload sent from the websocket</param>
        private async Task OnHelloMessageAsync(GatewayHello helloResume)
        { 

            this._socketClient.StartHeartbeatTimer(helloResume.HeartbeatInterval);

            GatewayIdentify gatewayIdentify = new GatewayIdentify();
            _socketConfig = JsonStorage.GetConfig();
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

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

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
