using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrutelDiscord.Abstractions.Gateway;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BrutelDiscord
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Trying to connect to websocket server");

            Uri websocketUri = new Uri("wss://gateway.discord.gg/?v=6&encoding=json");

            Console.Read();
        }
    }
}
