using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Abstractions.Gateway
{
    class GatewayIdentify
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("properties")]
        public ClientProperties ClientProperties { get; } = new ClientProperties();

        [JsonProperty("compress")]
        public bool Compress { get; set; }

        [JsonProperty("shard")]
        public ShardInfo ShardInfo { get; set; }

        [JsonProperty("presence", NullValueHandling = NullValueHandling.Ignore)]
        public StatusUpdate Presence { get; set; } = null;

    }
}
