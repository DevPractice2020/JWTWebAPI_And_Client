using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace apiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get Token very first time

            dynamic authobj = new ExpandoObject();
            authobj.username = "test";
            authobj.password = "test";

            var client = new RestClient("http://localhost:4000/users/authenticate");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");            
            request.AddParameter("application/json", JsonConvert.SerializeObject(authobj), ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            JObject jsonObj = JObject.Parse(response.Content);
            var accessToken = (String)jsonObj["jwtToken"];            
            var refreshToken = HttpUtility.UrlDecode(response.Cookies.Where(x => x.Name == "refreshToken").FirstOrDefault().Value);

            // Refresh the Token
            client = new RestClient("http://localhost:4000/users/refresh-token");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddParameter("refreshToken", refreshToken, ParameterType.Cookie);
            response = client.Execute(request);
            jsonObj = JObject.Parse(response.Content);
            accessToken = (String)jsonObj["jwtToken"];
            refreshToken = HttpUtility.UrlDecode(response.Cookies.Where(x => x.Name == "refreshToken").FirstOrDefault().Value);

            // Get the List of All Active Refresh Token of a particular User
            client = new RestClient("http://localhost:4000/users/1/refresh-tokens");
            client.Timeout = -1;
            request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);            
            response = client.Execute(request);
            var jArray = JArray.Parse(response.Content);

            // Revoke Token

            dynamic revokeobj = new ExpandoObject();
            authobj.token = refreshToken;           

            client = new RestClient("http://localhost:4000/users/revoke-token");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddHeader("Content-Type", "application/json");            
            request.AddParameter("application/json", JsonConvert.SerializeObject(authobj), ParameterType.RequestBody);
            response = client.Execute(request);
            


        }
    }
}
