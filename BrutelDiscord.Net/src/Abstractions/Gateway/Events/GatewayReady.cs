using BrutelDiscord.Entities.Snowflakes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Abstractions.Gateway.Events
{
    class GatewayReady
    {
        [JsonProperty("v")]
        public int GatewayVersion { get; private set; }

        [JsonProperty("user")]
        public GatewayUser CurrentUser { get; private set; }

        [JsonProperty("private_channels")]
        public List<DiscordDmChannel> DmChannels { get; private set; }

        [JsonProperty("guilds")]
        public List<DiscordGuild> Guilds { get; private set; }

        [JsonProperty("session_id")]
        public string SessionId { get; private set; }

        [JsonProperty("_trace")]
        public IReadOnlyList<string> Trace { get; private set; }
    }
}
