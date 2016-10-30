using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {

   public float moveSpeed = 5f;
   public float rotateSpeed = 30f;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.gameCtrl)
        {
            float moveDistanceZ = moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
            transform.Translate(0, 0, moveDistanceZ);
            float rotateAngle = rotateSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
            transform.Rotate(0, rotateAngle, 0);
        }
    }
}
