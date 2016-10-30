using UnityEngine;
using System.Collections;

public class fishMove : MonoBehaviour {


    Vector3 pos;

    void Awake()
    {
    
        StartCoroutine(RepositionWithDelay()); // 코루틴 함수 실행

    }
   


    IEnumerator RepositionWithDelay()
    {
        while (true)
        {
           
            SetRandomPosition(); // 랜덤방향을 설정
            StartCoroutine(run()); // 랜덤위치로 이동
            yield return new WaitForSeconds(5f); // 5초후에 다시 셋팅
        }
    }

    IEnumerator run()
    {
        while (true)
        {
           
            transform.Translate(pos * Time.deltaTime); // 랜덤위치로 이동

            yield return null;
        }
    }

    void SetRandomPosition()
    {
        float x = Random.Range(-1.0f, 1.0f);
        float y = Random.Range(-1.0f, 1.0f);
        float z = Random.Range(-1.0f, 1.0f);
        pos = new Vector3(x, y, z); // 랜덤 방향을 설정

    }
}
