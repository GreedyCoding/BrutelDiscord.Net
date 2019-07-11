using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Entities
{
    public abstract class SnowflakeObject
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong Id { get; internal set; }

        [JsonIgnore]
        public DateTimeOffset CreationTimestamp
            => new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.FromMilliseconds(Id >> 22));
    }
}
