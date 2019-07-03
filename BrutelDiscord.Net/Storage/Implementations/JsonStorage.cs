using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BrutelDiscord.Storage.Implementations
{
    public class JsonStorage : IDataStorage
    {
        private const string configFolder = "Resources";
        private const string tokenFile = "token.json";

        public T RestoreObject<T>(string key)
        {
            var json = File.ReadAllText($"{key}.json");
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void StoreObject(object obj, string key)
        {
            var file = $"{key}.json";
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(file, json);
        }
        public static bool ConfigExists()
        {
            return File.Exists(configFolder + "/" + tokenFile);
        }
        public static void SetToken()
        {
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }

            Console.WriteLine("Please Input your Bot Token");
            string token = Console.ReadLine();
            SocketConfig config = new SocketConfig();
            config.Token = token;

            string json = JsonConvert.SerializeObject(config);
            File.WriteAllText(configFolder + "/" + tokenFile, json);
        }

        public static SocketConfig GetToken()
        {
            string jsonString = File.ReadAllText(configFolder + "/" + tokenFile);
            SocketConfig config = JsonConvert.DeserializeObject<SocketConfig>(jsonString);
            return config;
        }
    }
}
