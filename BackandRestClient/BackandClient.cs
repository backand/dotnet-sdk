using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackandRestClient
{
    public class BackandClient
    {
        const string BACKAND_URL = "https://api.backand.com/";

        RestClient client;
        public BackandClient(string appName, string username, string password)
        {
            client = GetAuthentificatedClient(appName, username, password);
        }

        private RestClient GetRestClient()
        {
            RestClient client = new RestClient(BACKAND_URL);

            return client;
        }

        private LoginResult SignIn(string username, string password, string appName)
        {
            var request = new RestRequest("token", Method.POST);
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            request.AddParameter("appname", appName);
            request.AddParameter("grant_type", "password");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            var response = GetRestClient().Execute<LoginResult>(request).Data;
            return response;
        }

       
        private RestClient GetAuthentificatedClient(string appName, string username, string password)
        {
            var rest = GetRestClient();

            var res = SignIn(username, password, appName);
            rest.AddDefaultHeader("Authorization", res.token_type + " " + res.access_token);
            Console.WriteLine(res.access_token);
            return rest;
        }

        public IRestResponse GelAll(string name, bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, object filter = null, object sort = null, string search = null, bool? deep = null, bool? descriptive = true, bool? relatedObjects = false)
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

            var response = client.Execute(request);
            
            return response;
        }

        public IRestResponse GetOne(string name, string id, bool? deep = null, int? level = null)
        {
            var request = new RestRequest("/1/objects/{name}/{id}", Method.GET);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (level.HasValue)
                request.AddParameter("level", level.Value, ParameterType.QueryString);

            var response = client.Execute(request);
            return response;
        }

        public IRestResponse Post<T>(string name, T data, bool? deep = null, bool? returnObject = null, string parameters = null)
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

            var response = client.Execute(request);
            
            return response;
        }

        public IRestResponse Put(string name, string id, object data, bool? deep = null, bool? returnObject = null, string parameters = null, bool? overwrite = null)
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

            var response = client.Execute(request);

            return response;

        }

        public IRestResponse Delete(string name, string id, bool? deep = null, string parameters = null)
        {
            var request = new RestRequest("/1/objects/{name}/{id}", Method.DELETE);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (parameters != null)
                request.AddParameter("parameters", parameters, ParameterType.QueryString);

            var response = client.Execute(request);

            return response;
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

}
