using System;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using BrutelDiscord.Clients;
using System.Threading.Tasks;
using BrutelDiscord.Enums;

namespace BrutelDiscord
{
    class Heartbeater
    {
        private Timer _timer;
        private SocketClient _client;

        public Heartbeater(int interval, SocketClient client)
        {
            _timer = new Timer(interval);
            _client = client;
        }

        public void StartHeartbeating()
        {
            do
            {
                _timer.Elapsed += SendHeartBeat;
            }
            while (_client.IsConnected);
        }

        private async void SendHeartBeat(object sender, ElapsedEventArgs e)
        {
            await _client.SendAsync(OpCodes.Heartbeat, _client.LastSequenceNumber);
        }
    }
}
