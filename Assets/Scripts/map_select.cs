using UnityEngine;
using System.Collections;

public class map_select : MonoBehaviour {
    public Transform camera;
    public GameObject Blink;
    public GameObject gameCtrl;

    // Use this for initialization
    void Start () {

        StartCoroutine(MapSelect());
	}
	
    public IEnumerator MapSelect()
    {
        yield return null;
        while (true)
        {


            Ray ray = new Ray(camera.position, camera.rotation * Vector3.forward);
            RaycastHit hit;
            GameObject hitButton = null;
            if (Physics.Raycast(ray, out hit))
            {
                print(GameManager._level);
                if (hit.transform.gameObject.tag == "map1") //카메라와 버튼태그 충돌시
                {
                    if (Input.GetButtonDown("Jump") && GameManager._level >= 1)
                    {
                        Blink.SetActive(true);
                        gameCtrl.SetActive(true);
                        yield return new WaitForSeconds(5f);
                        Blink.SetActive(false);
                        gameCtrl.SetActive(false);
                        Application.LoadLevel("2_gameview_1");
                    }

                }
                else if (hit.transform.gameObject.tag == "map2") //카메라와 버튼태그 충돌시
                {
                    if (Input.GetButtonDown("Jump") && GameManager._level >= 2)
                    {
                        Application.LoadLevel("2_gameview_2");
                    }

                }
                else if (hit.transform.gameObject.tag == "map3") //카메라와 버튼태그 충돌시
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        Application.LoadLevel("2_gameview_3");
                    }


                }
                else if (hit.transform.gameObject.tag == "map4") //카메라와 버튼태그 충돌시
                {

                    if (Input.GetButtonDown("Jump") && GameManager._level >= 4)
                    {
                        Application.LoadLevel("2_gameview_4");
                    }


                }
                else if (hit.transform.gameObject.tag == "map5") //카메라와 버튼태그 충돌시
                {
                    if (Input.GetButtonDown("Jump") && GameManager._level >= 5)
                    {
                        Application.LoadLevel("2_gameview_5");
                    }

                }

            }
            yield return null;
        }
    }
 
}
