using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

public class MultiPrefabs : EditorWindow
{
    PTData DB;

    float height;
    float widthPos;
    float btnWidthPos;


    float loadingBarCount; // 로딩바 카운트
    string stateText;
    string titleText;
    string manualText;
    public string folderName = " Folder Name";

    bool createFolder;

    string lastName;

    List<string> objName = new List<string>();

    GameObject PREFAB;
    Vector2 SCROLLVIEW;
    Vector2 STATESCROLLVIEW;
    Vector2 MAINCROLLVIEW;
    //Dictionary<string, GameObject> DICSELECTOBJECT = new Dictionary<string, GameObject>();
    List<GameObject> DICSELECTOBJECT = new List<GameObject>();
    GUIStyle BgStyle = new GUIStyle();
    GUIStyle listBgStyle = new GUIStyle();
    GUIStyle darkListBGStyle = new GUIStyle();
    GUIStyle listBgLineStyle = new GUIStyle();
    GUIStyle listBtnStyle = new GUIStyle();
    GUIStyle darkBGStyle = new GUIStyle();
    GUIStyle darkListBGLogoStyle = new GUIStyle();
    GUIStyle footerStyle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    DirectoryInfo DIRINFO;
    FileInfo[] FILEINFO;

    enum STATEMESSAGE
    {
        NONE = 0,
        WATING,
        LOAD,
        COMPLETE
    }
    STATEMESSAGE stateMessage = STATEMESSAGE.WATING;

    void StateUpdate()
    {
        switch (stateMessage)
        {
            case STATEMESSAGE.WATING:
                stateText = "Please import objects / ";
                break;
            case STATEMESSAGE.LOAD:
                stateText = "IMPORTED OBJECTS : ";
                break;
            case STATEMESSAGE.COMPLETE:
                stateText = "CREATED PREFABS : ";
                break;
        }
    }
    void OnEnable()
    {
        LoadData();
        listBtnStyle.normal.background = DB.DBTexture[6];
        darkBGStyle.normal.background = DB.DBTexture[7];
        darkListBGStyle.normal.background = DB.DBTexture[8];
        
        widthPos = 0;
        btnWidthPos = 0;

        fontStyle.font = DB.DBFonts[1];

        // 풋터 폰트 설정
    }
    
    //저장된 데이터 로드
    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }

    void LoadIcon()
    {
        if(DICSELECTOBJECT.Count > 0)
        {
            darkListBGLogoStyle.normal.background = null;
        }
        else
        {
            darkListBGLogoStyle.normal.background = DB.DBTexture[9];
        }
    }

    void TitleTex()
    {
        if(DICSELECTOBJECT.Count > 0)
        {
            titleText = "OBJECT NAME    ";
            manualText = "";
        }
        else
        {
            // titleText = "Multi Prefabs Generator";
            titleText = "PREFAB MAKER   ";
            manualText = "Please select a game object \nand press the Load button.";
        }
    }

    void OnGUI()
    {
        TitleTex();

        //LoadIcon();
        StateUpdate();
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", darkBGStyle);

        GUI.Box(new Rect(0, 25, Screen.width, 124), "", darkListBGStyle);
        GUI.color = new Color32(255, 255, 255, 200);
        darkListBGLogoStyle.normal.background = null;
        darkListBGLogoStyle.alignment = TextAnchor.MiddleCenter;
        darkListBGLogoStyle.normal.textColor = Corr(120, 120, 120, 255);
        GUI.Box(new Rect(0, 42, Screen.width, 90), manualText, darkListBGLogoStyle);
        GUI.color = Color.white;



        //--[ top blue line ] --
        GUILayout.BeginHorizontal();

        BgStyle.normal.textColor = new Color32(204, 204, 204, 255);
        BgStyle.alignment = TextAnchor.MiddleCenter;
        BgStyle.normal.background = DB.DBTexture[0];    
        GUILayout.Box("#", BgStyle, GUILayout.MaxWidth(26), GUILayout.Height(25));
        //line
        BgStyle.normal.background = DB.DBTexture[1];
        GUILayout.Box("", BgStyle, GUILayout.MaxWidth(2), GUILayout.Height(25));
        
        BgStyle.normal.background = DB.DBTexture[0];

        GUILayout.Box(titleText, BgStyle, GUILayout.MaxWidth(Screen.width-26), GUILayout.Height(25));
        

        GUILayout.EndHorizontal();


        // Load object LIST
        GUILayout.BeginHorizontal(GUILayout.MinHeight(125.0f));
        GUI.color = new Color32(162, 162, 162, 150);
        SCROLLVIEW = GUILayout.BeginScrollView(SCROLLVIEW);
        GUI.color = Color.white;
        if (DICSELECTOBJECT.Count>0)
        {
            // objName = DICSELECTOBJECT.Keys.ToList();


            try
            {
                for (int i = 0; i < DICSELECTOBJECT.Count; i++)
                {
                    if (DICSELECTOBJECT.Count > 5)
                    {
                        widthPos = 2;
                        btnWidthPos = 15;
                    }
                    else
                    {
                        widthPos = 0;
                        btnWidthPos = 0;
                    }


                    if (i % 2 == 0)
                    {
                        listBgStyle.normal.background = DB.DBTexture[2]; // 회색배경
                        listBgLineStyle.normal.background = DB.DBTexture[3];
                    }
                    else
                    {
                        listBgStyle.normal.background = DB.DBTexture[4];// 라인
                        listBgLineStyle.normal.background = DB.DBTexture[5];
                    }


                    GUILayout.BeginHorizontal();
                    listBgStyle.alignment = TextAnchor.MiddleCenter;
                    listBgStyle.normal.textColor = new Color32(77, 77, 77, 255);
                    GUILayout.Box((i + 1).ToString(), listBgStyle, GUILayout.MaxWidth(26 + widthPos), GUILayout.Height(25));
                    GUILayout.Box("", listBgLineStyle, GUILayout.MaxWidth(2), GUILayout.Height(25));

                    listBgStyle.alignment = TextAnchor.MiddleLeft;
                    GUILayout.Box(" " + DICSELECTOBJECT[i].name, listBgStyle, GUILayout.MaxWidth(Screen.width - 54), GUILayout.Height(25));
                    GUILayout.Box("", listBgLineStyle, GUILayout.MaxWidth(2), GUILayout.Height(25));
                    GUILayout.Box("", listBgStyle, GUILayout.MaxWidth(26), GUILayout.Height(25));

                    GUILayout.BeginArea(new Rect(Screen.width - (25 + btnWidthPos), (i * 25), 26, 25), "");
                    if (GUI.Button(new Rect(0, 0, 26, 25), "", listBtnStyle))
                    {
                        DICSELECTOBJECT.RemoveAt(i);
                    }
                    GUILayout.EndArea();
                    GUILayout.EndHorizontal();
                }
            }
            catch
            {
                loadingBarCount = 0;
                DICSELECTOBJECT.Clear();
                stateMessage = STATEMESSAGE.WATING;

            }



        }

        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        //경계선
        BgStyle.normal.background = DB.DBTexture[11];
        GUILayout.Box("", BgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //---[ 버튼 영역 BUTTON ] ---
        GUILayout.BeginArea(new Rect(6,159,Screen.width-12,18));
        GUILayout.BeginHorizontal();
        //LOAD
        GUI.color = new Color32(110, 110, 110, 255);
        fontStyle = new GUIStyle(GUI.skin.button);
        fontStyle.font = DB.DBFonts[1];
        fontStyle.fontSize = 11;
        
        if (GUILayout.Button("LOAD OBJECTS", fontStyle) && Selection.gameObjects.Length > 0)
        {

            if (DICSELECTOBJECT.Count >0)
            {
                DICSELECTOBJECT.Clear();
            }

            //Hierachy 창에 있지 않은것만 담아준다
            DICSELECTOBJECT = Selection.gameObjects.Where(g => !AssetDatabase.Contains(g)).ToList();

            //foreach (var t in Selection.gameObjects)
            //{
            //    DICSELECTOBJECT[t.name] = t;
            //}

            stateMessage = STATEMESSAGE.LOAD;
        }
        //Remove ALL
       // GUI.color = new Color32(130, 130, 130, 255);

        if (GUILayout.Button("REMOVE ALL", fontStyle, GUILayout.MaxWidth(100)))
        {
            if (DICSELECTOBJECT != null)
            {
                // 삭제코드
                loadingBarCount = 0;
                DICSELECTOBJECT.Clear();
                stateMessage = STATEMESSAGE.WATING;
            }
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        //------------------------
        GUILayout.Box("", BgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));
        BgStyle.normal.background = DB.DBTexture[12];
        BgStyle.alignment = TextAnchor.MiddleLeft;
        BgStyle.normal.textColor = new Color32(120, 120, 120, 255);
        GUILayout.Box("      FOLDER", BgStyle,GUILayout.MinWidth(Screen.width), GUILayout.Height(20));
        GUI.color = new Color32(120, 120, 120, 255);
        createFolder = GUI.Toggle(new Rect(5,188,20,20), createFolder,"");
        if(!createFolder)
        {
            folderName = " Folder Name";
        }
        GUI.color = Color.white;
        BgStyle.normal.background = DB.DBTexture[11];
        //경계선
        GUILayout.Box("", BgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));
        if (createFolder)
        {
            BgStyle.normal.background = DB.DBTexture[13];
            folderName = GUILayout.TextField(folderName);
            //folderName = GUILayout.TextField(folderName, BgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
            //GUILayout.Box("", BgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        }


        EditorGUILayout.Space();

        //Folder NAME
        //folderName = GUILayout.TextField(folderName); 

        EditorGUILayout.BeginHorizontal();
        BgStyle.normal.background = null;
        GUILayout.Box("",BgStyle,GUILayout.MaxWidth(3));
        Cor(new Color32(81,98,156,255));
        if (GUILayout.Button("CREATE PREFABS",GUILayout.MaxWidth(Screen.width-12))&& DICSELECTOBJECT.Count > 0)
        {
            // 폴더 생성
            if (createFolder)
            {
                // 같은 이름의 폴더가 없을때만 폴더 생성
                if (AssetDatabase.IsValidFolder("Assets/" + folderName) == false)
                {  
                    AssetDatabase.CreateFolder("Assets", folderName);
                }

                foreach (var t in DICSELECTOBJECT)
                {
                    // 오브젝트가 이미 에셋 경로에 포함 되어 있는지 확인
                    // 경로에 같은 이름의 오브젝트가 있다면 이름뒤에 (new)를 붙여준다.
                    if (AssetDatabase.LoadAssetAtPath("Assets/"+ folderName + "/" + t.name + ".prefab", typeof(Object)) != null)
                    {
                        DIRINFO = new DirectoryInfo("Assets/"+ folderName + "/");
                        FILEINFO = DIRINFO.GetFiles().Where(g => g.Name.Contains(t.name + "(new)")).ToArray();
                        string newText = "(new)";
                        for (int i = 0; i < FILEINFO.Length / 2; i++)
                        {
                            //이름뒤에 (new)가 붙어 있으면 새로운 (new)를 붙여서 덮어씌워지는 문제 해결
                            newText = newText + "(new)"; 
                        }
                        PrefabUtility.CreatePrefab("Assets/"+ folderName + "/" + t.name + newText + ".prefab", t);
                    }
                    else
                    {
                        PrefabUtility.CreatePrefab("Assets/"+ folderName + "/" + t.name + ".prefab", t);
                    }

                    //PrefabUtility.CreatePrefab("Assets/" + t.name + ".prefab", PREFAB);
                    loadingBarCount++;
                    LoadingBarWindow(DICSELECTOBJECT.Count, loadingBarCount);
                }
                //AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                //folderName = " Folder Name";

                Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/" + folderName, typeof(Object));

            }


            // 폴더 생성 안함
            else
            {
                foreach (var t in DICSELECTOBJECT)
                {
                    // 오브젝트가 이미 에셋 경로에 포함 되어 있는지 확인
                    if (AssetDatabase.LoadAssetAtPath("Assets/" + t.name + ".prefab", typeof(Object)) != null)
                    {
                        DIRINFO = new DirectoryInfo("Assets");
                        FILEINFO = DIRINFO.GetFiles().Where(g=>g.Name.Contains(t.name+"(new)")).ToArray();
                        string newText = "(new)";
                        for(int i = 0; i < FILEINFO.Length/2; i++)
                        {
                            newText = newText + "(new)";
                        }
                        PrefabUtility.CreatePrefab("Assets/" + t.name + newText + ".prefab", t);
                        lastName = t.name + newText + ".prefab";
                    }
                    else
                    { 
                        PrefabUtility.CreatePrefab("Assets/" + t.name + ".prefab", t);
                        lastName = t.name + ".prefab";
                    }
                    
                     loadingBarCount++;
                     LoadingBarWindow(DICSELECTOBJECT.Count, loadingBarCount);
                   
                }
               
                //AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();

                Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/" + lastName, typeof(Object));
            }
            loadingBarCount = 0;

        }

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
        Footer();
    }

    //Loading bar
    void LoadingBarWindow(int Count, float loadingBarCount)
    {
        EditorUtility.DisplayProgressBar("Creating Prefabs", "Complete : " + Count + " / " + loadingBarCount.ToString(), loadingBarCount / (float)Count);
        stateMessage = STATEMESSAGE.COMPLETE;
    }


    void Footer()
    {
        footerStyle.normal.background = DB.DBTexture[14];
        GUI.Box(new Rect(0,Screen.height-54,137,31),"",footerStyle);
        footerStyle.normal.background = DB.DBTexture[15];
        GUI.Box(new Rect(137, Screen.height - 54, Screen.width, 31), "", footerStyle);
        footerStyle.normal.textColor = Corr(66, 66, 66, 255);
        footerStyle.font = DB.DBFonts[0];
        footerStyle.fontSize = 11;
        footerStyle.normal.background = null;
        footerStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Box(new Rect(137, Screen.height - 53, 32, 32), "" + DICSELECTOBJECT.Count, footerStyle);
    }
    
    //컬러 간단
    Color32 Corr(byte r, byte g, byte b, byte a)
    {
        return new Color32(r, g, b, a);
    }

    void Cor(Color32 cor)
    {
        GUI.color = cor;
    }

    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[1] = false;
    }



}
