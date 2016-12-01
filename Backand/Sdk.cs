using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Backand
{
    /// <summary>
    /// This SDK enables you to communicate comfortably and quickly with your Backand app.
    /// </summary>
    public class Sdk
    {
        const string BACKAND_URL = "https://api.backand.com/";
        const string BEARER = "bearer";
        const char SPACE = ' ';
        const string AUTHORIZATION = "Authorization";
        const string ANONYMOUS_TOKEN = "AnonymousToken";
        
        RestClient client = null;
        
        private RestClient GetRestClient()
        {
            return new RestClient(BACKAND_URL);
        }
        /// <summary>
        /// When using Backand security, after a user signs in, he gets an OAuth2 access token that identifies him and you can use it to connect to Backand.
        /// </summary>
        /// <param name="accessToken">Use the Signin to get access token. You can put it in a cockie and reuse it to initiate Backand in every request.</param>
        public void SetOAuth2Token(string accessToken)
        {
            client = GetRestClient();
            if (!accessToken.ToLower().StartsWith(BEARER))
            {
                accessToken = BEARER + SPACE + accessToken;
            }
            client.AddDefaultHeader(AUTHORIZATION, accessToken);
        }

        /// <summary>
        /// Use this if you are not using Backand security.
        /// </summary>
        /// <param name="anonymousToken">Get the Anonymous Token from your Backand app. It is located at Security & Auth => Configuration</param>
        public void SetAnonymousToken(string anonymousToken)
        {
            client = GetRestClient();
            client.AddDefaultHeader(ANONYMOUS_TOKEN, anonymousToken);
        }

        private SignInResult SignInInner(string username, string password, string appName)
        {
            var request = new RestRequest("token", Method.POST);
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            request.AddParameter("appname", appName);
            request.AddParameter("grant_type", "password");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            var response = GetRestClient().Execute<SignInResult>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            { 
                return response.Data; 
            }
            else
            {
                throw new BeckandException(response);
            }
        }

        /// <summary>
        /// This methods returns OAuth2 access token and other user information
        /// </summary>
        /// <param name="appName">Your Backand app name</param>
        /// <param name="username">The user username</param>
        /// <param name="password">The user password</param>
        /// <returns></returns>
        public SignInResult SignIn(string appName, string username, string password)
        {
            client = GetRestClient();

            var res = SignInInner(username, password, appName);
            client.AddDefaultHeader("Authorization", res.token_type + " " + res.access_token);
            return res;
        }

        /// <summary>
        /// Get a list of objects.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="name">The name of the object</param>
        /// <param name="totalRows">Returns the total rows in the database regardless the filter or page. Use it for paging</param>
        /// <param name="pageNumber">Page number start from one</param>
        /// <param name="pageSize">Number of returned objects in page</param>
        /// <param name="filter">filter the list either with NoSql: {"q":{"age":{"$lt":25}}} or [{fieldName:"age", operator:"lessThan", value:25}]</param>
        /// <param name="sort">sort the list with [{fieldName:"age", order:"asc"},{fieldName:"height", order:"desc"}]</param>
        /// <param name="search">critaria to search in all textual fields</param>
        /// <param name="deep">When set to true it gets all the parent objects under a relatedObjects node</param>
        /// <returns></returns>
        public List<T> GelList<T>(string name, out int? totalRows, int? pageNumber = null, int? pageSize = null, object filter = null, object sort = null, string search = null, bool? deep = null) where T : new()
        {
            var request = new RestRequest("/1/objects/{name}", Method.GET);
            request.AddUrlSegment("name", name);
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
            
            var response = client.Execute<GelListResult<T>>(request);

            totalRows = response.Data.totalRows;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data.data;
            }
            else
            {
                throw new BeckandException(response);
            }
        }
        /// <summary>
        /// Get a single object from Backand
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="name">The name of the object</param>
        /// <param name="id">The primary key value of the compatible row</param>
        /// <param name="deep">Get all the direct descendents of the row through child relations</param>
        /// <param name="level">The descendent deep level, default 3</param>
        /// <returns>A typed object</returns>
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
                throw new BeckandException(response);
            }
        }

        /// <summary>
        /// Create a new row in the database
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="name">The name of the object</param>
        /// <param name="data">The object to create</param>
        /// <param name="id">Out the created row id</param>
        /// <param name="deep">Create also direct descendents of the object through child relations</param>
        /// <param name="returnObject">If set to true it returns the object. Use it if you have triggered actions that modify the created object.</param>
        /// <param name="parameters">Parameters for triggered actions</param>
        /// <returns>A typed object</returns>
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
                throw new BeckandException(response);
            }
        }

        /// <summary>
        /// Update an existing row in the database
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="name">The name of the object</param>
        /// <param name="id">The row id to update</param>
        /// <param name="data">The object to update</param>
        /// <param name="deep">Updates and crteates also direct descendents of the object through child relations</param>
        /// <param name="returnObject">If set to true it returns the object. Use it if you have triggered actions that modify the created object</param>
        /// <param name="parameters">Parameters for triggered actions</param>
        /// <param name="overwrite">If deep it will also delete descendent that do not exist in the collections</param>
        /// <returns>A typed object</returns>
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
                throw new BeckandException(response);
            }
        }

        /// <summary>
        /// Delete an existing row in the database
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="id">The row id to delete</param>
        /// <param name="deep">Delete the object descendents</param>
        /// <param name="parameters">Parameters for triggered actions</param>
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
                throw new BeckandException(response);
            }
        }
    }

    /// <summary>
    /// The OAuth2 sign in results. 
    /// </summary>
    public class SignInResult
    {
        /// <summary>
        /// OAuth2 access token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// OAuth2 access type (bearer)
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// Duration in seconds
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// The app name
        /// </summary>
        public string appName { get; set; }
        /// <summary>
        /// The username
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// The user role
        /// </summary>
        public string role { get; set; }
        /// <summary>
        /// The user full name
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// The user id in the users object if such exists
        /// </summary>
        public string userId { get; set; }
    }

    /// <summary>
    /// Get list result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GelListResult<T>
    {
        /// <summary>
        /// The total rows in the database, regerdless the page or filter. Use this for paging
        /// </summary>
        public int totalRows { get; set; }
        /// <summary>
        /// The list of objects returned
        /// </summary>
        public List<T> data { get; set; }
    }

    /// <summary>
    /// Beckand general exception
    /// </summary>
    public class BeckandException : Exception
    {
        /// <summary>
        /// The http status code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Beckand general exception constructor
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="statusCode">The http status code</param>
        public BeckandException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Beckand general exception constructor
        /// </summary>
        /// <param name="response">Extract the status code and message from the Backand http response</param>
        public BeckandException(IRestResponse response)
            : this(response.Content, response.StatusCode)
        {
            
        }
    }
}
