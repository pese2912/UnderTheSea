using UnityEngine;
using System.Collections;

public class newText3 : MonoBehaviour {

    public TextMesh myText = null;
    public TextMesh startText = null;
    public string mainText;
    public string subText;
    int aniCount = 1;
    bool isTyping = false;
    public float delaytime = 4.0f;

   
    void Start()
    {
        StartCoroutine(delayTime());
    }

    IEnumerator delayTime()
    {
        yield return new WaitForSeconds(delaytime);
        isTyping = true;

        while(isTyping)
        {
            myText.text = mainText.Substring(0, aniCount-1);
            myText.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            myText.color = Color.white;
            aniCount++;
            if (aniCount > mainText.Length) isTyping = false;
        }

        startText.text = subText;
    }
}
