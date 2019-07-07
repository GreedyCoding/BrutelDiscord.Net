using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Abstractions.Gateway
{
    class GatewayResume
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("seq")]
        public long SequenceNumber { get; set; }
    }
}
