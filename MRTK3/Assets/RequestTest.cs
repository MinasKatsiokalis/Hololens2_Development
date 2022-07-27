using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestTest : MonoBehaviour
{
    // Start is called before the first frame update
    private string sensor1 = "urn:ngsi-ld:dl-atm22:dl-atm22-sn-15380";
    private string sensor2 = "urn:ngsi-ld:dl-pyr:dl-pyr-sn-15373";
    private string sensor3 = "urn:ngsi-ld:sensedge_stick:sensedge-01";
    private string sensor4 = "urn:ngsi-ld:synetica-enl-air-x:synetica-enl-air-x-004815";

    void Start()
    {
        AuthenticationManager.RequestAccessToken(OnSuccess, OnFailure);
        //AuthenticationManager.Authentication();
    }

    void OnSuccess()
    {
        Debug.Log("Access Token: "+AuthenticationManager.GetAccessToken());
        Debug.Log("Requesting Data.....");
        //AuthenticationManager.GetAllSensors();
        AuthenticationManager.GetSensorData(sensor4, 1);
    }

    void OnFailure(string message)
    {
        Debug.Log(message);
    }
}
