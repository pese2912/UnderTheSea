using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {


    public AudioSource audioSource; // 오디오 소스
    public AudioSource bgmSound;
    public AudioClip animalHit; 
    public AudioClip itemHit;
    public AudioClip trashHit;
    public AudioClip gameOver;

    public void ItemHit()
    {

        audioSource.PlayOneShot(itemHit); // 아이템 먹는 소리 발생
    }
    public void AnimalHit()
    {
     
        audioSource.PlayOneShot(animalHit);// 동물이 총에 맞은 소리 발생
     
    }
    public void TrashHit()
    {

        audioSource.PlayOneShot(trashHit); // 쓰레기 치웠을 때 소리 발생
    }

    public void GameOver()
    {
        bgmSound.Stop();
        audioSource.PlayOneShot(gameOver); // 게임 오버 시 소리 발생
    }

}
