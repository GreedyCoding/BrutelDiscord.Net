using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Enums
{
    public enum OpCodes
    {
        Dispatch = 0,

        Heartbeat = 1,

        Identity = 2,

        StatusUpdate = 3,

        VoiceStateUpdate = 4,

        Resume = 6,

        Reconnect = 7,

        RequestGuildMembers = 8,

        InvalidSession = 9,

        Hello = 10,

        HeartbeatAcknowledge = 11
    }
}
