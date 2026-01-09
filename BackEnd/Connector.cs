using BackEnd.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace BackEnd
{
    public enum Methods
    {
        POST,
        GET
    }

    public class Connector
    {
        private HTTP _httpClient;
        private string _token;

        public Connector()
        {
            _httpClient = new HTTP();
        }

        public async Task<bool> Authentification(Credential pCredential)
        {
            if(_httpClient is null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(_token))
            {
                return true;
            }

            string Token = await _httpClient.HandShake(pCredential);

            if (string.IsNullOrEmpty(Token))
            {
                return false;
            }

            _token = Token;
            return true;
        } 


        private class HTTP
        {
            private HttpClient _client;
            private const string BaseUrl = "http://127.0.0.1:50000/API";

            public HTTP()
            {
                _client = new HttpClient();
                _client.Timeout = TimeSpan.FromSeconds(5);
            }

            public async Task<string> HandShake(Credential pCredential)
            {
                if (_client == null)
                {
                    return null;
                }
                string url = "http://127.0.0.1:50000/register";
                Dictionary<string, object> ServerResponse = await HttpRequest(Methods.POST, pCredential, url);
                return ServerResponse["Token"].ToString();
            }

            private async Task<Dictionary<string, object>> HttpRequest(Methods pMethod, object pData, string url = BaseUrl)
            {
                switch (pMethod)
                {
                    case Methods.GET:
                        string response = await _client.GetStringAsync(url);
                        return JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    
                    case Methods.POST:
                        string json = JsonConvert.SerializeObject(pData);
                        try
                        {
                            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                            HttpResponseMessage MessageResponse = await _client.PostAsync(url, content);
                            MessageResponse.EnsureSuccessStatusCode();
                            string JsonResponse = await MessageResponse.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonResponse);
                        }
                        catch (Exception ex)
                        {
                            return new Dictionary<string, object>()
                            {
                                { "error", ex.Message }
                            };
                        }
                    default:
                        throw new Exception("Method not supported. Use Methods.GET or Methods.POST");
                }
            }

            private async Task<Dictionary<string, object>> HttpRequest(Methods pMethod, object pData,string pToken, string url = BaseUrl)
            {
                switch (pMethod)
                {
                    case Methods.POST:
                        string json = JsonConvert.SerializeObject(pData);
                        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        content.Headers.Add("token", pToken);
                        HttpResponseMessage MessageResponse = await _client.PostAsync(url, content);
                        try
                        {
                            MessageResponse.EnsureSuccessStatusCode();
                            string JsonResponse = await MessageResponse.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonResponse);
                        }
                        catch (Exception ex)
                        {
                            return new Dictionary<string, object>()
                            {
                                { "error", ex.Message }
                            };
                        }
                    default:
                        throw new Exception("Method not supported. Use Methods.GET or Methods.POST");
                }
            }


        }

        //private class Websocket
        //{
        //    private ClientWebSocket _client;
        //    private const string BaseUrl = "ws://localhost/message";

        //    public Websocket()
        //    {
        //        _client = new ClientWebSocket();
        //    }

        //    public async Task<bool> OpenConnection()
        //    {
        //        await _client.ConnectAsync(new Uri(BaseUrl), CancellationToken.None);
        //        _client.Options.
        //    }
        //}
    }
}
