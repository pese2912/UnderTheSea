using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class MeshEditor : EditorWindow
{

    [MenuItem("NEXTI/MESH_EDITOR")]
    static void ToolBarWindow()
    {
        MeshEditor window = (MeshEditor)EditorWindow.GetWindow(typeof(MeshEditor));
        window.Show();
    }

    PTData DB;

    GUIStyle BgStyle = new GUIStyle();
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle fontFildStyle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    GUIStyle footerStyle = new GUIStyle();

    GUIStyle sceneStyle = new GUIStyle();
    GUIStyle sceneIcon = new GUIStyle();


    GameObject SELECTOBJECT;

    List<GameObject> LISTHANDLES = new List<GameObject>();

    Mesh OBJECTMESH;
    Vector3[] OBJECTVERTICS;

    Vector2 pos;
    Vector3 VerticsPos;
    Dictionary<string, Vector3> DICVERTICSPOS = new Dictionary<string, Vector3>();
    Dictionary<string, GameObject> DICVERTICSOBJECT = new Dictionary<string, GameObject>();

    Color32 gizmoCor;

    DirectoryInfo DIRINFO;
    FileInfo[] FILEINFO;


    public static float GizMOscale;
    string meshName = "New Mesh Name";
    bool OptionEnable;

    bool optionCopy;
    bool handelSize;
    bool enable;

    float handleSize = 0.5f;

    float togglePosY, optionTextPosY;
    float labelPosX, labelPosX2;

    void OnEnable()
    {
        LoadData();  //데이터 로드

        sceneStyle.font = DB.DBFonts[0];
        sceneStyle.fontSize = 15;
        sceneStyle.alignment = TextAnchor.UpperLeft;
        sceneStyle.normal.textColor = Corr(71, 255, 31, 255);
        sceneIcon.normal.background = DB.DBTexture[30];

        GizMOscale = 0.11f;
        gizmoCor = Corr(0, 149, 255, 255);


        // scene view 렌더링
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    public void OnSceneGUI(SceneView sceneview)
    {
        // 플레이 상태에서 실행 안되게.
        if (Application.isPlaying == true)
        {
            return;
        }


        //scene에서 GUI 시작.
        Handles.BeginGUI();
        if (enable)
        {
            GUI.Label(new Rect(25, 25, 200, 100), "[ MESH EDITOR MODE ]", sceneStyle);
        }
        Handles.EndGUI();

        try
        {   
            // 매쉬 편집.
            if (LISTHANDLES.Count > 2)
            {
                for (int i = 0; i < OBJECTVERTICS.Length; i++)
                {
                    LISTHANDLES[i].transform.localPosition = LISTHANDLES[i].transform.parent.localPosition;
                    OBJECTVERTICS[i] = LISTHANDLES[i].transform.localPosition;
                }
                OBJECTMESH.vertices = OBJECTVERTICS;
                OBJECTMESH.RecalculateBounds();
                OBJECTMESH.RecalculateNormals();
            }
        }
        catch
        {

        }
    }

    //저장된 데이터 로드
    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }

    //--[ 타이틀 라인 ]--
    void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "MESH EDITOR for PROTOTYPE OBJECT", fontNomalStyle);
    }
    //--[ OPTIONS ]--
    //labelPosX, labelPosX2;
    void Options()
    {
        togglePosY = 48;
        optionTextPosY = 46;
        labelPosX = 7;
        labelPosX2 = 5;

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("SELECT OPTION", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인



        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(27, optionTextPosY, 3, 21), "COPY MESH", optionStyle);
        GUI.Label(new Rect(122 + labelPosX, optionTextPosY, 3, 21), "HANDLE SIZE", optionStyle);
        //GUI.Label(new Rect(218 - labelPosX2, optionTextPosY, 3, 21), "MULTIPLE", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(92 + labelPosX, 46, 2, 21), "", boundarylineStyle);
        //GUI.Box(new Rect(188 - labelPosX2, 46, 2, 21), "", boundarylineStyle);

        //bool optionCopy;
        //bool handelSize;

        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        optionCopy = GUI.Toggle(new Rect(7, togglePosY, 40, 25), optionCopy, ""); // SELECT POSITION
        if (optionCopy)
        {
          
        }
        handelSize = GUI.Toggle(new Rect(102 + labelPosX, togglePosY, 40, 25), handelSize, ""); // SELECT TAG
        if (handelSize)
        {
          
        }
        //optionMulti = GUI.Toggle(new Rect(198 - labelPosX2, togglePosY, 40, 25), optionMulti, ""); // selection
        //if (optionMulti)
        //{
        //    optionDelNew = false; optionNew = false;
        //}
        GUI.color = Color.white;
    }

    void OptionCopy()
    {
        fontFildStyle = new GUIStyle(GUI.skin.textField);
        FontSetting(fontFildStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Corr(180, 180, 180, 255);
        meshName = GUILayout.TextField(meshName, fontFildStyle); //텍스트 입력
        GUI.color = Color.white;
        //GUILayout.Box("", nullStylle, GUILayout.Width(0));
        //EditorGUILayout.EndHorizontal();

        ////사이 띄우기
        //GUILayout.Box("", nullStylle, GUILayout.Height(15));

        //EditorGUILayout.BeginHorizontal();
        BgStyle.normal.background = DB.DBTexture[27];
        GUILayout.Box("", nullStylle, GUILayout.Width(3));

        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;
        buttonStyle.margin.top = 1;
        

        GUI.color = Corr(107, 127, 173, 255);
        buttonStyle.normal.textColor = Corr(10, 10, 10, 180);
        if (GUILayout.Button("COPY MESH", buttonStyle, GUILayout.MinHeight(16)) && Selection.gameObjects.Length > 0 && Selection.gameObjects[0].activeSelf == true)
        {
            
            SELECTOBJECT = Selection.gameObjects[0];
            Mesh mesh = SELECTOBJECT.GetComponent<MeshFilter>().sharedMesh;
            Mesh newmesh = new Mesh();
            newmesh.vertices = mesh.vertices;
            newmesh.triangles = mesh.triangles;
            newmesh.uv = mesh.uv;
            newmesh.normals = mesh.normals;
            newmesh.colors = mesh.colors;
            newmesh.tangents = mesh.tangents;


            //경로에 같은 이름을 가진 매쉬가 있는지 확인
            // 같은 이름의 오브젝트가 있다면 이름 뒤에 (new) 붙여줌.
            if (AssetDatabase.LoadAssetAtPath("Assets/" + meshName + ".asset", typeof(Object)) != null)
            {
                DIRINFO = new DirectoryInfo("Assets");// 해당 경로의 폴더 정보 
                FILEINFO = DIRINFO.GetFiles().Where(g => g.Name.Contains(meshName + " (new)")).ToArray(); // meshName+" (new)" 이름을 가진것만 담음.
                string newText = " (new)";
                for (int i = 0; i < FILEINFO.Length / 2; i++)
                {
                    newText = newText + " (new)";
                }
                AssetDatabase.CreateAsset(newmesh, "Assets/" + meshName + newText + ".asset");
            }
            else
            {
                AssetDatabase.CreateAsset(newmesh, "Assets/" + meshName + ".asset");
            }


            GameObject create = new GameObject();
            create.AddComponent<MeshFilter>();
            create.AddComponent<MeshRenderer>();
            create.GetComponent<MeshFilter>().sharedMesh = newmesh;
            create.GetComponent<MeshRenderer>().material = SELECTOBJECT.GetComponent<MeshRenderer>().sharedMaterial;
            create.name = meshName;
            create.transform.position = SELECTOBJECT.transform.position;
            Selection.activeObject = create;
            SELECTOBJECT.SetActive(false);
            //AssetDatabase.Refresh();
        }
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    void OptionHandleSize()
    {

        // Button style
       

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Corr(180, 180, 180, 255);


        fontStyle.font = DB.DBFonts[1];
        fontStyle.fontSize = 11;
        fontStyle.margin.top = 5;
        fontStyle.margin.left = 4;
        fontStyle.normal.textColor = Corr(50, 50, 50, 255);

        GUILayout.Label("SIZE", fontStyle, GUILayout.Width(40));
        GizMOscale = EditorGUILayout.Slider(GizMOscale, 0.0f, 3);


        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Corr(180, 180, 180, 255);


        GUILayout.Label("COLOR", fontStyle, GUILayout.Width(40));

        gizmoCor = EditorGUILayout.ColorField("", gizmoCor);

        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();


        DrawGizmo.gizmoColor = gizmoCor;
        DrawGizmo.scale = new Vector3(GizMOscale, GizMOscale, GizMOscale);
        SceneView.RepaintAll();

    }

    void OnGUI()
    {

        TopLine();
        LineSetting(boundarylineStyle, 29, Screen.width, 1); // 밝은 경계선
        Options();
        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

        //시작 메세지
        if (!enable && !optionCopy && !handelSize)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(7));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", nullStylle, GUILayout.Width(5));

            GUI.color = Corr(153, 153, 153, 255);
            EditorGUILayout.HelpBox("Please Checked Option And Apply Enable", MessageType.Info);
            GUI.color = Color.white;

            GUILayout.Box("", nullStylle, GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();
        }



        if (optionCopy) OptionCopy();
        if (optionCopy && handelSize)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(15));
            ContentsBLine();
            GUILayout.Box("", nullStylle, GUILayout.Height(2));
        }

        if (handelSize|| enable) OptionHandleSize();





        ////////meshName = EditorGUILayout.TextField(meshName);
        //////if (GUILayout.Button("Copy Mesh") && Selection.gameObjects.Length > 0 && Selection.gameObjects[0].activeSelf == true) // 콤바인 방식으로 변경.
        //////{
        //////    SELECTOBJECT = Selection.gameObjects[0];
        //////    Mesh mesh = SELECTOBJECT.GetComponent<MeshFilter>().sharedMesh;
        //////    Mesh newmesh = new Mesh();
        //////    newmesh.vertices = mesh.vertices;
        //////    newmesh.triangles = mesh.triangles;
        //////    newmesh.uv = mesh.uv;
        //////    newmesh.normals = mesh.normals;
        //////    newmesh.colors = mesh.colors;
        //////    newmesh.tangents = mesh.tangents;


        //////    //경로에 같은 이름을 가진 매쉬가 있는지 확인
        //////    // 같은 이름의 오브젝트가 있다면 이름 뒤에 (new) 붙여줌.
        //////    if(AssetDatabase.LoadAssetAtPath("Assets/" + meshName + ".asset", typeof(Object)) != null)
        //////    {
        //////        DIRINFO = new DirectoryInfo("Assets");// 해당 경로의 폴더 정보 
        //////        FILEINFO = DIRINFO.GetFiles().Where(g => g.Name.Contains(meshName + " (new)")).ToArray(); // meshName+" (new)" 이름을 가진것만 담음.
        //////        string newText = " (new)";
        //////        for(int i =0; i < FILEINFO.Length/2; i++)
        //////        {
        //////            newText = newText + " (new)";
        //////        }
        //////        AssetDatabase.CreateAsset(newmesh, "Assets/" + meshName +newText+ ".asset");
        //////    }
        //////    else
        //////    {
        //////        AssetDatabase.CreateAsset(newmesh, "Assets/" + meshName + ".asset");
        //////    }


        //////    GameObject create = new GameObject();
        //////    create.AddComponent<MeshFilter>();
        //////    create.AddComponent<MeshRenderer>();
        //////    create.GetComponent<MeshFilter>().sharedMesh = newmesh;
        //////    create.GetComponent<MeshRenderer>().material = SELECTOBJECT.GetComponent<MeshRenderer>().sharedMaterial;
        //////    create.name = meshName;
        //////    create.transform.position = SELECTOBJECT.transform.position;
        //////    Selection.activeObject = create;
        //////    SELECTOBJECT.SetActive(false);
        //////    //AssetDatabase.Refresh();
        //////}

        GUILayout.Box("", nullStylle, GUILayout.Height(15));

        //경계선
        BgStyle.normal.background = DB.DBTexture[23];
        GUILayout.Box("", BgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));

        // 마지막 렉트 위치 가지고 오기
        BgStyle.normal.background = DB.DBTexture[27];
        GUI.Box(new Rect(0, GUILayoutUtility.GetLastRect().position.y + 2, Screen.width, Screen.height), "", BgStyle);

        //사이 띄우기
        GUILayout.Box("", nullStylle, GUILayout.Height(4));

        EditorGUILayout.BeginHorizontal();
        BgStyle.normal.background = DB.DBTexture[27];
        GUILayout.Box("", nullStylle, GUILayout.Width(3));

        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;


        GUI.color = enable ? Corr(145, 39, 74, 180) : Corr(180, 180, 180, 255);
        if (!enable)
        {
           

            if (GUILayout.Button("EDIT MESH") && Selection.activeGameObject != null)
            {

                if (Selection.activeGameObject.GetComponent<MeshFilter>() == null)
                {
                    // 베텍스 50 이하 에서만 사용가능
                    EditorUtility.DisplayDialog("INFO", "NO MEHS...", "OK");
                    goto DONE; // 
                }

                DICVERTICSPOS.Clear();  // 초기화
                DICVERTICSOBJECT.Clear(); // 초기화

                SELECTOBJECT = Selection.activeGameObject;
                OBJECTMESH = SELECTOBJECT.GetComponent<MeshFilter>().sharedMesh;
                OBJECTVERTICS = OBJECTMESH.vertices;

                foreach (var vert in OBJECTVERTICS)
                {
                    VerticsPos = SELECTOBJECT.transform.TransformPoint(vert);       // 각 vertices의 월드 공간의 좌표를 담음
                    GameObject handle = new GameObject(VerticsPos.ToString());      // 새로운 오브젝트 생성
                    handle.transform.localPosition = VerticsPos;                    // vertices 위치로 이동
                    handle.transform.SetParent(SELECTOBJECT.transform);             // 선택된 오브젝트의 자식으로
                                                                                    //handle.tag = "handle";                                          // 태그 설정
                    DICVERTICSPOS[VerticsPos.ToString()] = VerticsPos;              // 딕셔러니에 이름과 좌표 정보 담음 같은 이름의 오브젝트는 중복이므로 하나만 담긴다..
                }

                //핸들 오브젝트
                foreach (var t in DICVERTICSPOS)
                {
                    GameObject handleGroup = new GameObject(t.Key);                 // 좌표 이름을 가진 오브젝트 생성 
                    handleGroup.transform.localPosition = t.Value;                  // 표지션 적용
                    handleGroup.transform.SetParent(SELECTOBJECT.transform);        // 자식으로
                    handleGroup.AddComponent<DrawGizmo>();                          // 기즈모 넣어줌
                    DICVERTICSOBJECT[t.Key] = handleGroup;                          // 실제 오브젝트 담아줌
                }

                //HANDLES = SELECTOBJECT.GetComponentsInChildren<Transform>().Select(s => s.gameObject).ToArray();

                LISTHANDLES = SELECTOBJECT.GetComponentsInChildren<Transform>().Select(s => s.gameObject).ToList();
                LISTHANDLES.RemoveAt(0);

                foreach (var t in LISTHANDLES)
                {
                    if (t == null) continue;
                    t.transform.SetParent(DICVERTICSOBJECT[t.name].transform);
                    t.transform.localPosition = DICVERTICSOBJECT[t.name].transform.localPosition;
                }

                int nameI = 1;
                foreach (var t in DICVERTICSOBJECT.Keys.ToList())
                {
                    DICVERTICSOBJECT[t].name = nameI.ToString();
                    nameI++;
                }

                enable = true;
                SceneView.RepaintAll();
            }
        }
        else {

            if (GUILayout.Button("COMPLETE") && LISTHANDLES.Count > 0)
            {
                
                foreach (var t in LISTHANDLES)
                {
                    DestroyImmediate(t);
                }
                
                LISTHANDLES.Clear();
                SELECTOBJECT = null;
                enable = false;

               
            }
        }
        
        
        //[ 매쉬 버텍스 50개 이상 일때 ]
        DONE:;
        //============================


        GUI.color = Color.white;
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();

        Footer();
    }

    //하단 상태바
    void Footer()
    {
        
        footerStyle.normal.background = DB.DBTexture[28];
        footerStyle.font = DB.DBFonts[1];
        footerStyle.alignment = TextAnchor.MiddleLeft;
        footerStyle.normal.background = DB.DBTexture[28];
        GUI.Box(new Rect(0, Screen.height - 35, Screen.width, 31), "", footerStyle);
        footerStyle.font = DB.DBFonts[0];
        footerStyle.fontSize = 11;
        footerStyle.normal.background = null;
        footerStyle.alignment = TextAnchor.MiddleLeft;
        //GUISkin SK = new GUISkin();
        
    }


    //----[ OPEN ITEM ZONE ]----
    // 컨덴츠 영역 경계선(가로)
    void ContentsBLine()
    {
        EditorGUILayout.BeginHorizontal();
        boundarylineStyle.normal.background = DB.DBTexture[23];
        GUILayout.Box("", nullStylle, GUILayout.Width(7), GUILayout.Height(3));
        GUILayout.Box("", boundarylineStyle, GUILayout.MaxWidth(Screen.width), GUILayout.Height(3));
        GUILayout.Box("", nullStylle, GUILayout.Width(7), GUILayout.Height(3));
        EditorGUILayout.EndHorizontal();
    }
    // 한줄 라인
    void LineSetting(GUIStyle style, int backgrountTex, float width, float height)
    {
        style.normal.background = DB.DBTexture[backgrountTex];
        GUILayout.Box("", style, GUILayout.MinHeight(width), GUILayout.Height(height));
    }
    // 폰트 스타일 셋팅
    void FontSetting(GUIStyle style, int fontType, int fontSize, TextAnchor align, byte r, byte g, byte b, byte a)
    {
        style.font = DB.DBFonts[fontType];
        style.fontSize = fontSize;
        style.alignment = align;
        style.normal.textColor = Corr(r, g, b, a);
    }
    //컬러 간단
    Color32 Corr(byte r, byte g, byte b, byte a)
    {
        return new Color32(r, g, b, a);
    }
    //----[ CLOSE ITEM ZONE ]----


    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[4] = false;
    }

}
