﻿using BrutelDiscord.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Abstractions.Gateway
{
    public class GatewayPayload
    {
        [JsonProperty("op")]
        public OpCodes Opcode { get; set; }

        [JsonProperty("d")]
        public object Data { get; set; }

        [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
        public int? SequenceNumber { get; set; }

        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string EventName { get; set; }
    }
}
