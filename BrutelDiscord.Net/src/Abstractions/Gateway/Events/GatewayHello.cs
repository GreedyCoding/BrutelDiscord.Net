using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Abstractions.Gateway.Events
{
    class GatewayHello
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }

        [JsonProperty("_trace")]
        public IReadOnlyList<string> Trace { get; private set; }
    }
}
