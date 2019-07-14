using BrutelDiscord.Entities;
using Newtonsoft.Json;
using System.Globalization;

namespace BrutelDiscord.Entities.Snowflakes
{ 
    public class DiscordGuild : SnowflakeObject
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; internal set; }

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string IconHash { get; internal set; }

        [JsonIgnore]
        public string IconUrl
            => !string.IsNullOrWhiteSpace(this.IconHash) 
            ? $"https://cdn.discordapp.com/icons/{this.Id.ToString(CultureInfo.InvariantCulture)}/{IconHash}.jpg" 
            : null;

        [JsonProperty("splash", NullValueHandling = NullValueHandling.Ignore)]
        public string SplashHash { get; internal set; }

        [JsonIgnore]
        public string SplashUrl
            => !string.IsNullOrWhiteSpace(this.SplashHash) ? $"https://cdn.discordapp.com/splashes/{this.Id.ToString(CultureInfo.InvariantCulture)}/{SplashHash}.jpg" : null;

        [JsonProperty("owner_id", NullValueHandling = NullValueHandling.Ignore)]
        internal ulong OwnerId { get; set; }


    //"region": "us-east",
    //"afk_channel_id": "42072017402331136",
    //"afk_timeout": 300,
    //"embed_enabled": true,
    //"embed_channel_id": "41771983444115456",
    //"verification_level": 1,
    //"default_message_notifications": 0,
    //"explicit_content_filter": 0,
    //"mfa_level": 0,
    //"widget_enabled": false,
    //"widget_channel_id": "41771983423143937",
    //"roles": [],
    //"emojis": [],
    //"features": ["INVITE_SPLASH"],
    //"unavailable": false

    }
}
