using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Abstractions.Gateway.Commands
{
    class GatewayIdentify
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("properties")]
        public ClientProperties ClientProperties { get; } = new ClientProperties();
    }
}
