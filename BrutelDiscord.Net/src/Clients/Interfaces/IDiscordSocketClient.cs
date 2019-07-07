using BrutelDiscord.Enums;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace BrutelDiscord.Clients.Interfaces
{
    internal interface IDiscordSocketClient : IDisposable
    {
        CancellationTokenSource TokenSource { get; }
        bool IsConnected { get; }

        Task StartAsync();
        Task StopAsync(WebSocketCloseStatus closeStatus, string statusDescription);
    }
}