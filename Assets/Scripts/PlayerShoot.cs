using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour
{
    
    public GameObject bulletPrefab;
    public GameObject heartPrefab;

    float delayTimer = 0f;
    public float shootDelayTime = 0.2f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameCtrl)
        {
            delayTimer += Time.deltaTime;
            if (Input.GetButtonDown("Jump") && delayTimer > shootDelayTime) //바구니 발사
            {

                Instantiate(bulletPrefab, transform.position, transform.rotation);

                delayTimer = 0f;

            }

            if (Input.GetButtonDown("Shoot") && delayTimer > shootDelayTime) // 하트총알 발사
            {

                Instantiate(heartPrefab, transform.position, transform.rotation);

                delayTimer = 0f;

            }
        }
    }
}
