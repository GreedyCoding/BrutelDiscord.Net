using BrutelDiscord.Dto.EventArg;
using BrutelDiscord.Enums;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace BrutelDiscord.Clients.Interfaces
{
    public interface ISocketClient : IDisposable
    {
        //Events
        event EventHandler<SocketMessageEventArgs> ReceivedMessage;

        //Properties
        int? LastSequenceNumber { get; }
        bool IsConnected { get; }
        ClientWebSocket WebSocket { get; }
        DateTime LastHeartbeatAcknowledge { get; set; }


        Task<bool> StartAsync(string uri, TimeSpan pollTime, System.Threading.CancellationToken token);
        Task StopAsync(WebSocketCloseStatus status, string statusDescription);
        void StartHeartbeatTimer(int intervall);
        Task SendAsync(OpCodes opCode, object data, int? sequence = null, string eventName = null);
    }
}
