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

    private static string username = "castelfranoquantumleap@varcities.eu";
    private static string password = "um*$6yojC22YU%K!Tfw";
    private static string authorization_64 = "MThjYmRkMjEtZDI1Ni00OWQwLTg5OWUtODg5ZDBkYzRhODMwOjc2Y2VmYzNhLTIzYmQtNGQwMC1hMDg4LTViYzQwYzA2ZDNmNQ==";

    private static string quantmleap_proxy_https_port = "1030";
    private static string sensor1 = "urn:ngsi-ld:sensedge_stick:sensedge-09";
    private static string sensor2 = "urn:ngsi-ld:synetica-enl-air-x:synetica-enl-air-x-004815";
    private static string attribute = "airtemperature";


    private static List<HttpStatusCode> serverError = new List<HttpStatusCode>() { HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError, HttpStatusCode.BadGateway, HttpStatusCode.GatewayTimeout, HttpStatusCode.NotImplemented };

    public static string GetAccessToken() =>
        accessToken;
    public static string GetRefreshToken() =>
        refreshToken;
    public static int GetAccessExpiration() =>
        (tokenTime + expiresIn - DateTime.Now.Second);

    public static async void RequestAccessToken(Action onSuccess, Action<string> onFailure)
    {
        var client = new RestClient("https://varcities.tuc.gr:3443/oauth2/token");
        client.Timeout = -1;
        client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        var request = new RestRequest(Method.POST);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddHeader("Authorization", "Basic "+ authorization_64);
        request.AddParameter("grant_type", "password");
        request.AddParameter("username", username);
        request.AddParameter("password", password);

        IRestResponse response = await Task.Run(() =>
        {
            return client.ExecuteAsync(request);
        });

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            DateTime t2 = DateTime.Now;
            ResponseSuccessContent response_content = JsonUtility.FromJson<ResponseSuccessContent>(response.Content);
            Debug.Log(response.Content);

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
            Debug.Log(response.Content);
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

    public static async void GetAllSensors()
    {
        /****************************************************************************************************************************************
        * URL = 'https://'"${HOST}"':'"${QUANTMLEAP_PROXY_HTTPS_PORT}"'/v2/entities' # get all sensors
        * URL='https://'"${HOST}"':'"${QUANTMLEAP_PROXY_HTTPS_PORT}"'/v2/entities/'"${SENSOR}"'?lastN=100' # all the attribues of SENSOR
        * URL='https://'"${HOST}"':'"${QUANTMLEAP_PROXY_HTTPS_PORT}"'/v2/entities/'"${SENSOR}"'/attrs/'"${ATTRIBUTE}"'?lastN=100' //selected one
        ****************************************************************************************************************************************/

        string url = "https://varcities.tuc.gr:" + quantmleap_proxy_https_port + "/v2/entities";
        var client = new RestClient(url);
        client.Timeout = -1;
        client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        var request = new RestRequest(Method.GET);
        request.AddHeader("Fiware-Service", "openiot");
        request.AddHeader("X-Auth-Token", accessToken);
        request.AddHeader("Fiware-ServicePath", "/");

        IRestResponse response = await Task.Run(() =>
        {
            return client.ExecuteAsync(request);
        });

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Entity[] response_content = JsonHelper.GetJsonArray<Entity>(response.Content);
            foreach(Entity entity in response_content)
            {
                //Debug.Log(entity.entityType);
                switch (entity.entityType)
                {
                    case "dl-atm22":
                        Debug.Log("dl-atm22: " + entity.entityId);
                        break;
                    case "dl-pyr":
                        Debug.Log("dl-pyr: " + entity.entityId);
                        break;
                    case "synetica-enl-air-x":
                        Debug.Log("synetica-enl-air-x: " + entity.entityId);
                        break;
                    case "sensedge_stick":
                        Debug.Log("sensedge_stick: " + entity.entityId);
                        break;
                    default:
                        Debug.Log("Other type: " + entity.entityId);
                        break;
                }
            }
            Debug.Log(response.Content);
        }
        else
        {
            Debug.Log(response.StatusCode);
            ResponseFailContent response_content = JsonUtility.FromJson<ResponseFailContent>(response.Content);
            Debug.Log(response.Content);
        }
    }

    public static async void GetSensorData(string sensor, int lastN)
    {
        /****************************************************************************************************************************************
        * URL = 'https://'"${HOST}"':'"${QUANTMLEAP_PROXY_HTTPS_PORT}"'/v2/entities' # get all sensors
        * URL='https://'"${HOST}"':'"${QUANTMLEAP_PROXY_HTTPS_PORT}"'/v2/entities/'"${SENSOR}"'?lastN=100' # all the attribues of SENSOR
        * URL='https://'"${HOST}"':'"${QUANTMLEAP_PROXY_HTTPS_PORT}"'/v2/entities/'"${SENSOR}"'/attrs/'"${ATTRIBUTE}"'?lastN=100' //selected one
        ****************************************************************************************************************************************/

        string url = "https://varcities.tuc.gr:" + quantmleap_proxy_https_port + "/v2/entities/"+sensor+"?lastN="+lastN;
        var client = new RestClient(url);
        client.Timeout = -1;
        client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        var request = new RestRequest(Method.GET);
        request.AddHeader("Fiware-Service", "openiot");
        request.AddHeader("X-Auth-Token", accessToken);
        request.AddHeader("Fiware-ServicePath", "/");

        IRestResponse response = await Task.Run(() =>
        {
            return client.ExecuteAsync(request);
        });

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            EntityAttributes response_content = JsonUtility.FromJson<EntityAttributes>(response.Content);
            Debug.Log(response_content.entityType);
            foreach(Attribute attr in response_content.attributes)
            {
                Debug.Log(attr.attrName+" : "+ attr.values[0]);
            }
        }
        else
        {
            Debug.Log(response.StatusCode);
            ResponseFailContent response_content = JsonUtility.FromJson<ResponseFailContent>(response.Content);
            Debug.Log(response.Content);
        }
    }

    public static class JsonHelper
    {
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    [Serializable]
    public class ResponseSuccessContent
    {
        public string token_type;
        public string access_token;
        public string refresh_token;
        public int expires_in;
    }

    [Serializable]
    public class ResponseFailContent
    {
        public string error;
        public string error_description;
        public string message;
    }

    [Serializable]
    public class Entity
    {
        public string entityId;
        public string entityType;
        public DateTime index;
    }

    [Serializable]
    public class EntityAttributes
    {
        public List<Attribute> attributes;
        public string entityId;
        public string entityType;
        public List<DateTime> index;
    }

    [Serializable]
    public class Attribute
    {
        public string attrName;
        public List<double> values;
    }
}
