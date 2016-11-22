using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BackandRestClient
{
    public class BackandClient
    {
        const string BACKAND_URL = "https://api.backand.com/";

        RestClient client = null;
        public BackandClient(string accessToken = null, string anonymousToken = null)
        {
            if (accessToken != null)
            {
                client = GetRestClient();
                SetAccessToken(accessToken);
            }
            else if (anonymousToken != null)
            {
                client = GetRestClient();
                SetAnonymousToken(anonymousToken);
            }
        }

        public BackandClient(LoginResult loginResult)
            : this(loginResult.token_type + " " + loginResult.access_token)
        {
        }

        private RestClient GetRestClient()
        {
            return new RestClient(BACKAND_URL);
        }

        private void SetAccessToken(string accessToken)
        {
            client.AddDefaultHeader("Authorization", accessToken);
        }
        private void SetAnonymousToken(string anonymousToken)
        {
            client.AddDefaultHeader("AnonymousToken", anonymousToken);
        }

        private LoginResult SignInInner(string username, string password, string appName)
        {
            var request = new RestRequest("token", Method.POST);
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            request.AddParameter("appname", appName);
            request.AddParameter("grant_type", "password");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            var response = GetRestClient().Execute<LoginResult>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            { 
                return response.Data; 
            }
            else
            {
                throw new BeckandClientException(response);
            }
        }


        public LoginResult SignIn(string appName, string username, string password)
        {
            client = GetRestClient();

            var res = SignInInner(username, password, appName);
            SetAccessToken(res.token_type + " " + res.access_token);
            return res;
        }

        public List<T> GelList<T>(string name, out int? totalRows, bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, object filter = null, object sort = null, string search = null, bool? deep = null, bool? descriptive = true, bool? relatedObjects = false) where T : new()
        {
            var request = new RestRequest("/1/objects/{name}", Method.GET);
            request.AddUrlSegment("name", name);
            if (withFilterOptions.HasValue)
                request.AddParameter("withSelectOptions", withSelectOptions.Value, ParameterType.QueryString);
            if (withFilterOptions.HasValue)
                request.AddParameter("withFilterOptions", withFilterOptions.Value, ParameterType.QueryString);
            if (pageNumber.HasValue)
                request.AddParameter("pageNumber", pageNumber.Value, ParameterType.QueryString);
            if (pageSize.HasValue)
                request.AddParameter("pageSize", pageSize.Value, ParameterType.QueryString);
            if (filter != null)
                request.AddParameter("filter", JsonConvert.SerializeObject(filter), ParameterType.QueryString);
            if (sort != null)
                request.AddParameter("sort", JsonConvert.SerializeObject(sort), ParameterType.QueryString);
            if (search != null)
                request.AddParameter("search", search, ParameterType.QueryString);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (descriptive.HasValue)
                request.AddParameter("descriptive", descriptive.Value, ParameterType.QueryString);
            if (relatedObjects.HasValue)
                request.AddParameter("relatedObjects", relatedObjects.Value, ParameterType.QueryString);

            var response = client.Execute<GelListResult<T>>(request);

            totalRows = response.Data.totalRows;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data.data;
            }
            else
            {
                throw new BeckandClientException(response);
            }
        }

        public T GetOne<T>(string name, string id, bool? deep = null, int? level = null) where T : new()
        {
            var request = new RestRequest("/1/objects/{name}/{id}", Method.GET);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (level.HasValue)
                request.AddParameter("level", level.Value, ParameterType.QueryString);

            var response = client.Execute<T>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }
            else
            {
                throw new BeckandClientException(response);
            }
        }

        public T Post<T>(string name, T data, out string id, bool? deep = null, bool? returnObject = null, string parameters = null) where T : new()
        {
            var request = new RestRequest("/1/objects/{name}", Method.POST);
            request.AddUrlSegment("name", name);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (returnObject.HasValue)
                request.AddParameter("returnObject", returnObject.Value, ParameterType.QueryString);
            if (parameters != null)
                request.AddParameter("parameters", parameters, ParameterType.QueryString);

            var response = client.Execute<T>(request);

            id = null;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
                if (result.ContainsKey("__metadata"))
                {
                    Newtonsoft.Json.Linq.JToken metadata = (Newtonsoft.Json.Linq.JToken)result["__metadata"];
                    id = metadata["id"].ToString();
                    
                }
                if (returnObject.HasValue && returnObject.Value)
                {
                    return response.Data;
                }
                return default(T);
            }
            else
            {
                throw new BeckandClientException(response);
            }
        }

        public T Put<T>(string name, string id, object data, bool? deep = null, bool? returnObject = null, string parameters = null, bool? overwrite = null) where T : new()
        {
            var request = new RestRequest("/1/objects/{name}/{id}", Method.PUT);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (returnObject.HasValue)
                request.AddParameter("returnObject", returnObject.Value, ParameterType.QueryString);
            if (parameters != null)
                request.AddParameter("parameters", parameters, ParameterType.QueryString);
            if (overwrite.HasValue)
                request.AddParameter("overwrite", overwrite.Value, ParameterType.QueryString);

            var response = client.Execute<T>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }
            else
            {
                throw new BeckandClientException(response);
            }
        }

        public void Delete(string name, string id, bool? deep = null, string parameters = null)
        {
            var request = new RestRequest("/1/objects/{name}/{id}", Method.DELETE);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (parameters != null)
                request.AddParameter("parameters", parameters, ParameterType.QueryString);

            var response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new BeckandClientException(response);
            }
        }
    }

    public class LoginResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string appName { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string fullName { get; set; }
        public string userId { get; set; }
    }

    public class GelListResult<T>
    {
        public int totalRows { get; set; }
        public List<T> data { get; set; }
    }

    public class BeckandClientException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public BeckandClientException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public BeckandClientException(IRestResponse response)
            : this(response.Content, response.StatusCode)
        {
            
        }
    }

}
