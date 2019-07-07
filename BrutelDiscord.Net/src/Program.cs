using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrutelDiscord.Abstractions.Gateway;
using BrutelDiscord.Clients;
using BrutelDiscord.Storage.Implementations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace BrutelDiscord
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger _logger = LogManager.GetCurrentClassLogger();

            if (!JsonStorage.ConfigExists())
            {
                JsonStorage.SetToken();
            }
            
            DiscordSocketClient client = new DiscordSocketClient("wss://gateway.discord.gg/?v=6&encoding=json", new SocketClient());
            _logger?.Info("Trying to establish connection to the websocket server");

            await client.StartAsync();
            await Task.Delay(-1);
        }
    }
}
