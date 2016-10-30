using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class PivotEditor : EditorWindow
{

    PTData DB;

    Mesh PIVOTMESH; 
    Bounds BOUNDS;
    Collider COLLIDER;
    Renderer rend;           // 월드 좌표상의 vertives의 중심점.

    Renderer rendBound;

    Vector2 sceneGizmoPos;
    Vector3[] MESHVERTICS;   // mesh vertices.
    Vector3[] VERTICSValue;  // 최초 vertices 값.

    Vector3 CONTROLPOS;      // 이동될 값
    Vector3 BOUNDValue;      // 박스 콜라이더 위치(중심)
    Vector3 StartPos;        // 시작위치 ( 슬라이더 )

    Vector3 boundColPos; // 콜라이더 위치 재입력

    Vector3 firstPostion; // 콜라이더 위치 재입력을 위한

    GameObject SELECTOBJECT; // 선택된 오브젝트
    GameObject PIVOTOBJECT;  // 피봇 오브젝트

    

    GUIStyle BgStyle = new GUIStyle();
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle bgStyle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle fontFildStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle footerStyle = new GUIStyle();
    GUIStyle sliderLabelStyle = new GUIStyle();

    GUIStyle sceneStyle = new GUIStyle();
    GUIStyle sceneIcon = new GUIStyle();

    DirectoryInfo DIRINFO;
    FileInfo[] FILEINFO;

    bool OptionSceneOnEnable;
    bool OptionSliderOnEnable;
    bool OptionPivotPoint;

    bool optionSceneEdit, optionSlider , optionCopy;

    bool enable;

    bool firstCenter; // 슬라이더 콘트롤시 처음에 중심점을 리셋 한다.

    float pivotPosX, pivotPosY, pivotPosZ;
    float sliderPivotX, sliderPivotY, sliderPivotZ;
    float topLabelPosX;

    float topLabelPosX2;

    float valueNull;

    float gizmoBoxSize;

    string pivotName = "PivotObj";

    string mainBtnText;
    string meshName = "New Mesh Name";

    string colliderType;

    void OnEnable()
    {
        LoadData();  //데이터 로드
        // scene view 렌더링 ( update )
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        
        //this.minSize = new Vector2(200, 185);
        nullStylle.normal.background = null;

        fontStyle.font = DB.DBFonts[1];
        fontStyle.fontSize = 11;
        fontStyle.margin.top = 5;
        fontStyle.margin.left = 4;
        fontStyle.normal.textColor = Corr(50, 50, 50, 255);


        sliderLabelStyle.font = DB.DBFonts[1];
        sliderLabelStyle.fontSize = 11;
        sliderLabelStyle.normal.textColor = Corr(50, 50, 50, 255);

        sceneStyle.font = DB.DBFonts[0];
        sceneStyle.fontSize = 15;
        sceneStyle.alignment = TextAnchor.UpperLeft;
        sceneStyle.normal.textColor = Corr(71, 255, 31, 255);
        sceneIcon.normal.background = DB.DBTexture[42];

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


        try
        {

            // SCENE 콘트롤
            if (optionSceneEdit && enable)
            {

                // SCENE GUI(TEXT)
                Handles.BeginGUI();
                GUI.Label(new Rect(25, 25, 200, 100), "[ SCENE PIVOT EDIT MODE ]", sceneStyle);
                GizmoChange();
                Handles.EndGUI();

                    Repaint();

                    Selection.activeGameObject = PIVOTOBJECT; // PIVOTOBJECT 강제 선택
                    SELECTOBJECT.transform.rotation = PIVOTOBJECT.transform.rotation;

                    Event clickUp = Event.current;
                    if (clickUp.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        CONTROLPOS = PIVOTOBJECT.transform.position;
                        SELECTOBJECT.transform.position = CONTROLPOS;

                        for (int i = 0; i < MESHVERTICS.Length; i++)
                        {
                            MESHVERTICS[i] = VERTICSValue[i] + (CONTROLPOS * -1); //  VERTICSValue[i] = 최초 생성시 CONTROLPOS - 한 값이 되어야 한다.
                        }

                        PIVOTMESH.vertices = MESHVERTICS; // vercices 재정의
                    }
            }

            // SLIDER 콘트롤
            if (optionSlider && enable)
            {
                    Repaint();
                    CONTROLPOS = new Vector3(pivotPosX * BOUNDS.extents.x, pivotPosY * BOUNDS.extents.y, pivotPosZ * BOUNDS.extents.z);

                    SELECTOBJECT.transform.position = StartPos - (CONTROLPOS * -1);
                    Vector3[] MESHVERTICS = PIVOTMESH.vertices;
                    for (int i = 0; i < MESHVERTICS.Length; i++)
                    {
                        MESHVERTICS[i] = VERTICSValue[i] + (CONTROLPOS * -1);
                    }
                    PIVOTMESH.vertices = MESHVERTICS;
            }


        }
        catch
        {   // 사용자가 임의로 지웠을때
            SELECTOBJECT = null;
            enable = false;
            Reset();
        }

    }

    //저장된 데이터 로드
    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }

    void GizmoChange()
    {
        if(PIVOTOBJECT != null)
        {
            gizmoBoxSize = 40;
            sceneGizmoPos = HandleUtility.WorldToGUIPoint(PIVOTOBJECT.transform.position); 
            GUI.color = Corr(255, 255, 255, 170);
            GUI.Box(new Rect(sceneGizmoPos.x - (gizmoBoxSize/2), sceneGizmoPos.y - (gizmoBoxSize / 2), gizmoBoxSize, gizmoBoxSize), "", sceneIcon);
            GUI.color = Color.white;
            // 하이어 라키 상태에서 숨기기.
            PIVOTOBJECT.hideFlags = HideFlags.HideInHierarchy; 
        }
    }

    //--[ 타이틀 라인 ]--
    void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "PIVOT EDITOR", fontNomalStyle);
    }

    //--[  SLIDER | SCENE | COPY ]-- 메인 옵션
    void OptionLine()
    {
        topLabelPosX = 15;
        topLabelPosX2 = 94;
        
        bgStyle.normal.background = DB.DBTexture[16];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));


        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(193, 193, 193, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        // label
        GUI.Label(new Rect(25  , 24, 40, 25), "SLIDER", fontStyle);
        GUI.Label(new Rect(93+ topLabelPosX, 24, 40, 25), "SCENE", fontStyle);
        GUI.Label(new Rect(93 + topLabelPosX2, 24, 40, 25), "COPY", fontStyle);


        // 경계선
        boundarylineStyle.normal.background = DB.DBTexture[20];
        GUI.Box(new Rect(61+ topLabelPosX, 24, 3, 25), "", boundarylineStyle);
        GUI.Box(new Rect(61 + topLabelPosX2, 24, 3, 25), "", boundarylineStyle);

        // toggle
        GUI.color = Corr(120, 120, 120, 255);
        optionSlider = GUI.Toggle(new Rect(7, 28, 50, 25), optionSlider, "");
        if (optionSlider)
        {
            optionSceneEdit = false;  optionCopy = false;

        }
        optionSceneEdit = GUI.Toggle(new Rect(72 + topLabelPosX, 28, 50, 25), optionSceneEdit, "");
        if (optionSceneEdit)
        {
            optionSlider = false; optionCopy = false;
        }
        optionCopy = GUI.Toggle(new Rect(72 + topLabelPosX2, 28, 50, 25), optionCopy, "");
        if (optionCopy)
        {
            optionSceneEdit = false;  optionSlider = false;
        }


        GUI.color = Color.white;
    }


   

    void Slider(string label, ref float sliderValue)
    {
        

        fontStyle.normal.textColor = Corr(50, 50, 50, 255);
        EditorGUILayout.Space();

        bgStyle.normal.background = DB.DBTexture[27];
        GUI.Box(new Rect(5, GUILayoutUtility.GetLastRect().position.y+11, Screen.width-65, 11), "", bgStyle);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Corr(180, 180, 180, 255);

        GUILayout.Label(label, fontStyle, GUILayout.Width(55));
        sliderValue = EditorGUILayout.Slider(sliderValue, -1, 1);

        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    void SliderButton(string label, ref float sliderValue,byte r, byte g, byte b,string type)
    {

        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        EditorGUILayout.Space();
        GUI.Box(new Rect(5, GUILayoutUtility.GetLastRect().position.y + 9, Screen.width-70, 17), "", bgStyle);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));

        sliderLabelStyle.margin.top = 6;
        sliderLabelStyle.margin.left = 10;
        
        GUILayout.Label(label, sliderLabelStyle, GUILayout.Width(49));


        GUI.color = Corr(r, g, b, 255);

        if (type == "3BTN")
        {
            if (GUILayout.Button("LEFT", buttonStyle, GUILayout.Height(17))) sliderValue = -1;
            if (GUILayout.Button("CENTER", buttonStyle, GUILayout.Height(17))) sliderValue = 0;
            if (GUILayout.Button("RIGHT", buttonStyle, GUILayout.Height(17))) sliderValue = 1;
        }
        else
        {
            if (GUILayout.Button("RESET", buttonStyle, GUILayout.Height(17)))
            {
                pivotPosX = 0;  pivotPosY = 0;  pivotPosZ = 0;
            }
        }

        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();


    }

    //--[ OPTION SLIDER ]--
    void OptionSlider()
    {

     

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("SLIDER CONTROL", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선


        Slider("PIVOT X", ref pivotPosX);
        Slider("PIVOT Y", ref pivotPosY);
        Slider("PIVOT Z", ref pivotPosZ);

        SliderButton("X", ref pivotPosX, 200, 160, 160, "3BTN");
        SliderButton("Y", ref pivotPosY, 160, 200, 160, "3BTN");
        SliderButton("Z", ref pivotPosZ, 160, 160, 200, "3BTN");
        SliderButton("¤", ref valueNull, 160, 160, 160, "1BTN");


        GUI.color = Color.white;
    }

    //--[ OPTION SCENE ]--
    void OptionScene()
    {
        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("SCENE VIEW CONTROL", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(27, 71, 3, 21), "SNAP POINT", optionStyle);



        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        OptionPivotPoint = GUI.Toggle(new Rect(7, 73, 40, 25), OptionPivotPoint, ""); // SELECT NAME
        if (OptionPivotPoint)
        {
            //optionPosition = false;
        }

        GUI.color = Color.white;
    }
    
    //--[ OPTION COPY ]--
    void OptionCopy()
    {
        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("COPY MESH", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();


    }

    //-- [ OPTION COPY ITEM ]--
    void OptionCopyItem()
    {
        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

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
            GUI.FocusControl("");// 포커스 아웃
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
            SELECTOBJECT = null;
        }
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
    }


    void MainButtonText()
    {
     if(!enable)   mainBtnText = "LOAD & EDIT PIVOT";
     else mainBtnText = "COMPLETE";

    }


    void Reset()
    {
        pivotPosX = 0;
        pivotPosY = 0;
        pivotPosZ = 0;
    }

    // 콜라이더 확인하고 지우기
    void ColliderCheck(GameObject col)
    {
        if (col.GetComponent<Collider>() != null)
        {
            
            colliderType = col.GetComponent<Collider>().GetType().Name;
            DestroyImmediate(col.GetComponent<Collider>());
        }
        else colliderType = "null";
    }

    //콜라이더 다시 넣기
    void ColliderApply(GameObject col , string colName)
    {
        firstPostion = col.transform.position;
        if (colName == "null") return;
        ColPos(col);
        Debug.Log(boundColPos);
        //Debug.Log(boundColPos);
        //Debug.Log(rendBound.bounds.center);
        switch (colName)
        {
            case "BoxCollider":
                col.AddComponent<BoxCollider>();
                //col.GetComponent<BoxCollider>().center = boundColPos;
                break;
            case "SphereCollider":
                col.AddComponent<SphereCollider>();
                //col.GetComponent<SphereCollider>().center = boundColPos;
                break;
            case "CapsuleCollider":
                col.AddComponent<CapsuleCollider>();
                //col.GetComponent<CapsuleCollider>().center = boundColPos;
                break;
            case "MeshCollider":
                col.AddComponent<MeshCollider>();
                break;
        }
    }

    void ColPos(GameObject col)
    {
        boundColPos = col.GetComponent<MeshFilter>().sharedMesh.bounds.center;
    }

    void OnGUI()
    {
        MainButtonText();

        TopLine();
        OptionLine();
        if (!optionSlider)
        {
            LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
        }
        else
        {
            //GUI.FocusControl("");// 포커스 아웃
            LineSetting(boundarylineStyle, 29, Screen.width, 1); // 밝은 경계선
            OptionSlider();
        }
        if (optionCopy)
        {
            //LineSetting(boundarylineStyle, 29, Screen.width, 1); // 밝은 경계선
            OptionCopy();
            OptionCopyItem();

        }
        if (optionSceneEdit)
        {
         
            OptionScene();
            LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

            if (OptionPivotPoint)
            {
                GUILayout.Box("", nullStylle, GUILayout.Height(4));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box("", nullStylle, GUILayout.Width(3));

                EditorGUILayout.HelpBox("키보드에 'V' 키를 눌러 SNAP를 활성화 하세요.", MessageType.Info);

                GUILayout.Box("", nullStylle, GUILayout.Width(3));
                EditorGUILayout.EndHorizontal();
                GUILayout.Box("", nullStylle, GUILayout.Height(6));
            }


            //사이 띄우기
            if (enable)
            {
                BgStyle.normal.background = DB.DBTexture[27];

                GUILayout.Box("", nullStylle, GUILayout.Height(4));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box("", nullStylle, GUILayout.Width(3));

                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.font = DB.DBFonts[1];
                buttonStyle.fontSize = 11;
                GUI.color = Corr(180, 180, 180, 255);

                if (GUILayout.Button("CENTER PIVOT", buttonStyle))
                {
                    CenterPivot();
                    CenterPivot();
                    PIVOTOBJECT.transform.position = SELECTOBJECT.transform.position;
                }

                GUI.color = Color.white;
                GUILayout.Box("", nullStylle, GUILayout.Width(3));
                EditorGUILayout.EndHorizontal();
            }
        }

        //시작 메세지
        if (!optionSlider && !optionSceneEdit && !optionCopy)
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








        //---[ ENABLE버튼 ] ---

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
        if (optionCopy) GUI.color = Corr(40, 40, 40, 50);
        if (GUILayout.Button(mainBtnText) && Selection.activeGameObject != null && (optionSlider || optionSceneEdit) && Selection.activeGameObject.activeSelf == true)
        {
            Reset();
            SceneView.RepaintAll();
            if (enable)
            {
                Selection.activeGameObject = SELECTOBJECT; // 선택 오브젝트
                //ColliderApply(SELECTOBJECT, colliderType);
                SELECTOBJECT = null;
                firstCenter = false;
                DestroyImmediate(PIVOTOBJECT);
                PIVOTOBJECT = null;
            }
            else
            {
                //로드가 안되는 상황
                //1.선택된 오브젝트의 매쉬가 없는 경우.
                //2.자식으로 되어 있는 경우.
                //3.부모로 되어 있는 경우
                if (Selection.activeGameObject.GetComponent<MeshRenderer>() == null ||  Selection.activeGameObject.transform.childCount > 0 ||  Selection.activeGameObject.transform.parent != null)
                {
                    EditorUtility.DisplayDialog("INFO", "선택된 오브젝트에 매쉬가 없거나 부모 혹은 자식 오브젝트 입니다.", "OK");
                    goto Done; // 버튼 체크
                }
                else
                {
                    GetInfo();
                    ColliderCheck(SELECTOBJECT);
                    SELECTOBJECT.transform.rotation = Quaternion.Euler(Vector3.zero);//로테이션 초기화
                }
            }

            enable = !enable;
        }
        Done:; // 버튼 체크

        GUI.color = Color.white;
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();

        // 슬라이더 콘트롤시 씬을 매 프레임 재경신
        if(optionSlider && enable)
        {
            SceneView.RepaintAll();
            //로테이션 초기화 해야 됨.
        }


        //-----------------------------------------------------//



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
    }

    void CenterPivotSlider()
    {

        //SELECTOBJECT.transform.position = rend.bounds.center;

        Vector3 endpoint = SELECTOBJECT.transform.position;
        Vector3 startPoint = rend.bounds.center;
        Vector3 point = endpoint - startPoint;

        Vector3[] MESHVERTICS = PIVOTMESH.vertices;
        Vector3[] STARTVERTICS = PIVOTMESH.vertices;


        for (int i = 0; i < MESHVERTICS.Length; i++)
        {
            MESHVERTICS[i] = STARTVERTICS[i] + point;
        }
        PIVOTMESH.vertices = MESHVERTICS;
        PIVOTMESH.RecalculateBounds();
        try
        {
            //((BoxCollider)COLLIDER).center = BOUNDValue + point;
        }
        catch
        {

        }


         endpoint = SELECTOBJECT.transform.position;
         startPoint = rend.bounds.center;
         point = endpoint - startPoint;

         MESHVERTICS = PIVOTMESH.vertices;
         STARTVERTICS = PIVOTMESH.vertices;


        for (int i = 0; i < MESHVERTICS.Length; i++)
        {
            MESHVERTICS[i] = STARTVERTICS[i] + point;
        }
        PIVOTMESH.vertices = MESHVERTICS;
        PIVOTMESH.RecalculateBounds();
        try
        {
            //((BoxCollider)COLLIDER).center = BOUNDValue + point;
        }
        catch
        {

        }



        SELECTOBJECT = null;
        firstCenter = true;
        GetInfo();
    }



    void CenterPivot()
    {

        //if (Selection.gameObjects[0] != SELECTOBJECT)

        GetInfo();  // 참조 변수
        SELECTOBJECT.transform.position = rend.bounds.center;

        Vector3 endpoint = SELECTOBJECT.transform.position;
        Vector3 startPoint = rend.bounds.center;
        Vector3 point = endpoint - startPoint;

        Vector3[] MESHVERTICS = PIVOTMESH.vertices;
        Vector3[] STARTVERTICS = PIVOTMESH.vertices;


        for (int i = 0; i < MESHVERTICS.Length; i++)
        {
            MESHVERTICS[i] = STARTVERTICS[i] + point; 
        }
        PIVOTMESH.vertices = MESHVERTICS;
        PIVOTMESH.RecalculateBounds();
        try
        {
            ((BoxCollider)COLLIDER).center = BOUNDValue + (rend.bounds.center * -1);
        }
        catch
        {

        }

    }
    


    void GetInfo()
    {
       // Debug.Log(SELECTOBJECT);

        if(SELECTOBJECT == null)
        { 
            SELECTOBJECT = Selection.gameObjects[0];
            PIVOTMESH = SELECTOBJECT.GetComponent<MeshFilter>().sharedMesh;

            rend = SELECTOBJECT.GetComponent<Renderer>();

            //Debug.Log("sss");

            StartPos = SELECTOBJECT.transform.position;
            //StartPos = BOUNDS.center;
            BOUNDS = PIVOTMESH.bounds;
            COLLIDER = SELECTOBJECT.GetComponent<Collider>();
            MESHVERTICS = PIVOTMESH.vertices;
            VERTICSValue = new Vector3[MESHVERTICS.Length];


            // 최초위치 
            for (int i = 0; i < MESHVERTICS.Length; i++)
            {
                VERTICSValue[i] = MESHVERTICS[i];
            }
            if (optionSceneEdit == true)
            {
                for (int i = 0; i < VERTICSValue.Length; i++)
                {
                    VERTICSValue[i] = VERTICSValue[i] - (SELECTOBJECT.transform.position * -1);
                }
            }
            try
            {
                //BOUNDValue = ((BoxCollider)COLLIDER).center;
            }
            catch
            {

            }
            //슬라이더 콘트롤시 중심점으로.
            if (optionSlider && firstCenter == false)
            {
                CenterPivotSlider();
            }

        }


        if (optionSceneEdit && PIVOTOBJECT == null)
        {
            PIVOTOBJECT = new GameObject();
            PIVOTOBJECT.name = pivotName;
            PIVOTOBJECT.transform.position =  SELECTOBJECT.transform.position;
            //PIVOTOBJECT.transform.rotation = SELECTOBJECT.transform.rotation;
        }
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
        Icon_Menu.btnBools[3] = false;
    }

}
