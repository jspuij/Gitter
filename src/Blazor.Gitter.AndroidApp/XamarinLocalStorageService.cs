using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Blazored.LocalStorage;

using Xamarin.Essentials;

namespace Blazor.Gitter.AndroidApp
{
    public class XamarinLocalStorageService : ILocalStorageService
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
    }
}