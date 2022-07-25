using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Web;

public class AuthenticationManager
{
    private static string tokenType = null;
    private static string accessToken = string.Empty;
    private static string refreshToken = null;
    private static int expiresIn = 0;
    private static int tokenTime;
    private static string clientID = "8259b0469c9d49f676b633b6850a7b43d3b45b2e";

    private static List<HttpStatusCode> serverError = new List<HttpStatusCode>() { HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError, HttpStatusCode.BadGateway, HttpStatusCode.GatewayTimeout, HttpStatusCode.NotImplemented };
    public static string GetAccessToken() =>
        accessToken;

    public static string GetRefreshToken() =>
        refreshToken;

    public static int GetAccessExpiration() =>
         (tokenTime + expiresIn - DateTime.Now.Second);


    public static async void AccessToken()
    {
        var client = new RestClient("https://varcities.tuc.gr:3443/oauth2/token");
        client.Timeout = -1;
        client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        var request = new RestRequest(Method.POST);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Authorization", "Basic MThjYmRkMjEtZDI1Ni00OWQwLTg5OWUtODg5ZDBkYzRhODMwOjc2Y2VmYzNhLTIzYmQtNGQwMC1hMDg4LTViYzQwYzA2ZDNmNQ==");
        request.AddParameter("grant_type", "password");
        request.AddParameter("username", "castelfranoquantumleap@varcities.eu");
        request.AddParameter("password", "um*$6yojC22YU%K!Tfw");

        IRestResponse response = await client.ExecuteAsync(request);

        Debug.Log(response.Content);
        Debug.Log(response.StatusCode);
        Debug.Log(response.ErrorMessage);
        Debug.Log(response.ErrorException);
        Debug.Log(response.ResponseStatus);
    }
    /// <summary>
    /// User sends his/her email, username and password in order to be registered.
    /// </summary>
    public static async void Register()
    {
        string url = "https://varcities.tuc.gr:3443/oauth2/token";
        var client = new RestClient(url);
        client.Timeout = -1;
        client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        //string template = "{{\"email\": \"{0}\", \"username\": \"{1}\", \"password\": \"{2}\"}}";

        string template = "username={0}&password={1}&grant_type=password";
        string username = "castelfranoquantumleap@varcities.eu";
        string password = "um*$6yojC22YU%K!Tfw";
        string final_template = String.Format(template, username, password);

        //final_template = WebUtility.UrlEncode(final_template);
        Debug.Log(final_template);

        var request = new RestRequest(Method.POST);
        request.AlwaysMultipartFormData = true;

        
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", "Basic MThjYmRkMjEtZDI1Ni00OWQwLTg5OWUtODg5ZDBkYzRhODMwOjc2Y2VmYzNhLTIzYmQtNGQwMC1hMDg4LTViYzQwYzA2ZDNmNQ==");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        //request.AddParameter("username", username, ParameterType.UrlSegment);
        //request.AddParameter("password", password, ParameterType.UrlSegment);
        //request.AddParameter("grant_type", "password", ParameterType.UrlSegment);
        request.AddParameter("username", username);
        request.AddParameter("password", password);
        request.AddParameter("grant_type", "password");


        foreach (var param in request.Parameters)
        {
            Debug.Log(param);
        }
        IRestResponse response = await client.ExecuteAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Debug.Log(response.Content);
        }
        else
        {
            Debug.Log("ERROR!");
            Debug.Log(response.Content);
            Debug.Log(response.StatusCode);
            Debug.Log(response.ErrorMessage);
            Debug.Log(response.ErrorException);
            Debug.Log(response.ResponseStatus);

        }
    }

    /// <summary>
    /// Authenticating the user using hiw credentials, if authentication is successful an access & a refresh are returned.
    /// The access token lasts for an Hour, after this period of time user gets new access token only by sending his refresh token
    /// </summary>
    public static async void Authentication()
    {
        Debug.Log("Start...");

        string URL = "https://varcities.tuc.gr:1030/v2/entities/urn:ngsi-ld:synetica-enl-air-x:synetica-enl-air-x-004815?lastN=10";
        
        var client = new RestClient(URL);
        client.Timeout = -1;

        string token = "07679f4047dec3805c82e3df4392d143c5645b5b";
        
        var request = new RestRequest(Method.GET);

        request.AddHeader("Authorization", token);
        request.AddHeader("Accept", "application/json");

        Debug.Log("Before Authentication request");

        Debug.Log(request.Parameters);
        IRestResponse response = await client.ExecuteAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Debug.Log(response.Content);
            //ResponseSuccessContent response_content = JsonUtility.FromJson<ResponseSuccessContent>(response.Content);
        }
        else
        {
            Debug.Log(response.Content);
            Debug.Log(response.StatusCode);
            //ResponseFailContent response_content = JsonUtility.FromJson<ResponseFailContent>(response.Content);
        }
    }

    public static async void AuthenticationWithRefreshToken(string _refreshToken, Action onSuccess, Action<string> onFailure)
    {
        var client = new RestClient("https://xr4drama-integration.nurogames.com/server/oauth2/user/token");

        client.Timeout = -1;

        var request = new RestRequest(Method.POST);

        request.AlwaysMultipartFormData = true;

        request.AddParameter("grant_type", "refresh_token");
        request.AddParameter("refresh_token", _refreshToken);
        request.AddParameter("client_id", clientID);
        request.AddParameter("client_secret", "");

        IRestResponse response = await Task.Run(() =>
        {
            return client.ExecuteAsync(request);
        });

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            DateTime t2 = DateTime.Now;
            ResponseSuccessContent response_content = JsonUtility.FromJson<ResponseSuccessContent>(response.Content);

            tokenTime = DateTime.Now.Second;
            tokenType = response_content.token_type;
            expiresIn = response_content.expires_in;
            accessToken = response_content.access_token;
            refreshToken = response_content.refresh_token;
            onSuccess();
        }
        else
        {
            if (serverError.Contains(response.StatusCode))
            {
                onFailure("Server is busy");
                return;
            }
            Debug.Log(response.StatusCode);
            ResponseFailContent response_content = JsonUtility.FromJson<ResponseFailContent>(response.Content);
            // Debug.Log("ERROR: " + response_content.error);
            // Debug.Log("ERROR DESCRIPTION: " + response_content.error_description);
            // Debug.Log("MESSAGE: " + response_content.message);

            onFailure(""/*response_content.message*/);
        }

    }


    [Serializable]
    public class ResponseSuccessContent
    {
        public string token_type;
        public int expires_in;
        public string access_token;
        public string refresh_token;
    }

    [Serializable]
    public class ResponseFailContent
    {
        public string error;
        public string error_description;
        public string message;
    }
}
