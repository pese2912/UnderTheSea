using UnityEngine;
using System.Collections;

public class control : MonoBehaviour {

    public Transform camera;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HitCheck();
	}
    
    void HitCheck()
    {
        Ray ray = new Ray(camera.position, camera.rotation * Vector3.forward);
        RaycastHit hit;
        GameObject hitButton = null;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "start") //카메라와 버튼태그 충돌시
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Application.LoadLevel("1_map");
                }

            }
         

        }

    }
}
