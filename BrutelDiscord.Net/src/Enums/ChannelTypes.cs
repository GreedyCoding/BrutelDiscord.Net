using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Enums
{
    public enum ChannelTypes
    {
        Text = 0,

        Private = 1,

        Voice = 2,

        Group = 3,

        Category = 4,

        Unknown = int.MaxValue
    }
}
