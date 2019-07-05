using BrutelDiscord.Dto.EventArg;
using BrutelDiscord.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrutelDiscord.Interfaces
{
    public interface ISocket : IDisposable
    {
        event EventHandler<SocketMessageEventArgs> ReceivedMessage;
        bool IsConnected { get; }

        Task<bool> Connect(string uri, TimeSpan pollTime, System.Threading.CancellationToken token);
        void Stop();
        void StartHeartBeat(int intervall);
        Task SendAsync(OpCodes opCode, object data, int? sequence = null, string eventName = null);
    }
}
