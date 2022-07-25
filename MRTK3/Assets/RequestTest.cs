using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AuthenticationManager.AccessToken();
        //AuthenticationManager.Authentication();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
