using BrutelDiscord.Abstractions.Gateway;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Dto.EventArg
{
    public class SocketMessageEventArgs
    {
       public GatewayPayload Payload { get; set; }
    }
}
