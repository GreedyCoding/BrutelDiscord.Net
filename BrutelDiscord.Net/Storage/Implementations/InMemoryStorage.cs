using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrutelDiscord.Storage.Implementations
{
    //In memory storage implementation for storing information during runtime

    class InMemoryStorage : IDataStorage
    {    

        Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public void StoreObject(object obj, string key)
        {
            if (_dictionary.ContainsKey(key)) return;
            
            _dictionary.Add(key, obj);
        }

        public T RestoreObject<T>(string key)
        {
            if (!_dictionary.ContainsKey(key))
            {
                throw new ArgumentException($"The provided key '{key}' was not found");
            }
            return (T)_dictionary[key];
        }
    }
}
