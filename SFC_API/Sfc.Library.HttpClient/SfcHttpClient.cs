using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.Helpers;
using Sfc.Library.HttpClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sfc.Library.HttpClient
{
    public class SfcHttpClient : IDisposable
    {

        #region Constants
        #endregion
        #region fields

        private const string authPath = "/api/auth/token";
        private string _baseUrl;
        private string _clientId;
        private string _clientSecret;
        private string _db_key;
        private System.Net.Http.HttpClient _httpClient;
        private AccessTokenResponse _accessTokenResponse;
        private Timer _timer;
        private bool _isRunning;
        private int _interval = 60 * 1000; //
        #endregion
        #region Properties

        public string BaseUrl
        {
            get
            {
                return this._baseUrl;
            }

            set
            {
                this._baseUrl = value;
            }
        }

        public string ClientId
        {
            get
            {
                return this._clientId;
            }
            set
            {
                this._clientId = value;
            }
        }

        public string ClientSecret
        {
            get
            {
                return this._clientSecret;
            }
            set
            {
                this._clientSecret = value;
            }
        }
        public string DbKey
        {
            get
            {
                return this._db_key;
            }
            set
            {
                this._db_key = value;
            }
        }
        public System.Net.Http.HttpClient HttpClient
        {
            get
            {
                if(this._httpClient == null)
                {
                    this._httpClient = new System.Net.Http.HttpClient();
                }
                return this._httpClient;
            }
        }

        public AccessTokenResponse AccessTokenResponse
        {
            get
            {
                return this._accessTokenResponse;
            }
            set
            {
                this._accessTokenResponse = value;
            }
        }

        private bool _autoRefreshToken;
        public bool AutoRefreshToken
        {
            get 
            {
                return this._autoRefreshToken;   
            }
            set
            {
                this._autoRefreshToken = value;
            }
        }
        #endregion
        public SfcHttpClient(string baseUrl, string dbKey, string clientId, string clientSecret, bool autoRefreshToken = true)
        {
            this._baseUrl = baseUrl;
            this._db_key = dbKey;
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._httpClient = new System.Net.Http.HttpClient();
            this._autoRefreshToken = autoRefreshToken;

            if(AutoRefreshToken)
            {
                this.InitTimer();
            }
            
        }
        public async Task GetAccessTokenAsync(string username, string password)
        {
            try
            {

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + authPath))
                {
                    var values = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(Constants.Parameters.GrantType, Constants.GrantTypes.Password),
                            new KeyValuePair<string, string>(Constants.Parameters.Username, username),
                            new KeyValuePair<string, string>(Constants.Parameters.Password, password),
                            //new KeyValuePair<string, string>(Constants.Parameters.ClientId, "helloApp"),
                            //new KeyValuePair<string, string>(Constants.Parameters.ClientSecret, "123456"),
                            new KeyValuePair<string, string>(Constants.Parameters.DbKey, this._db_key),
                        };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", StringConvertHelper.EncodeToBase64(string.Format("{0}:{1}", this._clientId, this._clientSecret)));
                    request.Content = new FormUrlEncodedContent(values);
                    
                    using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                    {
                        //response.EnsureSuccessStatusCode();
                        //var result = await response.Content.ReadAsStringAsync();



                        var result = await response.Content.ReadAsAsync<AccessTokenResponseModel>();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (result.Data != null)
                            {
                               
                                _accessTokenResponse = result.Data;
                            } else
                            {
                                throw new Exception("No access token found");
                            }

                        } else
                        {
                            throw new Exception(result.Message);
                        }

                        
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task GetRefreshTokenAsync(string refreshToken)
        {


            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + authPath))
            {
                var values = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>(Constants.Parameters.GrantType, Constants.GrantTypes.RefreshToken),
                    new KeyValuePair<string, string>(Constants.Parameters.RefreshToken, refreshToken),
                    new KeyValuePair<string, string>(Constants.Parameters.DbKey, this._db_key)
                };

                // client_id and client_secret: http://tools.ietf.org/html/rfc6749#section-2.3.1
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", StringConvertHelper.EncodeToBase64(string.Format("{0}:{1}", this._clientId, this._clientSecret)));
                request.Content = new FormUrlEncodedContent(values);
                using (HttpResponseMessage response = await HttpClient.SendAsync(request))
                {
                    //response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsAsync<AccessTokenResponseModel>();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (result.Data != null)
                        {

                            _accessTokenResponse = result.Data;
                        }
                        else
                        {
                            throw new Exception("No access token found");
                        }

                    }
                    else
                    {
                        throw new Exception(result.Message);
                    }


                }
            }
        }

        public async Task<ResponseModelList> ExecuteAsync(QuerySingleParameterModel querySingleParameterModel)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/execute"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessTokenResponse.AccessToken);

                var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.Indented);



                //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                {
                    //response.EnsureSuccessStatusCode();
                    var results = await response.Content.ReadAsAsync<ResponseModelList>();
                    return results;
                    //var result = await response.Content.ReadAsStringAsync();
                }
            }
        }
        public async Task<ResponseModelSingle> QuerySingleAsync(QuerySingleParameterModel querySingleParameterModel)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/querysingle"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessTokenResponse.AccessToken);

                var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.Indented);

                

                //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                {
                    //response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsAsync<ResponseModelSingle>();
                    return result;
                    //var result = await response.Content.ReadAsStringAsync();
                }
            }
        }
        public async Task<ResponseModelList> QueryListAsync(QuerySingleParameterModel querySingleParameterModel)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/querylist"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessTokenResponse.AccessToken);

                var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.Indented);



                //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                {
                    //response.EnsureSuccessStatusCode();
                    var results = await response.Content.ReadAsAsync<ResponseModelList>();
                    return results;
                    //var result = await response.Content.ReadAsStringAsync();
                }
            }
        }
        //public async Task<ResponseModelSingle> QuerySingleAsyn(string sqlCommand)
        //{
        //    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/querysingle"))
        //    {
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessTokenResponse.AccessToken);
        //        JObject jObject = new JObject();
        //        jObject.Add("commandText", sqlCommand);
        //        //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
        //        request.Content = new StringContent(jObject.ToString(Newtonsoft.Json.Formatting.None), Encoding.UTF8, Constants.MediaType.ApplicationJson);
        //        using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
        //        {
        //            response.EnsureSuccessStatusCode();
        //            var result = await response.Content.ReadAsAsync<ResponseModelSingle>();
        //            return result;
        //            //var result = await response.Content.ReadAsStringAsync();
        //        }
        //    }
        //}
        //public async Task<ResponseModelList> QueryAsyn(string sqlCommand)
        //{
        //    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/queryex"))
        //    {
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessTokenResponse.AccessToken);
        //        JObject jObject = new JObject();
        //        jObject.Add("commandText", sqlCommand);
        //        //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
        //        request.Content = new StringContent(jObject.ToString(Newtonsoft.Json.Formatting.None), Encoding.UTF8, Constants.MediaType.ApplicationJson);
        //        using (HttpResponseMessage response = await  this._httpClient.SendAsync(request))
        //        {
        //            response.EnsureSuccessStatusCode();
        //            var result = await response.Content.ReadAsAsync<ResponseModelList>();
        //            return result;
        //            //var result = await response.Content.ReadAsStringAsync();
        //        }
        //    }
        //}

        #region Timer
        private void InitTimer()
        {
            if (this._timer == null)
            {
                this._timer = new Timer(new TimerCallback(this.TimerHandler), null, this._interval, this._interval);
            }
        }
        private async void Run()
        {
            try
            {

                if (_accessTokenResponse != null)
                {
                    var now = DateTime.Now;
                    var totalMinute = (_accessTokenResponse.Expires - now).TotalMinutes;
                    if(totalMinute - 1 <= 0)
                    {
                        await this.GetRefreshTokenAsync(_accessTokenResponse.RefreshToken);
                    }
                    
                }
            }
            catch
            {

            }
        }
        private void TimerHandler(object state)
        {

            this._timer.Change(-1, -1);
            this.Run();
            if (this._timer != null) // In case dispose
            {
                this._timer.Change(this._interval, this._interval);
            }
        }
        #endregion
        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
            if (this._timer != null)
            {
                lock (this)
                {
                    this._timer.Dispose();
                    this._timer = null;
                }
            }
        }

    }
}
