using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrutelDiscord.Abstractions.Gateway;
using BrutelDiscord.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BrutelDiscord
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SocketClient client = new SocketClient("wss://gateway.discord.gg/?v=6&encoding=json");

            Console.WriteLine("Trying to connect to websocket server");
            await client.StartAsync();

            Console.Read();
        }
    }
}
