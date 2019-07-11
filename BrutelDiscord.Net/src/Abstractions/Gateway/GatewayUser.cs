using BrutelDiscord.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutelDiscord.Abstractions.Gateway
{
    /// <summary>
    /// Represensts a Discord user
    /// </summary>
    class GatewayUser
    {
        [JsonProperty("id")]
        public ulong Id { get; internal set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; internal set; }

        [JsonProperty("discriminator", NullValueHandling = NullValueHandling.Ignore)]
        public string Discriminator { get; set; }

        [JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
        public string AvatarHash { get; internal set; }

        [JsonProperty("bot", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsBot { get; internal set; }

        [JsonProperty("mfa_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MfaEnabled { get; internal set; }

        [JsonProperty("verified", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Verified { get; internal set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; internal set; }

        [JsonProperty("premium_type", NullValueHandling = NullValueHandling.Ignore)]
        public PremiumTypes? PremiumType { get; internal set; }

        [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
        public string Locale { get; internal set; }

        //public static DiscordUser Clone(DiscordUser other)
        //{
        //    DiscordUser user = new DiscordUser();
        //    user.Id = other.Id;
        //    user.Username = other.Username;
        //    user.Discriminator = other.Discriminator;
        //    user.AvatarHash = other.AvatarHash;
        //    user.IsBot = other.IsBot;
        //    user.MfaEnabled = other.MfaEnabled;
        //    user.Verified = other.Verified;
        //    user.Email = other.Email;
        //    user.PremiumType = other.PremiumType;
        //    user.Locale = other.Locale;
        //    return user;
        //}
    }
}
