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
        event EventHandler<SocketMessageEventArgs> ReceivedMessage;
        bool IsConnected { get; }
        ClientWebSocket WebSocket { get; }

        Task<bool> StartAsync(string uri, TimeSpan pollTime, System.Threading.CancellationToken token);
        Task StopAsync();
        void StartHeartbeatTimer(int intervall);
        Task SendAsync(OpCodes opCode, object data, int? sequence = null, string eventName = null);
    }
}
