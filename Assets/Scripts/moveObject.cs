using UnityEngine;
using System.Collections;

public class moveObject : MonoBehaviour {

   
    public Transform endPosition;
    public Transform startPosition;
    public Transform _target;

    public float speed = 1.0f;

    private float startTime;
    private float journeyLength;



    void OnEnable()
    {
        startTime = Time.time; // 시간
        journeyLength = Vector3.Distance(startPosition.position, endPosition.position);  // 이동 거리
      
    }

    // Update is called once per frame
    void Update()
    {
        _target.LookAt(endPosition);
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        _target.position = Vector3.Lerp(startPosition.position, endPosition.position, fracJourney); // 시작위치에서 끝위치까지 이동
                                                                      //print(startPosition.position + "       " + endPosition.position);
    }

}
