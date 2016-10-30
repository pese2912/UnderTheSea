using UnityEngine;
using System.Collections;

public class Scene_Manager : MonoBehaviour {

    public GameObject Player;
    public GameObject Trash;
    public GameObject Item;
    public int trash_cnt;
    public int level;
    public Camera mainCamera;
    public Camera guideCamera;
    public GameObject canvas;
    public GameObject gameOver;
    public GameObject gameWin;
    public GameObject reStart;


    private int life;
    private bool guideMap = false;

    public static float startTime = 240;

    [HideInInspector]
    public static float demage = 0.2f;

    [HideInInspector]
    public static int leaved_trash_cnt=0;

    // Use this for initialization
    void Start()
    {
        print(GameManager._level);
        GameManager._level = level;//현재 레벨
        life = GameManager._life; // 현재 라이프갯수

        startTime = 240f; // 초기 시간
        demage = 0.2f; // 초기 데미지

        leaved_trash_cnt = 0; // 남아있는 쓰레기 갯수

        
        for (int i =1; i<=5; i++) //남아있는 라이프수만큼 거북이 그리기
        {
            if (i <= life)
            {
                canvas.transform.FindChild("turtle" + i).gameObject.active = true;
            }

            else
            {
                canvas.transform.FindChild("turtle" + i).gameObject.active = false;
            }
        }

        StartCoroutine(GameOverCheck());
        StartCoroutine(GameWinCheck());
        StartCoroutine(GuideMapCheck());
    }

  
    IEnumerator GameWinCheck() // 게임 깼는지
    {
        yield return null;
        while (true)
        {
            canvas.transform.FindChild("TrashText").GetComponent<TextMesh>().text = leaved_trash_cnt + "/" + trash_cnt; // "남았는쓰레기/남은쓰레기" 그려주기

            if(leaved_trash_cnt== trash_cnt) // 다 주웠으면
            {
                GameManager.gameCtrl = false;

                canvas.active = false;
                gameWin.active = true;
                
                if (Input.GetButtonDown("Jump"))
                {
                    GameManager._level = level + 1;
                    GameManager._life = 5;
                    Application.LoadLevel("1_map");
                    GameManager.gameCtrl = true;
                    break;
                }
                
            }

            yield return null;
        }
    }

    IEnumerator GameOverCheck()//게임오버 체크
    {
        yield return null;
        while (true)
        {


            if (startTime <= 0.0) // 시간 다되면
            {
                GameManager.gameCtrl = false;
                GameObject.Find("Manager").GetComponent<SoundManager>().GameOver();
                if (life != 1) //라이프가 남아있으면
                {

                    canvas.active = false;
                    reStart.active = true;

                    if (Input.GetButtonDown("Jump"))
                    {

                        GameManager._life -= 1;
                        Application.LoadLevel("2_gameview_" + GameManager._level);//현재 레벨에서 다시 시작
                        GameManager.gameCtrl = true;
                        break;
                    }


                }

                else // 라이프 없으면
                {
                    canvas.active = false;
                    gameOver.active = true;

                    if (Input.GetButtonDown("Jump"))
                    {

                        GameManager._life = 5;

                        Application.LoadLevel("1_map");// 첨부터 다시 시작
                        GameManager.gameCtrl = true;
                        break;
                    }

                }
            }


            yield return null;
        } 
    }

    IEnumerator GuideMapCheck() //가이드맵 체크
    {
        yield return null;
        while (true)
        {
            if (GameManager.gameCtrl)
            {
                if (Input.GetButtonDown("Guide")) // 가이드버튼 누르면
                {
                    guideMap = !guideMap;
                }

                if (!guideMap) // 상태체크에 따라 카메라 변경
                {

                    mainCamera.enabled = true; // 메인카메라 활성화
                    guideCamera.enabled = false;
                }
                else
                {

                    guideCamera.enabled = true;// 가이드카메라 활성화
                    mainCamera.enabled = false;
                }
                setGuide(guideMap); // 가이드 함수
            }
            yield return null;
        }
    }

    void setGuide(bool state)
    {
       
        Transform[] TrashChild = Trash.GetComponentsInChildren<Transform>(); //쓰레기 하위 오브젝트들
        Transform[] ItemChild = Item.GetComponentsInChildren<Transform>(); // 아이템 하위 오브젝트들

        if (state) // 가이드카메라 활성시
        {
     
            Player.transform.FindChild("GreenSphere").gameObject.active=true; // 플레이어 위 그린공 활성

            foreach (Transform child in TrashChild) //쓰레기 하위 오브젝트 하나씩
            {
                if (child.FindChild("RedSphere") != null)
                     child.FindChild("RedSphere").gameObject.active = true; // 레드공 활성화
            }

            foreach (Transform child in ItemChild)// 아이템 하위 오브젝트 하나씩
            {
                if (child.name.Contains("Gas Bottle")) // 산소통이면
                {
                    if (child.FindChild("BlueSphere") != null)
                        child.FindChild("BlueSphere").gameObject.active = true; //블루공 활성
                }

                else if (child.name.Contains("heart_can"))//하트 통조림이면
                {
                    if (child.FindChild("YellowSphere") != null)
                        child.FindChild("YellowSphere").gameObject.active = true; // 노란공 활성
                }
            }
        }

        else // 메인카메라 활성 시
        {

            Player.transform.FindChild("GreenSphere").gameObject.active = false;// 플레이어 위 그린공 비활성

            foreach (Transform child in TrashChild)//쓰레기 하위 오브젝트 하나씩
            {
                if (child.FindChild("RedSphere") != null)
                     child.FindChild("RedSphere").gameObject.active = false;// 레드공 비활성화
            }
            foreach (Transform child in ItemChild)// 아이템 하위 오브젝트 하나씩
            {
                if (child.name.Contains("Gas Bottle"))// 산소통이면
                {
                    if (child.FindChild("BlueSphere") != null)
                        child.FindChild("BlueSphere").gameObject.active = false;//블루공 비활성
                }

                else if (child.name.Contains("heart_can"))//하트 통조림이면
                {
                    if (child.FindChild("YellowSphere") != null)
                        child.FindChild("YellowSphere").gameObject.active = false;// 노란공 비활성
                }
            }
        }
    }
}
