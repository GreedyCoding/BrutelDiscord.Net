using BrutelDiscord.Enums;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BrutelDiscord.Clients
{
    internal interface ISocketClient : IDisposable
    {
        ClientWebSocket WebSocket { get; }
        CancellationTokenSource TokenSource { get; }
        CancellationToken CancelToken { get; }
        Uri WebSocketUri { get; }
        bool IsConnected { get; }

        Task<string> ReceiveMessage();
        Task SendAsync(OpCodes opCode, object data, int? sequence = null, string eventName = null);
        Task StartAsync();
        Task StopAsync(WebSocketCloseStatus closeStatus, string statusDescription);
    }
}