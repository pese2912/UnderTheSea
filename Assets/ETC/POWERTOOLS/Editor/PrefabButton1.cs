using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class PrefabButton1 : EditorWindow
{
    [MenuItem("NEXTI/PrefabView")]
    static void ToolBarWindow()
    {
        PrefabButton1 window = (PrefabButton1)EditorWindow.GetWindow(typeof(PrefabButton1));
        window.Show();
    }

    PTData DB; // 데이터   

    FileAttributes CHECKDIR;
    DirectoryInfo DIRINFO;
    FileInfo[] PREFABINFO;

    Object FOLDER;
    GameObject PREFAB;
    GameObject APPLYPREFAB;

    Vector2 SCROLLVIEWPREVIEW;

    Texture2D buttonTexture;
    GameObject[] SELECTObject;
    Texture2D[] PREFABTEXTURE;
    List<Texture2D> PREFABTEXTURELIST = new List<Texture2D>();
    List<GameObject> PREFABS = new List<GameObject>();
    List<GameObject> PREFABSCHECK = new List<GameObject>();// 중복체크
    Dictionary<string, FileInfo> DICPREFABSDATA = new Dictionary<string, FileInfo>();

    List<GameObject> SELECTOBJECT = new List<GameObject>();


    //--[ draw map open] --
    GameObject GroupMap;
    List<GameObject> tileMapList = new List<GameObject>();
    List<GameObject> brushObject = new List<GameObject>();
    bool optionDraw;
    bool drawRandom;
    bool drawEnable;
    int mapCol;
    int mapRow;
    string createMapText;
    GameObject currentBrushObject;
    GameObject mapObjRect;
    GameObject mapObjHexa;

    Vector2 mousePos;


    List<GameObject> isDrawMap = new List<GameObject>();
    List<GameObject> isCreatePrefab = new List<GameObject>();

    bool enableEraser;
    bool countOrder;

    Vector3 mapSize;

    bool optionRectMap;
    bool optionHexMap;

    GameObject createBrushObjGroup;


    //-- [draw map close] --



    string prefabName;
    string folderPath;
    string[] DirpathName;
    string returnText;
    string objectName  = "GameObject Name";
    string tagName;

    string addBrushNum; // 브러쉬 넘버

    bool optionReplace;
    bool optionAdd;
    bool optionCreate;

    bool optionReSelName; // replace name select
    bool optionReSelNameContains; // 포함하는 단어
    bool optionReSelTag;  // replace name tag
    bool optionReSelSelected; // 선택된 오브젝트

    //replace option Item
    bool replaceRot;
    bool replaceScale;


    bool optionParent;

    //--[ transform ]--
    bool optionPosition;
    bool optionRotation;
    bool optionScale;

    bool ratio;//정비율

    //--[ random ]--
    bool randomRotation;
    bool randomScale;

    


    float randRotPosY;
    float randScalePosY;

    float randScaleMinValue;
    float randScaleMaxValue;

    int selectionCount;

    Vector3 optionPos;
    Vector3 optionRot;
    Vector3 optionScl;

    Vector2 randRotX;
    Vector2 randRotY;
    Vector2 randRotZ;
    Vector2 randScaleX;
    Vector2 randScaleY;
    Vector2 randScaleZ;

    Vector3 randomRotationV3;
    Vector3 randomScaleV3;

    //--[GUISTYLE]--
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle bgStyle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle toggleStyle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle btnNameStyle = new GUIStyle();
    GUIStyle sceneIcon = new GUIStyle();
    GUIStyle sceneStyle = new GUIStyle();
    


    float yPosCheck; // select type 체크시 상세 옵션 위치 이동.
    float posxGap = 3;

    float ratioRandomScale;

    float buttonWidth;
    float buttonHeight;

    int CreateCount = 1;

    void OnEnable()
    {
        PREFABS.Clear();
        PREFABTEXTURELIST.Clear();
        DICPREFABSDATA.Clear();
        brushObject.Clear();
        LoadData(); // 데이터로드

        nullStylle.normal.background = null;
        FontSetting(btnNameStyle, 1, 10, TextAnchor.MiddleCenter, 50, 50, 50, 255);
        optionScl = Vector3.one;

        randScaleMinValue = 1;
        randScaleMaxValue = 2;

        mapObjRect = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/tile/mapTile.prefab", typeof(GameObject));
        mapObjHexa = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/tile/hex_tile.prefab", typeof(GameObject));

        sceneStyle.font = DB.DBFonts[0];
        sceneStyle.fontSize = 15;
        sceneStyle.alignment = TextAnchor.UpperLeft;
        sceneStyle.normal.textColor = Corr(71, 255, 31, 255);

        sceneIcon.normal.background = DB.DBTexture[99];

        mapSize = Vector3.one;

        optionRectMap = true;
        mapCol = 2;
        mapRow = 2;

        SceneView.onSceneGUIDelegate += this.OnSceneGUI;

        
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }
    // 이미지,폰트 데이터 로드
    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }


    RaycastHit hit;

    Rect rect;

    Texture2D currentBrushObjTex;

    Vector2 mousePosCur;

    Vector3 EditorSceneCameraPos;
    float sceneViewPosZ;
    Vector2 iconPos;

    public void OnSceneGUI(SceneView sceneview)
    {

        // 플레이 상태에서 실행 안되게.
        if (Application.isPlaying == true)
        {
            return;
        }

        
        if (drawEnable)
        {

            // 현재 선택된 오브젝트 표시
            if (currentBrushObject == null && !enableEraser) currentBrushObjTex = DB.DBTexture[101];
            if (currentBrushObject != null && !enableEraser) currentBrushObjTex = AssetPreview.GetAssetPreview(currentBrushObject);
            if (enableEraser) currentBrushObjTex = DB.DBTexture[103];
            if (drawRandom && !enableEraser) currentBrushObjTex = DB.DBTexture[102];



            //scene에서 GUI 시작.
            Handles.BeginGUI();
            //커서 숨기기
            //mousePosCur = Event.current.mousePosition;
            //if((mousePosCur.x > 0 && mousePosCur.x < Screen.width) && (mousePosCur.y > 0 && mousePosCur.y < Screen.height))
            //{
            //    Cursor.visible = false;
            //}
            //else
            //{
            //    Cursor.visible = true;
            //}


            rect = new Rect(25, 55, Screen.height / 7.5f, Screen.height / 7.5f);
            if (GUI.Button(rect, currentBrushObjTex))
            {

            }

            GUI.Label(new Rect(25, 25, 200, 100), "[ DRAW MODE ]", sceneStyle);
            GizmoChange();
            Tools.current = Tool.None;  // 기즈모 안보이게

            Handles.EndGUI();

            DrawPrefab();
        }

        

    }



    int brushCount = 0;
    int brushRandomCount = 0;
    Vector3 posValue;
    int isDrawMapNum;
    void DrawPrefab()
    {
        // 에디터 상태에서 scene뷰 다른 오브젝트 선택 안되게 하기
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        if (brushObject.Count > 0 && countOrder)
        {
            currentBrushObject = brushObject[brushCount];
        }
        if (brushObject.Count > 0 && drawRandom)
        {
            currentBrushObject = brushObject[brushRandomCount];
        }
        //else currentBrushObject = null;
        

        // SCENE DRAW

        Event e = Event.current;
        if ((e.type == EventType.mouseDown || e.type == EventType.mouseDrag) && Event.current.button == 0 && currentBrushObject != null)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            if (Physics.Raycast(ray,out hit))
            {
                // isDrawMap 리스트에 hit 오브젝트 담겨져 있지 않을때만 생성   
                // 프리팹 생성
                if (!isDrawMap.Contains(hit.transform.gameObject) && !isCreatePrefab.Contains(hit.transform.gameObject) && !enableEraser)
                {
                    GameObject create = PrefabUtility.InstantiatePrefab(currentBrushObject) as GameObject;
                    create.transform.position = hit.transform.position;

                    //그룹 밑으로
                    create.transform.SetParent(createBrushObjGroup.transform);
                    

                    isCreatePrefab.Add(create);
                    isDrawMap.Add(hit.transform.gameObject);


                    //--[ 트랜스폼 옵션 적용 ]-----
                    DrawTransform(create);



                    // 브러쉬 카운트 
                    if (brushObject.Count > 1 && countOrder) brushCount++;
                    if (brushCount >= brushObject.Count) brushCount = 0;
               
                    // 브러쉬 랜덤 카운트
                    if (drawRandom && !countOrder) brushRandomCount = Random.Range(0, brushObject.Count);
                }

                //선택한 오브젝트가 브러시로 그린 오브젝트라면
                if(isCreatePrefab.Contains(hit.transform.gameObject) && !enableEraser)
                {



                    //새로 생성
                    GameObject create = PrefabUtility.InstantiatePrefab(currentBrushObject) as GameObject;
                    create.transform.position = hit.transform.position;
                    
                    //그룹 밑으로
                    create.transform.SetParent(createBrushObjGroup.transform);

                    //기존의 리스트에 대입
                    isCreatePrefab[isCreatePrefab.IndexOf(hit.transform.gameObject)] = create;

                    // 기존 오브젝트 제거
                    DestroyImmediate(hit.transform.gameObject);

                    //--[ 트랜스폼 옵션 적용 ]-----
                    DrawTransform(create);

                    // 브러쉬 카운트 
                    if (brushObject.Count > 1 && countOrder) brushCount++;
                    if (brushCount >= brushObject.Count) brushCount = 0;

                    // 브러쉬 랜덤 카운트
                    if (drawRandom && !countOrder) brushRandomCount = Random.Range(0, brushObject.Count);

                }

                // 지우개 모드 
                if (enableEraser && isCreatePrefab.Contains(hit.transform.gameObject))
                {
                    isDrawMap.RemoveAt(isCreatePrefab.IndexOf(hit.transform.gameObject));
                    isCreatePrefab.Remove(hit.transform.gameObject);
                    DestroyImmediate(hit.transform.gameObject);

                }

            }

        }

    }


    void DrawTransform(GameObject create)
    {
        if (optionPosition) create.transform.position = create.transform.position + optionPos;
        if (optionRotation) // NEW Rotation 설정
        {
            if (!randomRotation) create.transform.rotation = Quaternion.Euler(optionRot);
            else
            {
                randomRotationV3.x = RandomNum(randRotX.x, randRotX.y);
                randomRotationV3.y = RandomNum(randRotY.x, randRotY.y);
                randomRotationV3.z = RandomNum(randRotZ.x, randRotZ.y);

                create.transform.rotation = Quaternion.Euler(randomRotationV3);
            }
        }
        if (optionScale)
        {
            if (!randomScale) create.transform.localScale = optionScl;
            else
            {
                if (!ratio)
                {
                    randomScaleV3.x = RandomNum(randScaleX.x, randScaleX.y);
                    randomScaleV3.y = RandomNum(randScaleY.x, randScaleY.y);
                    randomScaleV3.z = RandomNum(randScaleZ.x, randScaleZ.y);
                }
                else
                {
                    ratioRandomScale = RandomNum(randScaleMinValue, randScaleMaxValue);
                    randomScaleV3 = new Vector3(ratioRandomScale, ratioRandomScale, ratioRandomScale);
                }
                create.transform.localScale = randomScaleV3;
            }
        }
    }

    void GizmoChange()
    {
        try
        {
            mousePos =Event.current.mousePosition;
            GUI.color = Corr(255, 255, 255, 170);
            if (!enableEraser) sceneIcon.normal.background = DB.DBTexture[99];
            else sceneIcon.normal.background = DB.DBTexture[100];
            GUI.Box(new Rect(mousePos.x - 10, mousePos.y - 30, 35, 35), "", sceneIcon);
            GUI.color = Color.white;
            SceneView.RepaintAll();
        }
        catch
        {

        }
    }


    void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle,GUILayout.MinWidth(Screen.width),GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "PREFAB PLACER", fontNomalStyle);

    }

    //--[  REPLACE | ADD | NEW ]-- 토글 옵션들 
    float drawPosX;
    void OptionLine()
    {
        drawPosX = 66;

        bgStyle.normal.background = DB.DBTexture[16];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));

        //toggleStyle = GUI.skin.GetStyle("Toggle"); 토글 스킨 가지고 오기

        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(193, 193, 193, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        // label
        GUI.Label(new Rect(25 + posxGap, 24, 78, 25), "REPLACE", fontStyle);
        GUI.Label(new Rect(116 + posxGap, 24, 78, 25), "ADD", fontStyle);
        GUI.Label(new Rect(177 + posxGap, 24, 78, 25), "NEW", fontStyle);

        GUI.Label(new Rect(177 + posxGap + drawPosX, 24, 78, 25), "DRAW", fontStyle);

        // 경계선
        boundarylineStyle.normal.background = DB.DBTexture[20];
        GUI.Box(new Rect(84 + posxGap, 24, 3, 25), "", boundarylineStyle);
        GUI.Box(new Rect(146 + posxGap, 24, 3, 25), "", boundarylineStyle);
        GUI.Box(new Rect(146 + posxGap+drawPosX, 24, 3, 25), "", boundarylineStyle);

        // toggle
        GUI.color = Corr(120, 120, 120, 255);
        optionReplace = GUI.Toggle(new Rect(7, 28, 78, 25), optionReplace, "");
        if (optionReplace)
        {
            if (optionAdd || optionCreate || optionDraw) ResetOption();

            optionAdd = false;
            optionCreate = false;
            optionDraw = false;
        }
        optionAdd = GUI.Toggle(new Rect(96 + posxGap, 28, 50, 25), optionAdd, "");
        if (optionAdd)
        {
            if (optionReplace || optionCreate || optionDraw) ResetOption();

            optionReplace = false;
            optionCreate = false;
            optionDraw = false;
        }
        optionCreate = GUI.Toggle(new Rect(158 + posxGap, 28, 50, 25), optionCreate, "");
        if (optionCreate)
        {
            if (optionAdd || optionReplace || optionDraw) ResetOption();

            optionAdd = false;
            optionReplace = false;
            optionDraw = false;
        }

        optionDraw = GUI.Toggle(new Rect(158 + posxGap+drawPosX, 28, 78, 25), optionDraw, "");
        if (optionDraw)
        {
            if (optionAdd || optionReplace || optionCreate) ResetOption();

            optionAdd = false;
            optionReplace = false;
            optionCreate = false;
        }

        GUI.color = Color.white;
    }

    // name | tag | selection |  draw option
    void OptionReplace()
    {
        
        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("SELECT TYPE", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSeeting(optionStyle, 19, Screen.width, 21); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(27, 71, 3, 21), "NAME", optionStyle);
        GUI.Label(new Rect(91, 71, 3, 21), "TAG", optionStyle);
        GUI.Label(new Rect(150, 71, 3, 21), "SELECTION", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(61 , 71 , 2, 21), "", boundarylineStyle);
        GUI.Box(new Rect(121, 71, 2, 21), "", boundarylineStyle);


        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        optionReSelName = GUI.Toggle(new Rect(7, 73, 40, 25), optionReSelName, ""); // SELECT NAME
        if (optionReSelName)
        {
            optionReSelTag = false;
            optionReSelSelected = false;
        }
        optionReSelTag = GUI.Toggle(new Rect(72, 73, 40, 25), optionReSelTag, ""); // SELECT TAG
        if (optionReSelTag)
        {
            optionReSelName = false;
            optionReSelSelected = false;
        }
        optionReSelSelected = GUI.Toggle(new Rect(132, 73, 40, 25), optionReSelSelected, ""); // selection
        if (optionReSelSelected)
        {
            optionReSelTag = false;
            optionReSelName = false;
        }

        GUI.color = Color.white;
    }


    void OptionNew()
    {

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        
        GUILayout.Box("CREATE GAMEOBJECT", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[19];
        GUI.Box(new Rect(0, 70, Screen.width, 21), "", optionStyle);
        GUILayout.Box("", nullStylle,GUILayout.Width(60), GUILayout.Height(21));

        GUI.color = Corr(170, 170, 170, 255);
        // 생성될 개수
        CreateCount = EditorGUILayout.IntField("", CreateCount, GUILayout.Width(50));
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(9, 70, 3, 21), "COUNT :", optionStyle);
        
        
    }

    float hexaIpos;
    float hexaJpos;



    void OptionDraw()
    {


        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일


        GUILayout.Box("DRAW MODE", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[19];
        GUI.Box(new Rect(0, 70, Screen.width, 21), "", optionStyle);
        GUILayout.Box("", nullStylle, GUILayout.Width(75), GUILayout.Height(21));

        GUI.color = Corr(170, 170, 170, 255);
        // 생성될 개수
        mapCol = EditorGUILayout.IntField("", mapCol, GUILayout.Width(30));
        mapRow = EditorGUILayout.IntField("", mapRow, GUILayout.Width(30));

        GUILayout.Box("", nullStylle, GUILayout.Width(15), GUILayout.Height(21));

        // CREATE MAP 버튼
        createMapText = GroupMap == null ? "CREATE MAP" : "COMPLETE";
        GUI.color = GroupMap == null ? Corr(150, 150, 150, 255) : Corr(200, 100, 100, 255);
        if (GUI.Button(new Rect(150, 72, 90, 17), createMapText, buttonStyle) && (optionRectMap || optionHexMap))
        {

            if (GroupMap == null )
            {
                GroupMap = new GameObject();
                GroupMap.name = "GroupMap";
                GroupMap.transform.position = Vector3.zero;

                createBrushObjGroup = new GameObject();
                createBrushObjGroup.name = "MapGroup";
                createBrushObjGroup.transform.position = Vector3.zero;

                for (int i = 0; i < mapCol; i++)
                {
                    for (int j = 0; j < mapRow; j++)
                    {
                        // 타일 맵 생성
                        if (optionRectMap)
                        {
                            GameObject create = PrefabUtility.InstantiatePrefab(mapObjRect) as GameObject;
                            create.name = "isTileMap";
                            create.transform.position = new Vector3(1 * i, 0, 1 * j);
                            create.transform.SetParent(GroupMap.transform);
                            tileMapList.Add(create);

                        }
                        if (optionHexMap)
                        {
                            //-0.75,0.433
                            //0.866
                            if (i % 2 != 0 ) hexaJpos = 0.433f;
                            else hexaJpos = 0;

                            GameObject create = PrefabUtility.InstantiatePrefab(mapObjHexa) as GameObject;
                            create.name = "isTileMap";
                            create.transform.position = new Vector3(0.75f * i , 0, 0.866f * j + hexaJpos);
                            create.transform.SetParent(GroupMap.transform);
                            tileMapList.Add(create);
                        }

                    }
                }
                GroupMap.hideFlags = HideFlags.HideInHierarchy;
                createBrushObjGroup.hideFlags = HideFlags.HideInHierarchy;
                drawEnable = true;
            }
            else
            {
                createBrushObjGroup.hideFlags = HideFlags.None;
                if(createBrushObjGroup.transform.childCount <= 0)
                {
                    DestroyImmediate(createBrushObjGroup);
                }

                DestroyImmediate(GroupMap);
                GroupMap = null;
                tileMapList.Clear();
                isDrawMap.Clear();
                isCreatePrefab.Clear();
                currentBrushObject = null;
                drawEnable = false;
            }

            //create.transform.position = SceneView.lastActiveSceneView.pivot;
        }


        GUILayout.Box("", nullStylle, GUILayout.Width(82));
        optionRectMap = GUILayout.Toggle(optionRectMap, " RECTANGLE", GUILayout.Width(95));
        if (optionRectMap)
        {
            optionHexMap = false;
        }
        optionHexMap = GUILayout.Toggle(optionHexMap, " HEXAGON", GUILayout.Width(100));
        if (optionHexMap)
        {
            optionRectMap = false;
        }

        EditorGUILayout.EndHorizontal();

        //COL 가로 ROW 세로 --;
        //Label
        optionStyle.normal.background = null;

        GUI.Label(new Rect(9, 70, 3, 21), "COL ROW", optionStyle);
      //  GUI.Label(new Rect(250, 70, 3, 21), "MAP TYPE", optionStyle);
        //GUI.Label(new Rect(73, 70, 3, 21), "ROW", optionStyle);


        // 리사이즈 맵
        optionStyle.normal.background = DB.DBTexture[7];
        GUI.color = Corr(255, 255, 255, 80);
        GUI.Box(new Rect(0, 70 + 21, Screen.width, 1), "", optionStyle);
        GUI.color = Color.white;
        optionStyle.normal.background = DB.DBTexture[19];
        GUI.Box(new Rect(0, 70 + 22, Screen.width, 21), "", optionStyle);

        GUI.Label(new Rect(9, 70+22, 3, 21), "MAP SIZE", optionStyle);

        GUILayout.BeginHorizontal();

        GUILayout.Box("", nullStylle, GUILayout.Width(75) ,GUILayout.Height(22));

        GUI.color = Corr(170, 170, 170, 255);
        mapSize.x = EditorGUILayout.FloatField(mapSize.x,GUILayout.Width(30));
        mapSize.z = EditorGUILayout.FloatField(mapSize.z, GUILayout.Width(30));

        GUILayout.EndHorizontal();


        //맵 리사이즈
        GUI.color = Corr(150,150,150,255);
        if (GUI.Button(new Rect(150, 72+22, 90, 17), "RESIZE MAP", buttonStyle) && GroupMap != null)
        {
            GroupMap.transform.localScale = mapSize;

            for (int i = 0; i < isDrawMap.Count; i++)
            {
                //Debug.Log(isDrawMap[i].transform.position);
                //Debug.Log(isCreatePrefab[i].transform.position);

                isCreatePrefab[i].transform.position = isDrawMap[i].transform.position;
            }

            SceneView.RepaintAll();
        }


        
        // 전체 그리기. full draw
        if (GUI.Button(new Rect(247, 72+22, 90, 17), "FULL DRAW", buttonStyle)  && GroupMap != null)
        {
            if (currentBrushObject != null )
            {
                if (isCreatePrefab.Count > 0)
                {
                    foreach (var t in isCreatePrefab)
                    {
                        DestroyImmediate(t);
                    }
                    isCreatePrefab.Clear();
                    isDrawMap.Clear();
                }

                if (!enableEraser) // 지우개 모드 아닐때
                {
                    foreach (var t in tileMapList)
                    {
                        if (countOrder)
                        {
                            // 브러쉬 카운트 
                            if (brushObject.Count > 1 && countOrder) brushCount++;
                            if (brushCount >= brushObject.Count) brushCount = 0;
                            currentBrushObject = brushObject[brushCount];
                        }
                        if (drawRandom)
                        {
                            brushRandomCount = Random.Range(0, brushObject.Count);
                            currentBrushObject = brushObject[brushRandomCount];
                        }

                        GameObject create = PrefabUtility.InstantiatePrefab(currentBrushObject) as GameObject;
                        create.transform.position = t.transform.position;
                        create.transform.SetParent(createBrushObjGroup.transform);

                        DrawTransform(create);

                        isDrawMap.Add(t);
                        isCreatePrefab.Add(create);
                    }
                }

                SceneView.RepaintAll();
            }
        }

        GUI.color = Color.white;

    }


    // NAME | TAG | SELECTION 선택 했을때 나오는 컨덴츠
    void NameAndTag()
    {
        EditorGUILayout.Space();

        

        // 네임 선택
        if (optionReSelName) 
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", nullStylle, GUILayout.Width(4));
            FontSetting(fontStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);
            GUI.Label(new Rect(27, 96, 100, 25), "Contains", fontStyle);

            optionReSelNameContains = GUILayout.Toggle(optionReSelNameContains,"",GUILayout.Width(80));

            //GUILayout.Toggle()
            GUI.color = Corr(180, 180, 180, 255);
            objectName = GUILayout.TextField(objectName, GUILayout.Width(Screen.width-99)); //텍스트 입력
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        // 태그 선택
        if (optionReSelTag)
        {
            
            EditorGUILayout.BeginHorizontal();

            FontSetting(fontStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);

            //GUILayout.Label("SELECT TAG", fontStyle , GUILayout.Width(70));
            GUI.Label(new Rect(9, 96, 100, 25), "Select TAG", fontStyle);
            GUILayout.Box("", nullStylle, GUILayout.Width(83));
            // 태그선택
            GUI.color = Corr(180, 180, 180, 255);
            tagName = EditorGUILayout.TagField("", tagName, GUILayout.Width(Screen.width - 96));
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }


        // 셀렉션 선택
        if (optionReSelSelected)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", nullStylle, GUILayout.Width(3));
            GUI.color = Corr(150, 150, 150, 255);
            EditorGUILayout.HelpBox("Selection Objests : "+ selectionCount, MessageType.None);
            GUI.color = Color.white;
            GUILayout.Box("", nullStylle, GUILayout.Width(3));
            EditorGUILayout.EndHorizontal();
            
        }
    }

    
    //REPLACE OPTION = Rotation | Scale 리플레이스 부분 - 원본 값 따라가기.( 로테이션 스케일 )
    void OptionReplaceItems()
    {
        if (optionReSelName || optionReSelTag) yPosCheck = 129;
        else if (optionReSelSelected) yPosCheck = 130;
        else yPosCheck = 113;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(4));
        
        optionStyle.normal.background = DB.DBTexture[24];
        GUI.Box(new Rect(0, yPosCheck - 3, Screen.width, 22), "", optionStyle); // GARY BG
        GUI.color = Corr(226, 226, 226, 180);
        replaceRot = GUILayout.Toggle(replaceRot,  " Rotation",  GUILayout.Width(70));
        replaceScale = GUILayout.Toggle(replaceScale, " Scale (Copy Values)",  GUILayout.Width(150));
        //optionReSelNameContains = GUILayout.Toggle(optionReSelNameContains, " Parent", GUILayout.Width(70));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    //--[ ADD ITEM | ADD 옵션 부분 ]--
    void OptionAddItems()
    {
        if (optionReSelName || optionReSelTag) yPosCheck = 129;
        else if (optionReSelSelected) yPosCheck = 130;
        else yPosCheck = 113;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(4));

        optionStyle.normal.background = DB.DBTexture[24];
        GUI.Box(new Rect(0, yPosCheck - 3, Screen.width, 22), "", optionStyle); // GARY BG
        GUI.color = Corr(226, 226, 226, 180);

        optionPosition = GUILayout.Toggle(optionPosition, " Position", GUILayout.Width(67));
        optionRotation = GUILayout.Toggle(optionRotation, " Rotation", GUILayout.Width(68));
        optionScale = GUILayout.Toggle(optionScale, " Scale", GUILayout.Width(57));
        optionParent = GUILayout.Toggle(optionParent, " Parent", GUILayout.Width(68));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Height(8));

        if (optionPosition) optionPos = EditorGUILayout.Vector3Field("add position", optionPos, GUILayout.Width(Screen.width - 30));

        if (optionRotation)
        {
            optionRot = EditorGUILayout.Vector3Field("add Rotation", optionRot, GUILayout.Width(Screen.width - 30));
        }

        if (optionScale)
        {
            optionScl = EditorGUILayout.Vector3Field("add scale", optionScl, GUILayout.Width(Screen.width - 30));
        }


        if (optionPosition || optionRotation || optionScale)
        {
            
           // ContentsBLine();
            
        }
    }

    //--[ NEW ITEM | NEW 옵션 부분 ]--
    void OptionNewItems()
    {

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(4));

        optionStyle.normal.background = DB.DBTexture[24];
        GUI.Box(new Rect(0, 93, Screen.width, 22), "", optionStyle); // GARY BG
        GUI.color = Corr(226, 226, 226, 180);

        optionPosition = GUILayout.Toggle(optionPosition, " Position", GUILayout.Width(67));
        optionRotation = GUILayout.Toggle(optionRotation, " Rotation", GUILayout.Width(68));
        optionScale = GUILayout.Toggle(optionScale, " Scale", GUILayout.Width(57));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Height(8));


        //랜덤 버튼 위치 [R]
        if (optionRotation) randRotPosY = 134;
        if (optionPosition && optionRotation) randRotPosY = 168;
        if (optionScale) randScalePosY = 134;
        if ((optionPosition && optionScale) || (optionRotation && optionScale)) randScalePosY = 168;
        if (optionPosition && optionRotation && optionScale) randScalePosY = 202;


        if (optionPosition)
        {
            optionPos = EditorGUILayout.Vector3Field("position", optionPos, GUILayout.Width(Screen.width - 30));

        }
        if (optionRotation)
        {
            
            if (!randomRotation) // 랜덤이 아닐때
            {
                EditorGUILayout.BeginHorizontal();
                optionRot = EditorGUILayout.Vector3Field("rotation", optionRot, GUILayout.Width(Screen.width - 30));
            }
            else // 랜덤일때
            {
                //randRotPosY -= 15;
                randScalePosY += 2;
                GUILayout.Label("random rotation (0~360)");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box("", nullStylle, GUILayout.Width(20));
                GUILayout.Box("X", nullStylle, GUILayout.Width(10));
                EditorGUILayout.MinMaxSlider(ref randRotX.x, ref randRotX.y, 0, 360);
                GUILayout.Box("Y", nullStylle, GUILayout.Width(10));
                EditorGUILayout.MinMaxSlider(ref randRotY.x, ref randRotY.y, 0, 360);
                GUILayout.Box("Z", nullStylle, GUILayout.Width(10));
                EditorGUILayout.MinMaxSlider(ref randRotZ.x, ref randRotZ.y, 0, 360);
                GUILayout.Box("", nullStylle, GUILayout.Width(23));

                
            }

            GUI.color = !randomRotation ? Color.white : Color.green;
            if (GUI.Button(new Rect(Screen.width - 23, randRotPosY, 18, 18), "R", buttonStyle))
            {
                randomRotation = !randomRotation;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

        }
        if (optionScale)
        {

            if (!randomScale)
            {
                EditorGUILayout.BeginHorizontal();
                optionScl = EditorGUILayout.Vector3Field("scale", optionScl, GUILayout.Width(Screen.width - 30));
            }
            else
            {

                
                GUILayout.Label("random scale",GUILayout.Width(80));
                EditorGUILayout.BeginHorizontal();
                GUI.color = Corr(150, 150, 150, 150);
                bgStyle.normal.background = DB.DBTexture[24];
                GUI.Box(new Rect(0,randScalePosY-1,Screen.width,21),"", bgStyle);
                GUILayout.Label("Min", GUILayout.Width(23));
                randScaleMinValue = EditorGUILayout.FloatField( randScaleMinValue,GUILayout.Width(37),GUILayout.Height(15));
                GUILayout.Label("Max", GUILayout.Width(27));
                randScaleMaxValue = EditorGUILayout.FloatField( randScaleMaxValue, GUILayout.Width(37), GUILayout.Height(15));
                
                //정비율 토글
                GUILayout.Label("", GUILayout.Width(3));
                ratio = GUILayout.Toggle(ratio,"" ,GUILayout.Width(12));
                GUILayout.Label("Ratio");
                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (!ratio)
                {
                    
                    GUILayout.Box("", nullStylle, GUILayout.Width(20));
                    GUILayout.Box("X", nullStylle, GUILayout.Width(10));
                    EditorGUILayout.MinMaxSlider(ref randScaleX.x, ref randScaleX.y, randScaleMinValue, randScaleMaxValue);
                    GUILayout.Box("Y", nullStylle, GUILayout.Width(10));
                    EditorGUILayout.MinMaxSlider(ref randScaleY.x, ref randScaleY.y, randScaleMinValue, randScaleMaxValue);
                    GUILayout.Box("Z", nullStylle, GUILayout.Width(10));
                    EditorGUILayout.MinMaxSlider(ref randScaleZ.x, ref randScaleZ.y, randScaleMinValue, randScaleMaxValue);
                    GUILayout.Box("", nullStylle, GUILayout.Width(23));
                    
                }

            }

            GUI.color = !randomScale ? Color.white : Color.green;
            if (GUI.Button(new Rect(Screen.width - 23, randScalePosY, 18, 18), "R", buttonStyle))
            {
                randomScale = !randomScale;
            }
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
        }
    }

    //--[ DRAW ITEM | DRAW 옵션 부분 ]--
    void OptionDrawItems()
    {

        //첫번째 줄
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(4));

        optionStyle.normal.background = DB.DBTexture[24];
        GUI.Box(new Rect(0, GUILayoutUtility.GetLastRect().position.y, Screen.width, 22), "", optionStyle); // GARY BG
        GUI.color = Corr(226, 226, 226, 180);

        countOrder = GUILayout.Toggle(countOrder, " Order", GUILayout.Width(70));
        if (countOrder) drawRandom = false;
        drawRandom = GUILayout.Toggle(drawRandom, " Random", GUILayout.Width(70));
        if (drawRandom) countOrder = false;
         enableEraser = GUILayout.Toggle(enableEraser, " Eraser", GUILayout.Width(70));
        GUILayout.EndHorizontal();


        // 두번째 줄
        GUILayout.Box("", nullStylle, GUILayout.Height(8));
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(4));

        GUI.color = Color.white;
        GUI.Box(new Rect(0, GUILayoutUtility.GetLastRect().position.y-1, Screen.width, 22), "", optionStyle); // GARY BG

        GUI.color = Corr(226, 226, 226, 180);
        optionPosition = GUILayout.Toggle(optionPosition, " Position", GUILayout.Width(70));
        optionRotation = GUILayout.Toggle(optionRotation, " Rotation", GUILayout.Width(70));
        optionScale = GUILayout.Toggle(optionScale, " Scale", GUILayout.Width(70));
        
        
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Height(8));


        //랜덤 버튼 위치 [R]
        if (optionRotation) randRotPosY = 134;
        if (optionPosition && optionRotation) randRotPosY = 168;
        if (optionScale) randScalePosY = 134;
        if ((optionPosition && optionScale) || (optionRotation && optionScale)) randScalePosY = 168;
        if (optionPosition && optionRotation && optionScale) randScalePosY = 202;


        if (optionPosition)
        {
            optionPos = EditorGUILayout.Vector3Field("position", optionPos, GUILayout.Width(Screen.width - 30));

        }
        if (optionRotation)
        {

            if (!randomRotation) // 랜덤이 아닐때
            {
                EditorGUILayout.BeginHorizontal();
                optionRot = EditorGUILayout.Vector3Field("rotation", optionRot, GUILayout.Width(Screen.width - 30));
            }
            else // 랜덤일때
            {
                //randRotPosY -= 15;
                randScalePosY += 2;
                GUILayout.Label("random rotation (0~360)");
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box("", nullStylle, GUILayout.Width(20));
                GUILayout.Box("X", nullStylle, GUILayout.Width(10));
                EditorGUILayout.MinMaxSlider(ref randRotX.x, ref randRotX.y, 0, 360);
                GUILayout.Box("Y", nullStylle, GUILayout.Width(10));
                EditorGUILayout.MinMaxSlider(ref randRotY.x, ref randRotY.y, 0, 360);
                GUILayout.Box("Z", nullStylle, GUILayout.Width(10));
                EditorGUILayout.MinMaxSlider(ref randRotZ.x, ref randRotZ.y, 0, 360);
                GUILayout.Box("", nullStylle, GUILayout.Width(23));


            }

            GUI.color = !randomRotation ? Color.white : Color.green;
            if (GUI.Button(new Rect(Screen.width - 23, randRotPosY+46, 18, 18), "R", buttonStyle))
            {
                randomRotation = !randomRotation;
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

        }
        if (optionScale)
        {

            if (!randomScale)
            {
                EditorGUILayout.BeginHorizontal();
                optionScl = EditorGUILayout.Vector3Field("scale", optionScl, GUILayout.Width(Screen.width - 30));
            }
            else
            {


                GUILayout.Label("random scale", GUILayout.Width(80));
                EditorGUILayout.BeginHorizontal();
                GUI.color = Corr(150, 150, 150, 150);
                bgStyle.normal.background = DB.DBTexture[24];
                GUI.Box(new Rect(0, randScalePosY + 46, Screen.width, 21), "", bgStyle);
                GUILayout.Label("Min", GUILayout.Width(23));
                randScaleMinValue = EditorGUILayout.FloatField(randScaleMinValue, GUILayout.Width(37), GUILayout.Height(15));
                GUILayout.Label("Max", GUILayout.Width(27));
                randScaleMaxValue = EditorGUILayout.FloatField(randScaleMaxValue, GUILayout.Width(37), GUILayout.Height(15));

                //정비율 토글
                GUILayout.Label("", GUILayout.Width(3));
                ratio = GUILayout.Toggle(ratio, "", GUILayout.Width(12));
                GUILayout.Label("Ratio");
                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (!ratio)
                {

                    GUILayout.Box("", nullStylle, GUILayout.Width(20));
                    GUILayout.Box("X", nullStylle, GUILayout.Width(10));
                    EditorGUILayout.MinMaxSlider(ref randScaleX.x, ref randScaleX.y, randScaleMinValue, randScaleMaxValue);
                    GUILayout.Box("Y", nullStylle, GUILayout.Width(10));
                    EditorGUILayout.MinMaxSlider(ref randScaleY.x, ref randScaleY.y, randScaleMinValue, randScaleMaxValue);
                    GUILayout.Box("Z", nullStylle, GUILayout.Width(10));
                    EditorGUILayout.MinMaxSlider(ref randScaleZ.x, ref randScaleZ.y, randScaleMinValue, randScaleMaxValue);
                    GUILayout.Box("", nullStylle, GUILayout.Width(23));

                }

            }

            GUI.color = !randomScale ? Color.white : Color.green;
            if (GUI.Button(new Rect(Screen.width - 23, randScalePosY+46, 18, 18), "R", buttonStyle))
            {
                randomScale = !randomScale;
            }
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
        }
    }


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
    void LineSeeting(GUIStyle style, int backgrountTex, float width, float height)
    {
        style.normal.background = DB.DBTexture[backgrountTex];
        GUILayout.Box("", style, GUILayout.MinHeight(width), GUILayout.Height(height));
    }
    // 폰트 스타일 셋팅
    void FontSetting(GUIStyle style, int fontType,int fontSize,TextAnchor align, byte r, byte g, byte b, byte a)
    {
        style.font = DB.DBFonts[fontType];
        style.fontSize = fontSize;
        style.alignment = align;
        style.normal.textColor = Corr(r, g, b, a);
    }

    // 시작 메세지(설명)
    void StartMessage()
    {
        GUILayout.Box("",nullStylle,GUILayout.Height(3));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        //GUI.color = Corr(150, 150, 150, 255);
        GUI.color = Corr(153, 153, 153, 255);
        EditorGUILayout.HelpBox("Please Checked Option And Register your prefab button", MessageType.Info);
        GUI.color = Color.white;
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();

        GUILayout.Box("", nullStylle, GUILayout.Height(6));
    }

    //--------------------------------------------------------------------------------//
    //--[ Replace 적용 ]--
    void ApplyReplace(GameObject prefab)
    {
        if (optionReSelName) NameObject();
        if (optionReSelTag) TagObject();
        if (optionReSelSelected) SelectObject();

        if (SELECTOBJECT.Count > 0)
        {
            for (int i = 0; i < SELECTOBJECT.Count; i++)
            {
                APPLYPREFAB = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                APPLYPREFAB.transform.position = SELECTOBJECT[i].transform.position;
                if (replaceRot) APPLYPREFAB.transform.rotation = SELECTOBJECT[i].transform.rotation;
                if (replaceScale) APPLYPREFAB.transform.localScale = SELECTOBJECT[i].transform.localScale;
                if (optionReSelTag) APPLYPREFAB.tag = tagName;
                DestroyImmediate(SELECTOBJECT[i]);
                
            }
        }
    }
    
    //--[ ADD 적용 ]--
    void ApplyAddObject(GameObject prefab)
    {
        if (optionReSelName) NameObject();
        if (optionReSelTag) TagObject();
        if (optionReSelSelected) SelectObject();

        if (SELECTOBJECT.Count > 0)
        {
            for (int i = 0; i < SELECTOBJECT.Count; i++)
            {
                APPLYPREFAB = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (optionPosition) APPLYPREFAB.transform.position = (SELECTOBJECT[i].transform.position + optionPos);
                if (optionRotation) APPLYPREFAB.transform.rotation = Quaternion.Euler(optionRot);
                if (optionScale) APPLYPREFAB.transform.localScale = SELECTOBJECT[i].transform.localScale + optionScl;
                if (optionParent) APPLYPREFAB.transform.SetParent(SELECTOBJECT[i].transform);
            }
        }
    }
    
    //--[ NEW 적용 ]--
    void ApplyNewObject(GameObject prefab)
    {
        if(CreateCount > 0)
        {
            for(int i = 0; i< CreateCount; i++)
            {
                APPLYPREFAB = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (optionPosition) APPLYPREFAB.transform.position = optionPos;
                else APPLYPREFAB.transform.position =  SceneView.lastActiveSceneView.pivot;
                if (optionRotation) // NEW Rotation 설정
                {
                    if(!randomRotation) APPLYPREFAB.transform.rotation = Quaternion.Euler(optionRot);
                    else
                    {
                        randomRotationV3.x = RandomNum(randRotX.x, randRotX.y);
                        randomRotationV3.y = RandomNum(randRotY.x, randRotY.y);
                        randomRotationV3.z = RandomNum(randRotZ.x, randRotZ.y);

                        APPLYPREFAB.transform.rotation = Quaternion.Euler(randomRotationV3);
                    }

                }
                if (optionScale)
                {
                    if (!randomScale) APPLYPREFAB.transform.localScale = optionScl;
                    else
                    {
                        if (!ratio)
                        {
                            randomScaleV3.x = RandomNum(randScaleX.x, randScaleX.y);
                            randomScaleV3.y = RandomNum(randScaleY.x, randScaleY.y);
                            randomScaleV3.z = RandomNum(randScaleZ.x, randScaleZ.y);
                        }
                        else
                        {
                            ratioRandomScale = RandomNum(randScaleMinValue, randScaleMaxValue);
                            randomScaleV3 = new Vector3(ratioRandomScale, ratioRandomScale, ratioRandomScale);
                        }
                        Debug.Log(randomScaleV3);
                        APPLYPREFAB.transform.localScale = randomScaleV3;
                    }
                }
            }
        }
    }
    float RandomNum(float min, float max)
    {
        return Random.Range(min, max);
    }
    //--[ 토글 새로 체크시 옵션 값 리셋 ]--
    void ResetOption()
    {
        optionPosition = false;
        optionRotation = false;
        optionScale = false;
        optionParent = false;
        optionReSelName = false;
        optionReSelTag = false;
        optionReSelSelected = false;
        replaceRot = false;
        replaceScale = false;
        optionPos = Vector3.zero;
        optionRot = Vector3.zero;
        optionScl = Vector3.one;
        ratio = false;
        randomRotation = false;
        randomScale = false;
    }


    //--[ 지정 오브젝트 담기 ]--
    void SelectObject()
    {
        SELECTOBJECT = Selection.gameObjects.ToList();
    }
    void NameObject()
    {
        if (!optionReSelNameContains)
        { 
            SELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => g.name == objectName).ToList();
        }
        else
        {
            SELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => g.name.Contains(objectName)).ToList();
        }
    }
    void TagObject()
    {
        SELECTOBJECT = GameObject.FindGameObjectsWithTag(tagName).ToList();
    }

    void SelectCount()
    {
        if(Selection.gameObjects.Length > 0)
        {
            selectionCount = Selection.gameObjects.Length;
        }
    }
    

    void OnGUI()
    {
        
        SelectCount(); // 선택된 오브젝트

        TopLine();
        OptionLine();
        LineSeeting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

        if (optionReplace)
        {
            OptionReplace();
            LineSeeting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
            NameAndTag();
            if (optionReSelName || optionReSelTag || optionReSelSelected)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                OptionReplaceItems();
            }
        }

        if (optionAdd)
        {
            OptionReplace();
            LineSeeting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
            NameAndTag();
            if (optionReSelName || optionReSelTag || optionReSelSelected)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                OptionAddItems();
            }
        }

        if (optionCreate)
        {
            OptionNew();
            LineSeeting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
            OptionNewItems();
        }

        if (optionDraw)
        {
            OptionDraw();
            LineSeeting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
            OptionDrawItems();
        }

        if(!optionReplace && !optionAdd && !optionCreate && !optionDraw)
        {
            StartMessage();
        }
        else
        {
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //ContentsBLine();//컨덴츠 경계선
        GUI.color = Color.gray;
        //EditorGUILayout.HelpBox("Customized Prefab Palette", MessageType.None);
        GUI.color = Color.white;
        ContentsBLine();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        FOLDER = (Object)EditorGUILayout.ObjectField("Input Folder", FOLDER, typeof(Object),false); //booltype : scene의 오브젝트를 드래그 해서 넣을 수 있는가?
        if (GUI.changed && FOLDER != null)
        {
            folderPath = AssetDatabase.GetAssetPath(FOLDER);
            CHECKDIR = File.GetAttributes(folderPath);

            if (CHECKDIR == FileAttributes.Directory)
            {
                // 디렉터리 정보 로드
                DIRINFO = new DirectoryInfo(folderPath);
                // 디렉터리에 담긴 모든 오브젝트를 PREFABINFO[] 안에 넣어준다.

                DICPREFABSDATA.Clear();

                foreach (var t in DIRINFO.GetFiles())
                {
                    if (t.Name.Contains(".meta") && t.Name.Contains(".prefab")) continue;
                    else if (t.Name.Contains(".prefab")) DICPREFABSDATA[t.ToString()] = t;
                }

                PREFABSCHECK.Clear();
                PREFABSCHECK = DICPREFABSDATA.Values.Select(s => (GameObject)AssetDatabase.LoadAssetAtPath(DirPathNameFactory(s.ToString()), typeof(GameObject))).ToList();
                foreach (var t in PREFABSCHECK)
                {
                    if (!PREFABS.Contains(t)) PREFABS.Add(t);
                }

                PREFABTEXTURELIST.Clear();
                AssetPreview.SetPreviewTextureCacheSize(0);
                AssetPreview.SetPreviewTextureCacheSize(5000);
            }
            else
            {
                string title1 = "Error Meassage";
                string msg1 = "폴더가 아닙니다. 폴더를 선택해 주세요.\n Must Imported Folder!";
                EditorUtility.DisplayDialog(title1, msg1, "OK");
            }

            FOLDER = null;
           if(mouseOverWindow) Repaint();
        }

        PREFAB = (GameObject)EditorGUILayout.ObjectField("Input Prefab", PREFAB, typeof(GameObject), false);
        if (GUI.changed)
        {
            if (AssetDatabase.GetAssetPath(PREFAB).Contains(".prefab") && !PREFABS.Contains((GameObject)PREFAB))
            {
                PREFABS.Add((GameObject)PREFAB);
            }
            PREFAB = null;
            if (mouseOverWindow) Repaint();
        }

        EditorGUILayout.Space();

        // MESH 필터가 있는 오브젝트만 배열에 담음
        EditorGUILayout.BeginHorizontal();

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        GUI.color = Corr(180, 180, 180, 255);
        if (GUILayout.Button("LOAD SELECTED PREFABS" , buttonStyle, GUILayout.Height(17)) && Selection.objects.Length > 0)
        {
            foreach (var t in Selection.objects)
            {
                if (AssetDatabase.GetAssetPath(t).Contains(".prefab") && !PREFABS.Contains((GameObject)t)) PREFABS.Add((GameObject)t);
            }
        }
        GUI.color = Corr(180, 180, 180, 255);
        if (GUILayout.Button("REMOVE ALL", buttonStyle, GUILayout.Height(17),GUILayout.Width(100)) && PREFABS.Count > 0)
        {
            PREFABS.Clear();
            PREFABTEXTURELIST.Clear();
            DICPREFABSDATA.Clear();
            brushObject.Clear();
            FOLDER = null;
        }
        EditorGUILayout.EndHorizontal();

        // 사이 띄우기
        GUILayout.Box("", nullStylle, GUILayout.Height(0));

        buttonWidth = 70;
        int columns = Mathf.FloorToInt(Screen.width / 75);
        GUI.color = new Color(0.3f, 0.3f, 0.3f);
        GUILayout.BeginHorizontal(GUI.skin.box);
        SCROLLVIEWPREVIEW = GUILayout.BeginScrollView(SCROLLVIEWPREVIEW);
        GUILayout.Box("", nullStylle,GUILayout.Height(4));
        GUILayout.BeginHorizontal();

        //--[ PREVIEW BUTTONS ]--//
        int num = 0;
        for (int i = 0; i < PREFABS.Count; i++)
        {
            // 프리팹이 지워졌을때
            if (PREFABS[i] == null)
            {
              //  MapMagEditor.RandomFrefabOn = false;
              //  MapMag.selectPrefab = null;
                SceneView.RepaintAll();
                PREFABS.RemoveAt(i);
                brushObject.Clear();
                if (mouseOverWindow) Repaint();
            }

            try // 사용자가 임의로 프리팹을 지웠을때.
            {

                if (i % columns == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }

                

                if (AssetPreview.GetAssetPreview(PREFABS[i]) == null)
                {
                    buttonTexture = AssetPreview.GetMiniThumbnail(PREFABS[i]);
                    
                }
                else
                {
                    buttonTexture =  AssetPreview.GetAssetPreview(PREFABS[i]);
                }

                GUILayout.BeginVertical(GUILayout.Width(buttonWidth));

                if (brushObject.Contains(PREFABS[i]))
                {
                    addBrushNum = "" + (brushObject.IndexOf(PREFABS[i]) + 1); // 브러쉬 넘버 추가
                    GUI.color = Corr(200, 100, 100, 255);
                }
                else
                {
                    addBrushNum = "+";
                    GUI.color = Color.white;
                }
                // 리스트 버튼 클릭
                if (GUILayout.Button(buttonTexture, GUILayout.Width(buttonWidth+5), GUILayout.Height(buttonWidth+5)))
                {
                    //버튼 눌렀을때 모든 옵션 해제
                    if (enableEraser || countOrder || drawRandom) { enableEraser = false;  countOrder = false; drawRandom = false; }

                    if (optionReplace) ApplyReplace(PREFABS[i]);
                    if (optionAdd) ApplyAddObject(PREFABS[i]);
                    if (optionCreate) ApplyNewObject(PREFABS[i]);
                    if (optionDraw && !countOrder) currentBrushObject = PREFABS[i];
                    

                    // 옵션 체크 안되어 있을때 씬에 1개 생기도록한다.
                    if ( !optionReplace && !optionAdd && !optionCreate && !optionDraw) 
                    {
                        GameObject create = PrefabUtility.InstantiatePrefab(PREFABS[i])as GameObject;
                        create.name = PREFABS[i].name;
                        create.transform.position = SceneView.lastActiveSceneView.pivot;
                    }
                    
                    
                    SceneView.RepaintAll();
                }
                GUI.color = new Color(0.3f, 0.3f, 0.3f);

                // 프리팹 이름
                prefabName = PREFABS[i].name;
                if (prefabName.Length > 11) prefabName = prefabName.Substring(0, 10);
                EditorGUILayout.HelpBox(prefabName, MessageType.None);
                //GUILayout.Box(prefabName, btnNameStyle);

                GUI.color = Corr(100, 100, 100, 120);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("-", buttonStyle, GUILayout.Width(buttonWidth / 2)))
                {
                    brushObject.Remove(PREFABS[i]);
                    PREFABS.RemoveAt(i);
                    SceneView.RepaintAll();
                }
                // "+" 브러쉬 추가 버튼
                GUI.color = brushObject.Contains(PREFABS[i]) ? Corr(200, 100, 100, 255) : Corr(100, 100, 100, 120);
                if (GUILayout.Button(addBrushNum, buttonStyle, GUILayout.Width(buttonWidth / 2)) && optionDraw)
                {
                    if(!brushObject.Contains(PREFABS[i])) brushObject.Add(PREFABS[i]);
                    else brushObject.Remove(PREFABS[i]);
                    SceneView.RepaintAll();
                }
              


                GUILayout.EndHorizontal();
                
                GUILayout.EndVertical();
                if (mouseOverWindow) Repaint();
                GUI.color = Color.white;
            }

            catch
            {
                // 사용자가 임의로 프리팹을 지웠을때.
                brushObject.Clear();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        // Reload
        //if (GUILayout.Button("REMOVE ALL PREFABS DATA", GUILayout.MaxWidth(220)))
        //{
        //    PREFABS.Clear();
        //    PREFABTEXTURELIST.Clear();
        //    DICPREFABSDATA.Clear();
        //    FOLDER = null;
        //}

        //GUI.color = PREFABS.Count >= 50 ? Color.yellow : Color.white;
        GUI.color = Corr(100,100,100,255);
        EditorGUILayout.HelpBox(" Prefab Buttons : " + PREFABS.Count ,  MessageType.None);
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
    }

    //경로 수정
    string DirPathNameFactory(string pathName)
    {
        DirpathName = pathName.Split(new string[] { "Assets" }, System.StringSplitOptions.None);
        returnText = "Assets" + DirpathName[1];
        return returnText;
    }

    //컬러 간단
    Color32 Corr(byte r, byte g, byte b, byte a)
    {
        return new Color32(r, g, b, a);
    }

    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[0] = false;
        if (GroupMap != null)
        {
            createBrushObjGroup.hideFlags = HideFlags.None;

            if (createBrushObjGroup.transform.childCount <= 0)
            {
                DestroyImmediate(createBrushObjGroup);
            }

            DestroyImmediate(GroupMap);
            GroupMap = null;
            tileMapList.Clear();
            isDrawMap.Clear();
            isCreatePrefab.Clear();
            currentBrushObject = null;
            drawEnable = false;
        }
    }

}

