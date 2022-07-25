using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AuthenticationManager.RequestAccessToken(OnSuccess, OnFailure);
        //AuthenticationManager.Authentication();
    }

    void OnSuccess()
    {
        Debug.Log("Access Token: "+AuthenticationManager.GetAccessToken());
        Debug.Log("Requesting Data.....");
        AuthenticationManager.GetData();
    }

    void OnFailure(string message)
    {
        Debug.Log(message);
    }
}
