using UnityEngine;
using System.Collections;

public class CiclularProgress : MonoBehaviour {
	

	// Use this for initialization
	void Start () {

		//Use this to Start progress
		StartCoroutine(RadialProgress());
	}
	
	IEnumerator RadialProgress()
	{

		float rate = 1 ;
		float i = 0;
		while (i < 1)
		{
            i = 0.3f;//Time.deltaTime;
            this.gameObject.GetComponent<Renderer>().material.SetFloat("_Progress", i);
            yield return 0;
		}
	}
}