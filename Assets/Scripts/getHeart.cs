using UnityEngine;
using System.Collections;

public class getHeart : MonoBehaviour {

    public float distance;
    public GameObject Player;
    public GameObject portal;
    private float _distance;
    private GameObject _Player;


    // Use this for initialization
    void Start()
    {

        _Player = Player;
        distance = 30f;

    }

    // Update is called once per frame
    void Update()
    {

        _distance = Vector3.Distance(this.transform.position, _Player.transform.position); // 물체와 플레이어 간격
        if (_distance < distance) // 일정간격 이내로 들어오면
        {
            if (portal != null)
                portal.GetComponent<ParticleSystem>().startColor = new Color(1f, 1f, 0f); // 노란색으로
        }
        else // 벗어나면 
        {
            if (portal != null)
                portal.GetComponent<ParticleSystem>().startColor = new Color(1f, 1f, 1f); // 흰색으로
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player") // 플레이어와 충돌시
        {
            Scene_Manager.demage += 0.2f;// 총알데미지 증가
            GameObject.Find("Manager").GetComponent<SoundManager>().ItemHit();// 아이템 먹는소리
            Destroy(gameObject);
        }

       
    }
}
