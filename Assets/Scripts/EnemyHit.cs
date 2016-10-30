using UnityEngine;
using System.Collections;

public class EnemyHit : MonoBehaviour {

   	void Start () { 
	}

    void OnTriggerEnter( Collider col)
    {
        if (col.gameObject.tag == "bullet")
        {
            GameObject.Find("Manager").GetComponent<SoundManager>().TrashHit();
            Scene_Manager.leaved_trash_cnt++;
            Destroy(gameObject);
            Destroy(col.gameObject);
        }
        
    }
	void Update () {
	}
}
