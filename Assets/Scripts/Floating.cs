using UnityEngine;
using System.Collections;

public class Floating : MonoBehaviour {



    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time, 3), transform.position.z); 
        // 둥둥 떠다님
    }


}
