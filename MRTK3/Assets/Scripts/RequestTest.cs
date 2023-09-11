using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;

public class RequestTest : MonoBehaviour
{
    public static RequestTest Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Dictionary<string, string> envVariables;
    void Start()
    {
        //AuthenticationManager.RequestAccessToken(OnSuccess, OnFailure);
        //AuthenticationManager.Authentication();

        string envFilePath = Path.Combine(Application.streamingAssetsPath, "file.env");
        envVariables = ReadEnvFile(envFilePath);
        
        OAuth2(Pilot.leuven.ToString());
        
    }

    void OnSuccess()
    {
        Debug.Log("Access Token: "+AuthenticationManager.GetAccessToken());
        Debug.Log("Requesting Data.....");
        AuthenticationManager.GetAllSensors();
        //AuthenticationManager.GetSensorData(sensor4, 1);
    }

    void OnFailure(string message)
    {
        Debug.Log(message);
    }

    async void OAuth2(string pilot)
    {
        string accessToken = await AuthenticationManager.GetAccessTokenAsync(pilot);
        AuthenticationManager.GetData(accessToken, pilot);
    }
    private Dictionary<string, string> ReadEnvFile(string path)
    {
        Dictionary<string, string> env = new Dictionary<string, string>();
        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            {
                string[] keyValue = line.Split(new[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    // Replace double underscores with a single underscore
                    key = key.Replace("__", "_");

                    env[key] = value;
                }
            }
        }

        return env;
    }
}

public enum Pilot
{
    chania,
    dundalk,
    castelfranco_veneto,
    leuven,
    gzira,
    skelleftea,
    novo_mesto
}
