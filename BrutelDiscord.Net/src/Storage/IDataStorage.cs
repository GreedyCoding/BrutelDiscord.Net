using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrutelDiscord.Storage
{
    //Interface for storing data in distinct storage types

    interface IDataStorage
    {
        void StoreObject(object obj, string key);

        T RestoreObject<T>(string key);
    }
}
