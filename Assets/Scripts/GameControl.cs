using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

    public GameObject Manager;
    public GameObject Blink;
    public AudioSource Audio;
    public GameObject gameCtrl;

    void Awake()
    {
     
        if (GameManager._life==5)
            StartCoroutine(gameCtrol());
    }

    public IEnumerator gameCtrol()
    {
        Audio.enabled = false;
        Blink.SetActive(true);
        Manager.SetActive(false);
        gameCtrl.SetActive(true);


        GameManager.gameCtrl = false;
        yield return new WaitForSeconds(5f);
        Audio.enabled = true;
        Blink.SetActive(false);
        Manager.SetActive(true);
        gameCtrl.SetActive(false);
        GameManager.gameCtrl = true;

    }
}
