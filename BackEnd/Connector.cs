using BackEnd.Security;
using BackEnd.API;
using BackEnd.User;
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
using System.Web;
using static System.Net.WebRequestMethods;

namespace BackEnd.Connection
{
    /// <summary>
    /// Class that handle all network related request. It can send http and WebSocket requests
    /// </summary>
    public class Connector
    {
        // private instance of the HTTP class that handle all http related request
        private HTTP _httpClient;
        // private jwt token sent by the server after authentification (HTTP handshake)
        private string _token;
        // private bool that indicated if the initial hand check with the server happened and
        // and that the user has a token 
        private bool _authenticated = default;
        // private string that server as buffer to hold message for API
        private Queue<string> Messages;
        // public event that is triggered when a message enter the queue of message
        public event EventHandler NewMessage;

        /// <summary>
        /// Class constructor. Initialize all parameters (HTTP, WebSocket etc..)
        /// </summary>
        public Connector()
        {
            _httpClient = new HTTP();
            _authenticated = false;
            Messages = new Queue<string>();
        }

        public bool IsAuthentified
        {
            get
            {
                return _authenticated;
            }
        }

        public string message
        {
            get
            {
                if(Messages.Count == 0)
                {
                    return string.Empty;
                }
                return Messages.Dequeue();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Messages.Enqueue(value);
                    NewMessage?.Invoke(this, EventArgs.Empty);
                }
            }   
        }

        

        /// <summary>
        /// Method that handle the user authentification. It send the information wrap in 
        /// the credential class to the server and wait for his response in the form of a bool.
        /// 
        /// </summary>
        /// <param name="pCredential"></param>
        /// <returns>The server authentification response as a boolean</returns>
        public async Task Authentification(Credential pCredential, HandShake pType)
        {
            // Check if the HttpClient is not null
            if(_httpClient is null)
            {
                return;
            }
            // Check if the user already has a token
            if (!string.IsNullOrEmpty(_token))
            {
                return;
            }


            // dictionary to hold the future server response
            Dictionary<string, object> response = await _httpClient.ServerHandShake(pCredential, pType);

            // we check the code of the server to know if the token is created
            int code = int.Parse(response["Code"].ToString());
            switch (code)
            {
                case 30:
                    string token = response["Token"].ToString();
                    _token = token;
                    message = $"Connection successful, Welcome {response["Content"]}";
                    _authenticated = true;
                    _httpClient.SetHeader(_token);
                    API.API._user = new Person(response["Content"].ToString());
                    break;
                case 35:
                    _authenticated = false;
                    message = response["Content"].ToString();
                    break;
                
            }
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
                _client.Timeout = TimeSpan.FromSeconds(20);
            }

            public bool SetHeader(string pToken)
            {
                _client.DefaultRequestHeaders.Add("tk",  pToken);
                return true;
            }

            public async Task<Dictionary<string,object>> UserFriends()
            {
                string URL = "http://127.0.0.1/Friends";
                string n = string.Empty;
                Dictionary<string, object> data = await HttpRequest(Methods.GET, n, URL);
                return data;
            }

            /// <summary>
            /// Send the user's information to the server for registration and wait for jwt token from the server or 
            /// an error message if the user is already register to the server.
            /// </summary>
            /// <param name="pCredential">User's information wrap in the credential class</param>
            /// <returns>string that represent the jwt token from the server or an error message</returns>
            public async Task<Dictionary<string,object>> ServerHandShake(Credential pCredential, HandShake pType)
            {
                // check is the HttpClient is not null
                if (_client == null)
                {
                    return null;
                }
                // url to the server
                string url = "http://127.0.0.1:50000/";
                Dictionary<string, object> ServerResponse = new Dictionary<string, object>();

                switch (pType)
                {
                    case HandShake.REGISTRATION:
                        // Send the user's information to the server using the private methods HttpRequest with a POST argument
                        // Wait for the response as a Dict<string, object>. 
                        url = url + "register";
                        ServerResponse = await HttpRequest(Methods.POST, pCredential, url);
                        return ServerResponse;

                    case HandShake.CONNEXION:
                        // Send the user's information to the server using the private methods HttpRequest with a POST argument
                        // Wait for the response as a Dict<string, object>. 
                        url = url + "connexion";
                        ServerResponse = await HttpRequest(Methods.POST, pCredential, url);
                        return ServerResponse;
                    default:
                        return ServerResponse;
                }
                
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
                            Dictionary<string, object> nResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonResponse);
                            if(MessageResponse.Headers.TryGetValues("tk", out IEnumerable<string> token)){
                                nResponse.Add("Token", token);
                            }
                            
                                return nResponse;
                        }
                        catch (Exception ex)
                        {
                            return new Dictionary<string, object>()
                            {
                                { "Content ", ex.Message },
                                {"Code", 100}
                            };
                        }
                    default:
                        throw new Exception("Method not supported. Use Methods.GET or Methods.POST");
                }
            }
        }

        //private class Socket
        //{
        //    private ClientWebSocket _client;
        //    private const string BaseUrl = "ws://localhost/message";

        //    public Socket()
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
