using UnityEngine;
using System.Collections;

public class crash : MonoBehaviour {
    public GameObject portal;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "enemy")
        {
            //Destroy(col.gameObject);
        }
    }

    Color colorStart = Color.red;
    Color colorEnd = Color.green;
    float distance = 0f;
    Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

	// Update is called once per frame
	void Update () 
    {
        if (CharControl.m_This == null)
            return;
        distance = Vector3.Distance(CharControl.m_This.transform.position, this.transform.position);
        if (distance > 50f)
        {// 여기 숫자 바꾸면 거리가 바뀌는거야 
           // rend.material.color = new Color(1f, 1f, 1f);//colorStart; 
            if (portal != null)
                portal.GetComponent<ParticleSystem>().startColor = new Color(1f, 1f, 1f);
        }
        else
        {
            //rend.material.color = colorStart;
            if (portal != null)
                portal.GetComponent<ParticleSystem>().startColor = new Color(1f, 0f, 0f);
        }
	}
}
