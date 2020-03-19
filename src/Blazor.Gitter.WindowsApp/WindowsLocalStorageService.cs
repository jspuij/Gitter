using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using System.Threading;

namespace Blazor.Gitter.WindowsApp
{
    public class WindowsLocalStorageService : ILocalStorageService, ISyncLocalStorageService
    {
        private ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();

        public event EventHandler<ChangingEventArgs> Changing;
        public event EventHandler<ChangedEventArgs> Changed;
        private Dictionary<string, object> storage;

        public Dictionary<string, object> Storage => storage ?? this.ReadDictionary();


        public Task ClearAsync()
        {
            this.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainKeyAsync(string key)
        {
            return Task.FromResult(this.ContainKey(key));
        }

        public Task<T> GetItemAsync<T>(string key)
        {
            return Task.FromResult(this.GetItem<T>(key));
        }

        public Task<string> KeyAsync(int index)
        {
            return Task.FromResult(this.Key(index));
        }

        public Task<int> LengthAsync()
        {
            return Task.FromResult(this.Length());
        }

        public Task RemoveItemAsync(string key)
        {
            this.RemoveItem(key);
            return Task.CompletedTask;
        }

        public Task SetItemAsync(string key, object data)
        {
            this.SetItem(key, data);
            return Task.CompletedTask;
        }

        private ChangingEventArgs RaiseOnChangingSync(string key, object data)
        {
            var e = new ChangingEventArgs
            {
                Key = key,
                OldValue = ((ISyncLocalStorageService)this).GetItem<object>(key),
                NewValue = data
            };

            Changing?.Invoke(this, e);

            return e;
        }

        private void RaiseOnChanged(string key, object oldValue, object data)
        {
            var e = new ChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = data
            };

            Changed?.Invoke(this, e);
        }

        public void Clear()
        {
            slimLock.EnterWriteLock();
            try
            {
                Storage.Clear();
                this.WriteDictionary();
            } finally
            {
                slimLock.ExitWriteLock();
            }
        }

        public T GetItem<T>(string key)
        {
            slimLock.EnterReadLock();
            try
            {
                if (!this.Storage.ContainsKey(key))
                    return default(T);
                return (T)Storage[key];
            } finally
            {
                slimLock.ExitReadLock();
            }
}

        public string Key(int index)
        {
            slimLock.EnterReadLock();
            try
            {
                return this.Storage.Keys.ToList()[index];
            }
            finally
            {
                slimLock.ExitReadLock();
            }
        }

        public bool ContainKey(string key)
        {
            slimLock.EnterReadLock();
            try
            {
                return this.Storage.Keys.Contains(key);
            }
            finally
            {
                slimLock.ExitReadLock();
            }
        }

        public int Length()
        {
            return Storage.Count;
        }

        public void RemoveItem(string key)
        {
            slimLock.EnterWriteLock();
            try
            {
                this.Storage.Remove(key);
                this.WriteDictionary();
            }
            finally
            {
                slimLock.ExitWriteLock();
            }
        }

        public void SetItem(string key, object data)
        {
            var e = RaiseOnChangingSync(key, data);

            if (e.Cancel)
                return;

            slimLock.EnterWriteLock();
            try
            {
                this.Storage[key] = data;
                this.WriteDictionary();
            }
            finally
            {
                slimLock.ExitWriteLock();
            }

            RaiseOnChanged(key, e.OldValue, data);
        }

        private Dictionary<string, object> ReadDictionary()
        {
            // deserialize JSON directly from a file
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                if (isoStore.FileExists("storage.json"))
                {
                    using (var file = isoStore.OpenFile(@"storage.json", FileMode.Open))
                    using (var streamreader = new StreamReader(file))
                    using (var jsonReader = new JsonTextReader(streamreader))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        this.storage = serializer.Deserialize<Dictionary<string, object>>(jsonReader);
                        return this.storage;
                    }
                }
                else
                {
                    this.storage = new Dictionary<string, object>();
                    return this.storage;
                }
            }
        }

        private void WriteDictionary()
        {
            // deserialize JSON directly from a file
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
            {
                // serialize JSON directly to a file
                using (var file = isoStore.OpenFile(@"storage.json", FileMode.OpenOrCreate, FileAccess.Write))
                using (var writer = new StreamWriter(file))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, this.storage);
                }
            }
        }
    }
}
