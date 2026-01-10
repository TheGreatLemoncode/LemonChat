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

namespace BackEnd.Connection
{
    /// <summary>
    /// Enum that represent the http request type
    /// </summary>
    public enum Methods
    {
        POST,
        GET
    }

    /// <summary>
    /// Class that handle all network related request. It can send http and WebSocket requests
    /// </summary>
    public class Connector
    {
        // private instance of the HTTP class that handle all http related request
        private HTTP _httpClient;
        // private jwt token sent by the server after authentification (HTTP handshake)
        private string _token;

        /// <summary>
        /// Class constructor. Initialize all parameters (HTTP, WebSocket etc..)
        /// </summary>
        public Connector()
        {
            _httpClient = new HTTP();
        }

        /// <summary>
        /// Method that handle the user authentification. It send the information wrap in 
        /// the credential class to the server and wait for his response in the form of a bool.
        /// </summary>
        /// <param name="pCredential"></param>
        /// <returns>The server authentification response as a boolean</returns>
        public async Task<bool> Authentification(Credential pCredential)
        {
            // Check if the HttpClient is not null
            if(_httpClient is null)
            {
                return false;
            }
            // Check if the user already has a token
            if (!string.IsNullOrEmpty(_token))
            {
                return true;
            }
            // Give the information to the HTTP class and wait for the server response that is the token
            string Token = await _httpClient.RegistrationHandShake(pCredential);
            // if the response is null or empty, this mean there is an error somewhere and return false
            if (string.IsNullOrEmpty(Token))
            {
                return false;
            }
            // Give the token to the connector to be used in future communication with the server then return 
            // true
            _token = Token;
            return true;
        } 

        /// <summary>
        /// Private class to the Connector. His role is to handle all http communication with the server. It contains 
        /// a HttpClient object and a base Url in case something is wrong
        /// </summary>
        private class HTTP
        {
            // private instance of the HttpClient class. His role is to send request and receive server responses
            private HttpClient _client;
            private const string BaseUrl = "http://127.0.0.1:50000/API";

            /// <summary>
            /// Class constructor. Initialize the http client and give it a time span of 7 seconds
            /// </summary>
            public HTTP()
            {
                _client = new HttpClient();
                _client.Timeout = TimeSpan.FromSeconds(7);
            }

            /// <summary>
            /// Send the user's information to the server for registration and wait for jwt token from the server or 
            /// an error message if the user is already register to the server.
            /// </summary>
            /// <param name="pCredential">User's information wrap in the credential class</param>
            /// <returns>string that represent the jwt token from the server or an error message</returns>
            public async Task<string> RegistrationHandShake(Credential pCredential)
            {
                // check is the HttpClient is not null
                if (_client == null)
                {
                    return null;
                }
                // url to the server
                string url = "http://127.0.0.1:50000/register";
                // Send the user's information to the server using the private methods HttpRequest with a POST argument
                // Wait for the response as a Dict<string, object>. 
                Dictionary<string, object> ServerResponse = await HttpRequest(Methods.POST, pCredential, url);
                // Since the server can ever send the token or an error message we need an if tree 
                if(ServerResponse.TryGetValue("Token", out object Token))
                {
                    return Token.ToString();
                }
                else
                {
                    throw new Exception($"The server didn't respond with a token \n Message : {ServerResponse["message"]}");
                }
                    // Extract the token from the response and return it.
                    return ServerResponse["Token"].ToString();
            }

            /// <summary>
            /// Private method that can send and receive data from the server and return it's response. It can POST and GET 
            /// depending of the given argument.
            /// </summary>
            /// <param name="pMethod">An enum that represent the http request type to be execute</param>
            /// <param name="pData">The data to be send to the server</param>
            /// <param name="url">Destination url to the server in string</param>
            /// <returns>A dictionary that contains the response from the server</returns>
            /// <exception cref="Exception"></exception>
            private async Task<Dictionary<string, object>> HttpRequest(Methods pMethod, object pData, string url = BaseUrl)
            {
                // Check the method
                switch (pMethod)
                {
                    // If the method is GET, we wait for the json response from the server, deserialize then return it.
                    case Methods.GET:
                        string response = await _client.GetStringAsync(url);
                        return JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    
                    // If the method is POST, the data is first serialize.
                    case Methods.POST:
                        string json = JsonConvert.SerializeObject(pData);
                        try
                        {
                            // It's then wrap in a http content and the header are set.
                            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                            // The data is then sent and we wait for the response.
                            HttpResponseMessage MessageResponse = await _client.PostAsync(url, content);
                            // We check the response status code.
                            MessageResponse.EnsureSuccessStatusCode();
                            // If it's all right, we read the content of the response, deserialize and then return it.
                            string JsonResponse = await MessageResponse.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonResponse);
                        }
                        catch (Exception ex)
                        {
                            return new Dictionary<string, object>()
                            {
                                { "message ", ex.Message }
                            };
                        }
                    default:
                        throw new Exception("Method not supported. Use Methods.GET or Methods.POST");
                }
            }

            /// <summary>
            /// Private method that can send and receive data from the server with an authentification token and return it's response. It can POST and GET 
            /// depending of the given argument. Use only to communicate with endpoint that require authentification
            /// </summary>
            /// <param name="pMethod">An enum that represent the http request type to be execute</param>
            /// <param name="pData">The data to be send to the server</param>
            /// <param name="url">Destination url to the server in string</param>
            /// <param name="pToken">string that represent the user jwt token</param>
            /// <returns>A dictionary that contains the response from the server</returns>
            /// <exception cref="Exception"></exception>
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
