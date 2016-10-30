using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

public class prototypeObjects : EditorWindow
{
    PTData DB; // 데이터   

    DirectoryInfo DIRINFO;
    FileInfo[] PREFABINFO;

    GameObject[] prefabs;
    int prefabLength;
    Vector2 scrollPos;
    List<GameObject> CHECKGAMEOBJECT = new List<GameObject>();
    List<GameObject> LISTGAMEOBJECT = new List<GameObject>();

    Dictionary<string, FileInfo> DICPREFABSDATA = new Dictionary<string, FileInfo>();
    Texture2D buttonTexture;

    Vector3 cameraPos;
    Vector3 objPos;

    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();

    string[] DirpathName;
    string returnText;

    void OnEnable()
    {
        LoadData(); // 데이터로드
        nullStylle.normal.background = null;
        Refresh();
    }

    public void Refresh()
    {
        AssetPreview.SetPreviewTextureCacheSize(0);
        AssetPreview.SetPreviewTextureCacheSize(5000);

        DIRINFO = new DirectoryInfo("Assets/POWERTOOLS/Data/prototype_obj/prefabs/");
        PREFABINFO = DIRINFO.GetFiles();
        foreach (var t in DIRINFO.GetFiles())
        {
            if (t.Name.Contains(".meta") && t.Name.Contains(".prefab")) continue;
            else if (t.Name.Contains(".prefab")) DICPREFABSDATA[t.ToString()] = t;
        }

        LISTGAMEOBJECT  = DICPREFABSDATA.Values.Select(s => (GameObject)AssetDatabase.LoadAssetAtPath(DirPathNameFactory(s.ToString()), typeof(GameObject))).ToList();
        Repaint();
    }

  
    //경로 수정
    string DirPathNameFactory(string pathName)
    {
        DirpathName = pathName.Split(new string[] { "Assets" }, System.StringSplitOptions.None);
        returnText = "Assets" + DirpathName[1];
        return returnText;
    }

    // 이미지,폰트 데이터 로드
    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }

    void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "PROTOTYPE OBJECTS", fontNomalStyle);

    }


    void OnGUI()
    {

        TopLine();

        GUILayout.Box("", nullStylle, GUILayout.Height(3));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        //GUI.color = Corr(150, 150, 150, 255);
        GUI.color = Corr(153, 153, 153, 255);
        EditorGUILayout.HelpBox("Please Checked Option And Register your prefab button", MessageType.Info);
        GUI.color = Color.white;
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();

        GUILayout.Box("", nullStylle, GUILayout.Height(7));
        //ContentsBLine();
        //GUILayout.Box("", nullStylle, GUILayout.Height(5));




        int columns = Mathf.FloorToInt(Screen.width / 85);
        GUI.color = Corr(75,75,75,255);
        GUILayout.BeginHorizontal(GUI.skin.box);
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < LISTGAMEOBJECT.Count; i++)
        {

            if (LISTGAMEOBJECT[i] == null)
            {
                Refresh();
            }

            try
            {
                if (i % columns == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }

                if (AssetPreview.GetAssetPreview(LISTGAMEOBJECT[i]) == null)
                {
                    buttonTexture = AssetPreview.GetMiniThumbnail(LISTGAMEOBJECT[i]);
                }
                else
                {
                    buttonTexture = AssetPreview.GetAssetPreview(LISTGAMEOBJECT[i]);
                }

                GUI.color = new Color(1, 1, 1, 1.0f);
                if (GUILayout.Button(buttonTexture, GUILayout.Width(80), GUILayout.Height(80)))
                {
                    GameObject create = PrefabUtility.InstantiatePrefab(LISTGAMEOBJECT[i]) as GameObject;
                    create.name = LISTGAMEOBJECT[i].name;
                    cameraPos = SceneView.lastActiveSceneView.pivot;
                    objPos = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z);
                    create.transform.position = objPos;
                    //MapMagEditor.RandomFrefabOn = false;
                    //MapMag.selectPrefab = LISTGAMEOBJECT[i];
                    SceneView.RepaintAll();
                }
                GUI.color = Color.white;
                Repaint();
            }
            catch { }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        GUI.color = Color.white;
        GUILayout.EndScrollView();

        GUI.color = Corr(100, 100, 100, 255);
        EditorGUILayout.HelpBox(" Prefab Buttons : " + LISTGAMEOBJECT.Count, MessageType.None);
        GUI.color = Color.white;

        //GUILayout.BeginHorizontal();

        //if (GUILayout.Button("REFRESH PREFABS DATA", GUILayout.MaxWidth(180)))
        //{
        //    OnEnable();
        //}

        //GUI.color = LISTGAMEOBJECT.Count >= 50 ? Color.yellow : Color.white;
        //EditorGUILayout.HelpBox(" Current Buttons : " + LISTGAMEOBJECT.Count + "", MessageType.None);
        //GUILayout.EndHorizontal();
    }
  
    // 컨덴츠 영역 경계선(가로)
    void ContentsBLine()
    {
        EditorGUILayout.BeginHorizontal();
        boundarylineStyle.normal.background = DB.DBTexture[23];
        //GUILayout.Box("", nullStylle, GUILayout.Width(7), GUILayout.Height(3));
        GUILayout.Box("", boundarylineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));
        //GUILayout.Box("", nullStylle, GUILayout.Width(7), GUILayout.Height(3));
        EditorGUILayout.EndHorizontal();
    }

    //컬러 간단
    Color32 Corr(byte r, byte g, byte b, byte a)
    {
        return new Color32(r, g, b, a);
    }
    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[11] = false;
    }
}