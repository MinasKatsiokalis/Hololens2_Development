using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class TextAnim : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;

    public string[] stringArray;

    [SerializeField] float timeBtwnChars;
    [SerializeField] float timeBtwnWords;

    //int i = 0;

    void Start()
    {
        StartCoroutine(EndCheck()); 
    }

    public IEnumerator EndCheck()
    {
        for(int i=0; i <= stringArray.Length - 1; i++)
        //if (i <= stringArray.Length - 1)
        {
            Debug.Log("word");
            _textMeshPro.text += stringArray[i] + "\n\n";

            yield return StartCoroutine(TextVisible());

        }
    }

    private int visibleCount = 0;
    private int counter = 0;
    private IEnumerator TextVisible()
    {
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;

        while (visibleCount < totalVisibleCharacters)
        {
            visibleCount = counter % (totalVisibleCharacters + 1);
            _textMeshPro.maxVisibleCharacters = visibleCount;

            /*
            if (visibleCount >= totalVisibleCharacters)
            {
                i += 1;
                Invoke("EndCheck", timeBtwnWords);
                break;
            }*/

            counter += 1;
            yield return new WaitForSeconds(timeBtwnChars);
        }
    }
}
