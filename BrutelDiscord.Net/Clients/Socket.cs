using BrutelDiscord.Abstractions.Gateway;
using BrutelDiscord.Dto.EventArg;
using BrutelDiscord.Enums;
using BrutelDiscord.Interfaces;
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
    public class Socket : ISocket
    {
        public event EventHandler<SocketMessageEventArgs> ReceivedMessage;
        private void OnReceived(SocketMessageEventArgs e) => this.ReceivedMessage?.Invoke(this, e);

        private bool disposedValue = false; // To detect redundant calls
        private Logger _logger;
        private ClientWebSocket _webSocket;
        private Uri _webSocketUri;
        private System.Threading.Timer _heartBeatTimer;
        private CancellationTokenSource _cts;

        private Task _listenTask;
        private object _lastSequenceNumber;

        public bool IsConnected => this._webSocket.State == WebSocketState.Open;

        public Socket()
        {
            this._logger = LogManager.GetCurrentClassLogger();
            this._webSocket = new ClientWebSocket();
        }

        public async Task<bool> Connect(string uri, TimeSpan pollTime,CancellationToken token)
        {
            var result = false;
            this._webSocketUri = new Uri(uri);
            await this._webSocket.ConnectAsync(this._webSocketUri, token);
            

            if(this._webSocket.State == WebSocketState.Open)
            {
                this._logger?.Info("Connection is now open");
                //Handling first Payload
                this._cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                this._listenTask = this.LookForMessage(pollTime,this._cts.Token);
                result = true;
               
            }

            else
            {
                this._logger?.Warn($"Konnte Verbindung {uri} nicht aufbauen");
            }

            return result;
        }


        public void StartHeartBeat(int intervall)
        {
            this._heartBeatTimer = new Timer(this.StartHeartbeatingNew, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(intervall));
        }

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

            await this._webSocket.SendAsync(arraySegment, WebSocketMessageType.Binary, true, this._cts.Token);
            this._logger?.Debug("Sent payload to the server");
        }

        private async void  StartHeartbeatingNew(object state)
        {
            this._logger.Debug("Send Heartbeat");
            await this.SendAsync(OpCodes.Heartbeat, this._lastSequenceNumber);
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


        private async Task ReadMessage()
        {
            var resultMessage = string.Empty;

            var buffer = new byte[4096];

            while (true)
            {
                ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);
                var result = await this._webSocket.ReceiveAsync(arraySegment, this._cts.Token);
                resultMessage += Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);

                if (result.EndOfMessage)
                {
                    break;
                }
            }

            this._logger?.Debug(resultMessage);
            this.OnReceived(new SocketMessageEventArgs { Message = resultMessage });
        }

        private async Task LookForMessage(TimeSpan pollTime, CancellationToken token)
        {
            while (!token.IsCancellationRequested && this.IsConnected)
            {
                await this.ReadMessage();
                await Task.Delay(pollTime);
            }
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
