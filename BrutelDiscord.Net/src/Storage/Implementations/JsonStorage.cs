using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrutelDiscord.Abstractions;
using Newtonsoft.Json;

namespace BrutelDiscord.Storage.Implementations
{
    public class JsonStorage : IDataStorage
    {
        private const string CONFIG_FOLDER = "Resources";
        private const string CONFIG_FILENAME = "config.json";

        /// <summary>
        /// Restores object from a json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Name of the json file wanting to restore</param>
        /// <returns></returns>
        public T RestoreObject<T>(string key)
        {
            var json = File.ReadAllText($"{key}.json");
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Stores and object as a json
        /// </summary>
        /// <param name="obj">Object that should be serialized</param>
        /// <param name="key">Name of the json file we want to write to</param>
        public void StoreObject(object obj, string key)
        {
            var file = $"{key}.json";
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(file, json);
        }

        /// <summary>
        /// Checks if there is already a config file
        /// </summary>
        /// <returns>bool - If the file is available</returns>
        public static bool ConfigExists()
        {
            return File.Exists(CONFIG_FOLDER + "/" + CONFIG_FILENAME);
        }

        /// <summary>
        /// Checks if the config directory exists and lets the user set his token in the console window
        /// </summary>
        public static void SetToken()
        {
            if (!Directory.Exists(CONFIG_FOLDER))
            {
                Directory.CreateDirectory(CONFIG_FOLDER);
            }

            Console.WriteLine("Please Input your Bot Token");
            string token = Console.ReadLine();
            SocketConfig config = new SocketConfig();
            config.Token = token;

            string json = JsonConvert.SerializeObject(config);
            File.WriteAllText(CONFIG_FOLDER + "/" + CONFIG_FILENAME, json);
        }

        /// <summary>
        /// Gets the stored config file
        /// </summary>
        /// <returns></returns>
        public static SocketConfig GetConfig()
        {
            string jsonString = File.ReadAllText(CONFIG_FOLDER + "/" + CONFIG_FILENAME);
            SocketConfig config = JsonConvert.DeserializeObject<SocketConfig>(jsonString);
            return config;
        }
    }
}
