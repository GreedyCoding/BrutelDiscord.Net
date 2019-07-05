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
        Uri WebSocketUri { get; }
        bool IsConnected { get; }

        Task StartAsync();
        void Stop (WebSocketCloseStatus closeStatus, string statusDescription);
    }
}