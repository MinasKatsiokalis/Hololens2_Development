using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeManager : MonoBehaviour
{
    [SerializeField] GameObject fogParticles;
    [SerializeField] AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeScene());
    }

    IEnumerator InitializeScene()
    {
        yield return new WaitForSeconds(1);
        audioSource.Play();

        yield return new WaitForSeconds(2);
        fogParticles.SetActive(true);
    }
}
