using BrutelDiscord.Abstractions.Gateway;
using BrutelDiscord.Clients;
using BrutelDiscord.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BrutelDiscord
{
    class HeartbeatHandler
    {
        private GatewayHelloResume _helloResume;
        private SocketClient _client;
        public HeartbeatHandler(GatewayHelloResume helloResume, SocketClient client)
        {
            _helloResume = helloResume;
            _client = client;
        }

        public void Start()
        {
            Heartbeater heartbeater = new Heartbeater(_helloResume.HeartbeatInterval, _client);
            ThreadStart threadStart = heartbeater.StartHeartbeating;
            Thread thread = new Thread(threadStart);
            thread.Start();
        }
    }
}
