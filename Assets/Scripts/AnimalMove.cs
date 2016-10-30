using UnityEngine;
using System.Collections;

public class AnimalMove : MonoBehaviour
{


    public GameObject nextAnimal;
    public Transform progressBar;
    public Transform camera;
    public float time = 30;
    public GameObject move1;
    public GameObject move2;


    private float _demage = 0.0f;
    private 

    // Use this for initialization
    void Start()
    {
        move1.active = true; // 쓰레기산으로 이동
        StartCoroutine(RadialProgress());
    }


    IEnumerator RadialProgress()
    {

        while (true)
        {

            if (_demage >= 1.0f) // 데미지가 1이상 되었을 경우
            {
                move1.active = false; // 이동멈추고
                move2.active = true; // 다시 되돌아간다.

                StartCoroutine(nextAnimalCome());

            }


            progressBar.FindChild("Progress").GetComponent<Renderer>().material.SetFloat("_Progress", _demage); // 게이지바를 그려준다
            yield return 0;
        }
    }

    void Update()
    {
        progressBar.LookAt(camera); // 게이지바는 항상 카메라를 향해
    }



    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "trash_m") // 쓰레기산과 충돌시
        {
            Scene_Manager.startTime -= time; // 시간 감소
            move1.active = false;
            gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
            StartCoroutine(nextAnimalCome());
        }

        if (col.gameObject.tag == "heart") // 하트 총알 맞으면
        {
            _demage += Scene_Manager.demage; // 데미지 증가
            GameObject.Find("Manager").GetComponent<SoundManager>().AnimalHit();
            Destroy(col.gameObject);
        }
    }

    IEnumerator nextAnimalCome()
    {
        float random = Random.Range(15f, 40f); // 최소 15초 최대 40초
        yield return new WaitForSeconds(random); // 랜덤 시간 후
        Destroy(gameObject); // 해제
        if (nextAnimal != null)
        {
            nextAnimal.active = true; // 다음 동물이 있으면 나타난다.
        }
    }
}
