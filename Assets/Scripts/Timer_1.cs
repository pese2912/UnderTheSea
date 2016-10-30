using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer_1 : MonoBehaviour
{

    public GameObject timerText;
    private bool timerIsActive = false;

    void Start()
    {
        timerIsActive = true;
    }

    void Update()
    {
        if (timerIsActive)
        {
            Scene_Manager.startTime -= Time.deltaTime;
            float t = Scene_Manager.startTime - Time.deltaTime;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f0");
            if (Scene_Manager.startTime > 0)
            {
                timerText.GetComponent<TextMesh>().text = minutes + ":" + seconds;
            }
        }
    }
}