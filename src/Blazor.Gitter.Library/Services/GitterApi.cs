using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class GitterApi : IChatApi
    {
        private const string APIBASE = "https://api.gitter.im/v1/";
        private const string APIUSERPATH = "user";
        private const string APIROOMS = "rooms";
        private bool isAot;

        private string Token { get; set; }
        private HttpClient HttpClient { get; set; }
        
        public GitterApi(HttpClient httpClient, bool isAot)
        {
            this.isAot = isAot;
            HttpClient = httpClient ?? throw new Exception("Make sure you have added an HttpClient to your DI Container");
        }

        public void SetAccessToken(string token)
        {
            Token = token;
            PrepareHttpClient();
        }

        private void PrepareHttpClient()
        {
            if (HttpClient.BaseAddress == null || HttpClient.BaseAddress.ToString() != APIBASE)
            {
                HttpClient.BaseAddress = new Uri(APIBASE);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.Replace("\"",String.Empty)}");
            }
        }

        public async Task<IChatUser> GetCurrentUser()
        {
            try
            {
                Console.WriteLine("Fetching gitter user.");
                return (await HttpClient.GetJsonAsync<GitterUser[]>(APIUSERPATH)).First();
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public Task<IChatUser> GetChatUser(string UserId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IChatUser>> GetChatRoomUsers(string RoomId)
        {
            throw new NotImplementedException();
        }

        public async Task<IChatRoom> GetChatRoom(string UserId, string RoomId)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"id", RoomId}
            });

            if (isAot)
            {
                var result = await HttpClient.PostAsync($"{APIUSERPATH}/{UserId}/{APIROOMS}", content);
                return JsonConvert.DeserializeObject<GitterRoom[]>(await result.Content.ReadAsStringAsync()).First();
            }
            else
            {
                return (await HttpClient.PostJsonAsync<GitterRoom[]>($"{APIUSERPATH}/{UserId}/{APIROOMS}", content)).First();
            }
        }

        public async Task<IEnumerable<IChatRoom>> GetChatRooms(string UserId)
        {
            if (isAot)
            {
                var result = await HttpClient.GetAsync($"{APIUSERPATH}/{UserId}/{APIROOMS}");
                return JsonConvert.DeserializeObject<GitterRoom[]>(await result.Content.ReadAsStringAsync());
            }
            else
            {
                return await HttpClient.GetJsonAsync<GitterRoom[]>($"{APIUSERPATH}/{UserId}/{APIROOMS}");
            }
        }

        public Task<IChatMessage> GetChatMessage(string RoomId, string MessageId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IChatMessage>> GetChatMessages(string RoomId, IChatMessageOptions Options)
        {
            if (isAot)
            {
                var result = await HttpClient.GetAsync($"{APIROOMS}/{RoomId}/chatMessages{Options}");
                return JsonConvert.DeserializeObject<GitterMessage[]>(await result.Content.ReadAsStringAsync());
            }
            else
            {
                return await HttpClient.GetJsonAsync<GitterMessage[]>($"{APIROOMS}/{RoomId}/chatMessages{Options}");
            }
        }

        public async Task<IEnumerable<IChatMessage>> SearchChatMessages(string RoomId, IChatMessageOptions Options)
        {
            if (string.IsNullOrWhiteSpace(Options.Query))
            {
                return default;
            }
            return await GetChatMessages(RoomId, Options);
        }

        public async Task<IChatMessage> SendChatMessage(string RoomId, string Message)
        {
            var content = new NewMessage() { text = Message };
            
            if (isAot)
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                var result = await HttpClient.PostAsync($"{APIROOMS}/{RoomId}/chatMessages", stringContent);
                return JsonConvert.DeserializeObject<GitterMessage>(await result.Content.ReadAsStringAsync());
            }
            else
            {
                return await HttpClient.PostJsonAsync<GitterMessage>($"{APIROOMS}/{RoomId}/chatMessages", content);
            }
        }

        public async Task<IChatMessage> EditChatMessage(string RoomId, string MessageId, string Message)
        {
            var content = new NewMessage() { text = Message };
            if (isAot)
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                var result = await HttpClient.PostAsync($"{APIROOMS}/{RoomId}/chatMessages/{MessageId}", stringContent);
                return JsonConvert.DeserializeObject<GitterMessage>(await result.Content.ReadAsStringAsync());
            }
            else
            {
                return (await HttpClient.PutJsonAsync<GitterMessage>($"{APIROOMS}/{RoomId}/chatMessages/{MessageId}", content));
            }
        }

        public async Task<bool> MarkChatMessageAsRead(string UserId, string RoomId, string MessageId)
        {
            var content = new MarkUnread { chat = new string[] { MessageId } };
            try
            {
                if (isAot)
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                    var result = await HttpClient.PostAsync($"{APIUSERPATH}/{UserId}/{APIROOMS}/{RoomId}/unreadItems", stringContent);
                    return JsonConvert.DeserializeObject<SimpleSuccess>(await result.Content.ReadAsStringAsync()).success;
                }
                else
                {
                    var result = await HttpClient.PostJsonAsync<SimpleSuccess>($"{APIUSERPATH}/{UserId}/{APIROOMS}/{RoomId}/unreadItems", content);
                    return result.success;
                }
            }
            catch { }
            return false;
        }

        public IChatMessageOptions GetNewOptions()
        {
            return new GitterMessageOptions();
        }
    }
    public class NewMessage
    {
        public string text { get; set; }
    }
    public class MarkUnread
    {
        public string[] chat { get; set; }
    }

    public class SimpleSuccess
    {
        public bool success { get; set; }
    }
}
