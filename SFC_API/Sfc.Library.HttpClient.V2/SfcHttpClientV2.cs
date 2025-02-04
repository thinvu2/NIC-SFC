using Newtonsoft.Json;
using Sfc.Core.Helpers;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.V2.Logging;
using Sfc.Library.HttpClient.V2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sfc.Library.HttpClient.V2
{
    public class SfcHttpClientV2 : IDisposable
    {
        #region Fields
        private string _baseUrl;
        private System.Net.Http.HttpClient _httpClient;
        private IniFile _iniFile;
        
        private Logger _logger;
        private string _db_key;
        private string _connectionString;
        private string _clientId;
        private string _clientSecret;
        private Timer _timer;
        //private int _interval = 60 * 1000; //
        private int _interval = 1 * 1000; //
        #endregion
        #region Properties
        public bool Logging { get; set; } = true;

        public int LogAvailabelDays { get; set; } = 15; //Default 15 days
        public bool IsConsole { get; set; } = false;

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
        private string LogPath
        {
            get
            {
                string path = Directory.GetCurrentDirectory();
                path = Path.Combine(path, "logs");
                return path;
            }
        }
        private string LogFileName
        {
            get
            {

                if (!Directory.Exists(this.LogPath))
                {
                    Directory.CreateDirectory(this.LogPath);
                }
                string fileName = this._clientId + "_" + DateTime.Now.ToString("yyyy-MM-dd");
                return Path.Combine(this.LogPath, fileName);
            }
        }
        public System.Net.Http.HttpClient HttpClient
        {
            get
            {
                if (this._httpClient == null)
                {
                    this._httpClient = new System.Net.Http.HttpClient();
                }
                return this._httpClient;
            }
        }
        #endregion
        #region Ctor
        /// <summary>
        /// SfcHttpClientV2
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="dbKey"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public SfcHttpClientV2(string baseUrl, string dbKey, string clientId, string clientSecret)
        {
            _iniFile = new IniFile(Sfc.Core.Constants.API_CONFIG_FILE_NAME);
            if (string.IsNullOrEmpty(baseUrl))
            {
                this._baseUrl = _iniFile.Read(Sfc.Core.Constants.API_BASE_URL_KEY, Sfc.Core.Constants.API_BASE_URL_SECTION); ;
            }
            else
            {
                this._baseUrl = baseUrl;
            }
            if (string.IsNullOrEmpty(dbKey))
            {
                this._db_key = _iniFile.Read(Sfc.Core.Constants.API_DB_KEY, Sfc.Core.Constants.API_BASE_URL_SECTION); ;
            }
            else
            {
                this._db_key = dbKey;
            }
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._httpClient = new System.Net.Http.HttpClient();
            this._logger = new Logger(this.LogFileName);


            if (this.Logging)
            {
                this.InitTimer();
            }


        }
        /// <summary>
        /// SfcHttpClientV2
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public SfcHttpClientV2(string clientId, string clientSecret)
        {

            _iniFile = new IniFile(Sfc.Core.Constants.API_CONFIG_FILE_NAME);
            this._baseUrl = _iniFile.Read(Sfc.Core.Constants.API_BASE_URL_KEY, Sfc.Core.Constants.API_BASE_URL_SECTION);
            this._db_key = _iniFile.Read(Sfc.Core.Constants.API_DB_KEY, Sfc.Core.Constants.API_BASE_URL_SECTION);
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._httpClient = new System.Net.Http.HttpClient();
            this._logger = new Logger(this.LogFileName);


            if (this.Logging)
            {
                this.InitTimer();
            }


        }
        #endregion
        #region Timer
        private void InitTimer()
        {
            if (this._timer == null)
            {
                this._timer = new Timer(new TimerCallback(this.TimerHandler), null, this._interval, this._interval);
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

        private void Run()
        {
            var now = DateTime.Now;
            try
            {
                //_logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                if (IsConsole)
                {
                    Console.WriteLine(now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                DirectoryInfo di = new DirectoryInfo(this.LogPath);
                foreach (FileInfo fi in di.GetFiles())
                {
                    var totalDays = (DateTime.Now - fi.CreationTime).TotalDays;
                    if (totalDays > this.LogAvailabelDays)
                    {
                        if (IsConsole)
                        {
                            Console.WriteLine("[{0}] : {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), "Delete file " + fi.FullName);
                        }
                        fi.Delete();
                    }
                }

            }
            catch (Exception ex)
            {
                if (IsConsole)
                {
                    Console.WriteLine("[{0}] : {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message);
                }
            }
        }
        #endregion
        #region ExecuteList
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="querySingleParameterModels"></param>
        /// <returns></returns>
        public ResponseModelCommandList Execute(List<QuerySingleParameterModel> querySingleParameterModels)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network


                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/executelist"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModels, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][ExecuteList][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ResponseModelCommandList>(jsonString);
                        // var result = response.Content.ReadAsAsync<ResponseModelCommandList>().Result;
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][ExecuteList][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][ExecuteList][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        /// <summary>
        /// ExecuteAsync
        /// </summary>
        /// <param name="querySingleParameterModels"></param>
        /// <returns></returns>
        public async Task<ResponseModelCommandList> ExecuteAsync(List<QuerySingleParameterModel> querySingleParameterModels)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/executelist"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());


                    var json = JsonConvert.SerializeObject(querySingleParameterModels, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][ExecuteListAsync][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<ResponseModelCommandList>(jsonString);
                        //var result = await response.Content.ReadAsAsync<ResponseModelCommandList>();
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][ExecuteListAsync][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][ExecuteListAsync][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        #endregion
        #region ExecuteThrowExceptionWhenNoRowsAffected
        /// <summary>
        /// ExecuteThrowExceptionWhenNoRowsAffected
        /// </summary>
        /// <param name="querySingleParameterModels"></param>
        /// <returns></returns>
        public ResponseModelCommandList ExecuteThrowExceptionWhenNoRowsAffected(List<QuerySingleParameterModel> querySingleParameterModels)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network


                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/execute-throw-exception-when-no-rows-affected"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModels, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][ExecuteThrowExceptionWhenNoRowsAffected][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ResponseModelCommandList>(jsonString);
                        // var result = response.Content.ReadAsAsync<ResponseModelCommandList>().Result;
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][ExecuteThrowExceptionWhenNoRowsAffected][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][ExecuteThrowExceptionWhenNoRowsAffected][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        /// <summary>
        /// ExecuteThrowExceptionWhenNoRowsAffectedAsync
        /// </summary>
        /// <param name="querySingleParameterModels"></param>
        /// <returns></returns>
        public async Task<ResponseModelCommandList> ExecuteThrowExceptionWhenNoRowsAffectedAsync(List<QuerySingleParameterModel> querySingleParameterModels)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/execute-throw-exception-when-no-rows-affected"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());


                    var json = JsonConvert.SerializeObject(querySingleParameterModels, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][ExecuteThrowExceptionWhenNoRowsAffectedAsync][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<ResponseModelCommandList>(jsonString);
                        //var result = await response.Content.ReadAsAsync<ResponseModelCommandList>();
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][ExecuteThrowExceptionWhenNoRowsAffectedAsync][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][ExecuteThrowExceptionWhenNoRowsAffectedAsync][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        #endregion
        #region Execute

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public ResponseModelList Execute(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/execute"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][Execute][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ResponseModelList>(jsonString);
                        //var result = response.Content.ReadAsAsync<ResponseModelList>().Result;
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][Execute][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][Execute][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        /// <summary>
        /// ExecuteAsync
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public async Task<ResponseModelList> ExecuteAsync(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/execute"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));


                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());

                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][ExecuteAsync][Request] => " + json);
                    }

                    using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                    {
                        //response.EnsureSuccessStatusCode();
                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<ResponseModelList>(jsonString);
                        //var result = await response.Content.ReadAsAsync<ResponseModelList>();
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][ExecuteAsync][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][ExecuteAsync][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        #endregion
        #region DeriveParametersAsync
        /// <summary>
        /// DeriveParameters
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public ResponseModelListGeneric<List<SfcParameter>> DeriveParameters(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/deriveparameters"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][DeriveParameters][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ResponseModelListGeneric<List<SfcParameter>>>(jsonString);
                        //var result = response.Content.ReadAsAsync<ResponseModelList>().Result;
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][DeriveParameters][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][DeriveParameters][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }

        /// <summary>
        /// DeriveParametersAsync
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public async Task<ResponseModelListGeneric<List<SfcParameter>>> DeriveParametersAsync(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/deriveparameters"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));


                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());

                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][DeriveParametersAsync][Request] => " + json);
                    }

                    using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                    {
                        //response.EnsureSuccessStatusCode();
                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<ResponseModelListGeneric<List<SfcParameter>>>(jsonString);
                        //var result = await response.Content.ReadAsAsync<ResponseModelList>();
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][DeriveParametersAsync][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][DeriveParametersAsync][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        #endregion
        #region ExecuteRefcursor
        /// <summary>
        /// ExecuteRefcursor
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public ResponseModelList ExecuteRefcursor(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/executerefcursor"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));
                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][ExecuteRefcursor][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ResponseModelList>(jsonString);
                        //var result = response.Content.ReadAsAsync<ResponseModelList>().Result;
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][ExecuteRefcursor][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][ExecuteRefcursor][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        /// <summary>
        /// ExecuteRefcursorAsync
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public async Task<ResponseModelList> ExecuteRefcursorAsync(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network



                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/executerefcursor"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));


                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());

                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][ExecuteRefcursorAsync][Request] => " + json);
                    }

                    using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                    {
                        //response.EnsureSuccessStatusCode();
                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<ResponseModelList>(jsonString);
                        //var result = await response.Content.ReadAsAsync<ResponseModelList>();
                        if (this.Logging)
                        {
                            _logger.Info($"[{ this._baseUrl}][ExecuteRefcursorAsync][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][ExecuteRefcursorAsync][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        #endregion
        #region QuerySingle
        /// <summary>
        /// QuerySingle
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public ResponseModelSingle QuerySingle(QuerySingleParameterModel querySingleParameterModel)
        {



            try
            {

                VerifyNetworkAvailable(); //Verify network


                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/querysingle"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));

                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);

                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][QuerySingle][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ResponseModelSingle>(jsonString);
                        //var result = response.Content.ReadAsAsync<ResponseModelSingle>().Result;
                        if (this.Logging)
                        {
                            _logger.Info($"[{this._baseUrl}][QuerySingle][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }

                        return result;
                        //var result = await response.Content.ReadAsStringAsync();

                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][QuerySingle][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        /// <summary>
        /// QuerySingleAsync
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public async Task<ResponseModelSingle> QuerySingleAsync(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network


                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/querysingle"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));


                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);

                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][QuerySingleAsync][Request] => " + json);
                    }

                    using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                    {
                        //response.EnsureSuccessStatusCode();
                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<ResponseModelSingle>(jsonString);
                        //var result = await response.Content.ReadAsAsync<ResponseModelSingle>();
                        if (this.Logging)
                        {
                            _logger.Info($"[{this._baseUrl}][QuerySingleAsync][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][QuerySingleAsync][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        #endregion
        #region QueryList
        /// <summary>
        /// QuerySingleAsync
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public ResponseModelList QueryList(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/querylist"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));

                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][QueryList][Request] => " + json);
                    }

                    using (HttpResponseMessage response = this._httpClient.SendAsync(request).Result)
                    {
                        //response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<ResponseModelList>(jsonString);
                        //var result = response.Content.ReadAsAsync<ResponseModelList>().Result;
                        if (this.Logging)
                        {
                            _logger.Info($"[{this._baseUrl}][QueryList][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][QueryList][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        /// <summary>
        /// QueryListAsync
        /// </summary>
        /// <param name="querySingleParameterModel"></param>
        /// <returns></returns>
        public async Task<ResponseModelList> QueryListAsync(QuerySingleParameterModel querySingleParameterModel)
        {

            try
            {
                VerifyNetworkAvailable(); //Verify network

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, this._baseUrl + "/api/v2/querylist"))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.MediaType.ApplicationJson));

                    if (!string.IsNullOrEmpty(this._db_key))
                    {
                        request.Headers.Add("dbKey", this._db_key);
                    }
                    else
                    {
                        request.Headers.Add("connectionString", this._connectionString);
                    }
                    request.Headers.Add("clientId", this._clientId);
                    request.Headers.Add("clientSecret", this._clientSecret);
                    request.Headers.Add("clientVersion", GetCurrentAssemblyVersion());
                    var json = JsonConvert.SerializeObject(querySingleParameterModel, Formatting.None);



                    //request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    request.Content = new StringContent(json, Encoding.UTF8, Constants.MediaType.ApplicationJson);
                    if (this.Logging)
                    {
                        _logger.Info($"[{this._baseUrl}][QueryListAsync][Request] => " + json);
                    }
                    using (HttpResponseMessage response = await this._httpClient.SendAsync(request))
                    {
                        //response.EnsureSuccessStatusCode();
                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<ResponseModelList>(jsonString);
                        //var result = await response.Content.ReadAsAsync<ResponseModelList>();
                        if (this.Logging)
                        {
                            _logger.Info($"[{this._baseUrl}][QueryListAsync][Response] => " + JsonConvert.SerializeObject(result));
                        }

                        if (result != null && result.Result == ResponseModelResult.ERROR)
                        {
                            throw new Exception(result.Message);
                        }
                        return result;
                        //var result = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.Logging)
                {
                    _logger.Error($"[{ this._baseUrl}][QueryListAsync][Exception] => {ex.Message}");
                }
                throw ex;
            }
        }
        #endregion
        #region VerifyNetwork
        private void VerifyNetworkAvailable()
        {


            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.BaseUrl);

            // Set the credentials to the current user account
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.Method = "GET";

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Do nothing; we're only testing to see if we can get the response
                }
            }
            catch (WebException ex)
            {
                throw new Exception($"Network exception: {this.BaseUrl} => " + ex.Message);
            }

        }
        #endregion
        #region GetCurrentAssemblyVersion
        private string GetCurrentAssemblyVersion()
        {
            try
            {
                var type = this.GetType();
                var assembly = type.Assembly;
                var fullName = assembly.FullName;
                var splitedStrings = fullName.Split(',');
                var versionText = splitedStrings[1];
                var version = versionText.Substring(versionText.IndexOf("=") + 1, versionText.Length - (versionText.IndexOf("=") + 1));
                return version ?? "Unknow";
            }
            catch
            {
                return "Unknow";
            }
        }
        #endregion
        #region Ini Helper
        /// <summary>
        /// Read SFIS.ini File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public string SFISIniRead(string Key, string Section = null)
        {
            return _iniFile.Read(Key, Section);
        }
        /// <summary>
        /// Write SFIS.ini File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="Section"></param>
        public void SFISIniWrite(string Key, string Value, string Section = null)
        {
            _iniFile.Write(Key, Value, Section);
        }
        /// <summary>
        /// Delete Key in SFIS.ini File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        public void SFISIniDeleteKey(string Key, string Section = null)
        {
            _iniFile.DeleteKey(Key, Section);
        }
        /// <summary>
        /// Delete Section in SFIS.ini File
        /// </summary>
        /// <param name="Section"></param>
        public void SFISIniDeleteSection(string Section = null)
        {
            _iniFile.DeleteSection(Section);
        }
        /// <summary>
        /// Check Key Exist in SFIS.ini File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public bool SFISIniKeyExists(string Key, string Section = null)
        {
            return _iniFile.KeyExists(Key, Section);
        }
        #endregion
        #region Dispose
        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
        }
        #endregion
    }
}
