using BrutelDiscord.Abstractions.Gateway;
using BrutelDiscord.Dto.EventArg;
using BrutelDiscord.Enums;
using BrutelDiscord.Clients.Interfaces;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrutelDiscord.Clients
{
    public class SocketClient : ISocketClient
    {
        //Events
        public event EventHandler<SocketMessageEventArgs> ReceivedMessage;
        private void RaiseReceivedMessage(SocketMessageEventArgs eventArgs) => this.ReceivedMessage?.Invoke(this, eventArgs);

        //Properties
        public int? LastSequenceNumber => this._lastSequenceNumber;
        public bool IsConnected => this._webSocket.State == WebSocketState.Open;
        public ClientWebSocket WebSocket => this._webSocket;

        //Fields
        private bool disposedValue = false; // To detect redundant calls
        private Logger _logger;
        private ClientWebSocket _webSocket;
        private Uri _webSocketUri;
        private Timer _heartBeatTimer;
        private CancellationTokenSource _cancellationTokenSource;

        private Task _listenTask;
        private int? _lastSequenceNumber;

        public SocketClient()
        {
            this._logger = LogManager.GetCurrentClassLogger();
            this._webSocket = new ClientWebSocket();
        }

        /// <summary>
        /// Starting connection to a websocket
        /// </summary>
        /// <param name="uri">Uri of the websocket to connect to</param>
        /// <param name="pollTime">Delay for receiving messages</param>
        /// <param name="token">Cancellation Token from the other client to link our cancelTokens</param>
        /// <returns>bool - if connection was succesful</returns>
        public async Task<bool> StartAsync(string uri, TimeSpan pollTime, CancellationToken token)
        {
            var result = false;
            this._webSocketUri = new Uri(uri);

            try
            {
                await this._webSocket.ConnectAsync(this._webSocketUri, token);
            }
            catch (Exception ex)
            {
                this._logger?.Warn(ex, "Error connecting to the websocket");
                throw ex;
            }
            
            if(this._webSocket.State == WebSocketState.Open)
            {
                this._logger?.Info("Connection is now open");
                //Handling first Payload
                this._cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                this._listenTask = this.ListenForMessage(pollTime, this._cancellationTokenSource.Token);
                result = true;
               
            }
            else
            {
                this._logger?.Warn($"Could not establish connection to this websocket: {uri}");
            }

            return result;
        }

        /// <summary>
        /// Stopping connection to a websocket
        /// </summary>
        public async Task StopAsync(WebSocketCloseStatus status, string statusDescription)
        {
            //TODO Finish implementation of the stop method for the socketclient
            await this._webSocket.CloseAsync(status, statusDescription, this._cancellationTokenSource.Token);
        }


        /// <summary>
        /// Sending payloads to the websocket
        /// </summary>
        /// <param name="opCode">OpCode of the operation wanted to perform</param>
        /// <param name="data">Data to send with the payload</param>
        /// <param name="sequence">Sequence Number</param>
        /// <param name="eventName">Event Name</param>
        public async Task SendAsync(OpCodes opCode, object data, int? sequence = null, string eventName = null)
        {
            var payload = new GatewayPayload
            {
                Opcode = opCode,
                Data = data,
                SequenceNumber = sequence,
                EventName = eventName
            };
            string jsonString = JsonConvert.SerializeObject(payload);

            var buffer = UTF8Encoding.UTF8.GetBytes(jsonString);
            ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);

            await this._webSocket.SendAsync(arraySegment, WebSocketMessageType.Binary, true, this._cancellationTokenSource.Token);
            this._logger?.Debug("Sent payload to the server");
        }

        /// <summary>
        /// Receiving messages from the websocket and deserializing it into an GatewayPayload
        /// </summary>
        private async Task ReceiveMessage()
        {
            var resultMessage = string.Empty;

            var buffer = new byte[4096];

            while (true)
            {
                ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);
                var result = await this._webSocket.ReceiveAsync(arraySegment, this._cancellationTokenSource.Token);
                resultMessage += Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);

                if (result.EndOfMessage)
                {
                    break;
                }
            }

            var payload = JsonConvert.DeserializeObject<GatewayPayload>(resultMessage);

            if (payload.SequenceNumber != null)
            {
                this._lastSequenceNumber = payload.SequenceNumber;
            }

            this._logger?.Debug("Received Message");
            this._logger?.Debug($"OpCode {(int)payload.Opcode}: {payload.Opcode}" +
                                $"Data: {payload.Data}" +
                                $"Sequence Number: {payload.SequenceNumber}" +
                                $"Event Name: {payload.EventName}");

            this.RaiseReceivedMessage(new SocketMessageEventArgs { Payload = payload });
        }

        /// <summary>
        /// This task is used to listen for messages
        /// </summary>
        /// <param name="pollTime">Delay for receiving messages</param>
        /// <param name="token">Linked CancellationToken</param>
        private async Task ListenForMessage(TimeSpan pollTime, CancellationToken token)
        {
            while (!token.IsCancellationRequested && this.IsConnected)
            {
                await this.ReceiveMessage();
                await Task.Delay(pollTime);
            }
        }
        

        /// <summary>
        /// Starts the threading timer with the SendHeartbeat callback every {interval} milliseconds
        /// </summary>
        /// <param name="interval">HeartbeatInterval provided by Discord from the first websocket message</param>
        public void StartHeartbeatTimer(int interval)
        {
            this._heartBeatTimer = new Timer(this.SendHeartbeat, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(interval));
        }

        /// <summary>
        /// Sends a heartbeat payload to the websocket
        /// </summary>
        /// <param name="state"></param>
        private async void SendHeartbeat(object state)
        {
            await this.SendAsync(OpCodes.Heartbeat, this._lastSequenceNumber);
            this._logger.Debug("Sent Heartbeat");
        }


        #region IDisposable Support
    
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {

            Dispose(true);
        }
        #endregion
    }
}
