using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;

using Xamarin.Essentials;

namespace Blazor.Gitter.XamarinShared
{
    public class XamarinLocalStorageService : ILocalStorageService, ISyncLocalStorageService
    {
        public event EventHandler<ChangingEventArgs> Changing;
        public event EventHandler<ChangedEventArgs> Changed;
        private List<string> keys = new List<string>();

        public Task ClearAsync()
        {
            SecureStorage.RemoveAll();
            keys.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainKeyAsync(string key)
        {
            return Task.FromResult(keys.Contains(key));
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            var item = await SecureStorage.GetAsync(key);
            if (item == null)
                return default(T);
            return JsonSerializer.Deserialize<T>(item);
        }

        public Task<string> KeyAsync(int index)
        {
            return Task.FromResult(keys[index]);
        }

        public Task<int> LengthAsync()
        {
            return Task.FromResult(keys.Count);
        }

        public Task RemoveItemAsync(string key)
        {
            SecureStorage.Remove(key);
            keys.Remove(key);
            keys.Sort();
            return Task.CompletedTask;
        }

        public async Task SetItemAsync(string key, object data)
        {
            var e = RaiseOnChangingSync(key, data);

            if (e.Cancel)
                return;

            await SecureStorage.SetAsync(key, JsonSerializer.Serialize(data));
            if (!keys.Contains(key))
            {
                keys.Add(key);
                keys.Sort();
            }

            RaiseOnChanged(key, e.OldValue, data);
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
            SecureStorage.RemoveAll();
            keys.Clear();
        }

        public T GetItem<T>(string key)
        {
            var item = Task.Run(async () => await SecureStorage.GetAsync(key)).Result;
            if (item == null)
                return default(T);
            return JsonSerializer.Deserialize<T>(item);
        }

        public string Key(int index)
        {
            return keys[index];
        }

        public bool ContainKey(string key)
        {
            return keys.Contains(key);
        }

        public int Length()
        {
            return keys.Count;
        }

        public void RemoveItem(string key)
        {
            SecureStorage.Remove(key);
            keys.Remove(key);
            keys.Sort();
        }

        public void SetItem(string key, object data)
        {
            var e = RaiseOnChangingSync(key, data);

            if (e.Cancel)
                return;

            Task.Run(async () => await SecureStorage.SetAsync(key, JsonSerializer.Serialize(data))).Wait();

            if (!keys.Contains(key))
            {
                keys.Add(key);
                keys.Sort();
            }

            RaiseOnChanged(key, e.OldValue, data);
        }
    }
}