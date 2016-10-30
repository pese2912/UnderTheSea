using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Enum = System.Enum;

public class Transfer : EditorWindow
{


    PTData DB;

    GUIStyle BgStyle = new GUIStyle();
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle footerStyle = new GUIStyle();
    GUIStyle sceneStyle = new GUIStyle();
    GUIStyle sceneIcon = new GUIStyle();


    List<GameObject> SELECTOBJECT = new List<GameObject>();
    Vector2 MOUSEPOS;
    Vector2 CURRENTMOUSEPOS;
    Vector3 TARGETPOS;
    Vector2 pos;
    Rect rect;

    Vector3 EditorSceneCameraPos;
    float sceneViewPosZ;

    float targetPosX, targetPosY, targetPosZ;
    float selectObjPos;

    float posSubTextPosy;
    float posSubTogglePosy;

    bool enable, optionPOSX, optionPOSY, optionPOSZ;
    bool optionPOSITION;
    bool optionSCALE;
    bool optionROTATION;

    float togglePosY, optionTextPosY;

    int SelectObjectCount;

    string buttonText;

    void OnEnable()
    {
        optionPOSX = true;
        optionPOSY = true;
        optionPOSZ = true;

        LoadData();  //데이터 로드

        sceneStyle.font = DB.DBFonts[0];
        sceneStyle.fontSize = 15;
        sceneStyle.alignment = TextAnchor.UpperLeft;
        sceneStyle.normal.textColor = Corr(71, 255, 31, 255);
        sceneIcon.normal.background = DB.DBTexture[30];

        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    //저장된 데이터 로드
    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }





    void GizmoChange()
    {
        EditorSceneCameraPos = SceneView.lastActiveSceneView.camera.transform.position;
        sceneViewPosZ = (EditorSceneCameraPos.z * 170 / 100);

        try { 
        foreach (var t in SELECTOBJECT)
        {
            pos = HandleUtility.WorldToGUIPoint(t.transform.position);
            GUI.color = Corr(255, 255, 255, 170);
            GUI.Box(new Rect(pos.x- (sceneViewPosZ/2), pos.y- (sceneViewPosZ/2), sceneViewPosZ, sceneViewPosZ),"" ,sceneIcon);
            GUI.color = Color.white;
        }
        }
        catch
        {
            ResetObj();
        }
    }

    // 선택된 오브젝트가 지워 졌을때
    void ResetObj()
    {
        if (enable)
        {
            SELECTOBJECT.Clear();
            enable = false;
        }
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
            GUI.Label(new Rect(25, 25, 200, 100),"[ TRASNFER MODE ]", sceneStyle);
            GizmoChange();
        }
        Handles.EndGUI();

        // - Transfer 실행
        Event e = Event.current;
        if (e.type == EventType.mouseDown && enable)
        {
            MOUSEPOS = Event.current.mousePosition;
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(MOUSEPOS);

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit);

                try
                {
                    foreach (var t in SELECTOBJECT.ToList())
                    {
                        Undo.RecordObject(t.transform, t.name);
                        EditorUtility.SetDirty(t);

                        targetPosX = TargetPosition(optionPOSX, hit.transform, t.transform, "x");
                        targetPosY = TargetPosition(optionPOSY, hit.transform, t.transform, "y");
                        targetPosZ = TargetPosition(optionPOSZ, hit.transform, t.transform, "z");

                        TARGETPOS = new Vector3(targetPosX, targetPosY, targetPosZ);
                        t.transform.position = TARGETPOS;
                        if (optionSCALE) t.transform.localScale = hit.transform.localScale;
                        if (optionROTATION) t.transform.rotation = hit.transform.rotation;
                    }
                }
                catch
                {
                    ResetObj();
                }
            }
        }


        // --[ Transfer ON ]--
        if (enable && SELECTOBJECT.Count > 0)
        {
            Tools.current = Tool.None;  // 기즈모 안보이게
            foreach (var t in SELECTOBJECT.ToList())
            {
               // t.GetComponent<Renderer>().sharedMaterial.color = Color.red;
            }

            CURRENTMOUSEPOS = Event.current.mousePosition;
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(CURRENTMOUSEPOS);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.GetComponent<MeshRenderer>() != null)
                {
                    foreach(var t in SELECTOBJECT.ToList())
                    {
                        Handles.DrawLine(t.transform.position, hit.transform.position);
                        HandleUtility.Repaint();
                    }
                }
            }
        }

        // --[ Transfer OFF & Reset]--
        else
        {
            if (SELECTOBJECT.Count > 0)
            {
                SELECTOBJECT.Clear();
            }
        }
    }



    //--[ 타이틀 라인 ]--
    void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "TRANSFER", fontNomalStyle);
    }
    //--[ OPTIONS ]--
    void Options()
    {
        togglePosY = 48;
        optionTextPosY = 46;


        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("SELECT OPTIONS", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인



        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(27, optionTextPosY, 3, 21),  "POSITION", optionStyle);
        GUI.Label(new Rect(122, optionTextPosY, 3, 21),  "ROTATION", optionStyle);
        GUI.Label(new Rect(218 , optionTextPosY, 3, 21),"SCALE", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(92, 46, 2, 21), "", boundarylineStyle);
        GUI.Box(new Rect(188 , 46, 2, 21), "", boundarylineStyle);


        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        optionPOSITION = GUI.Toggle(new Rect(7, togglePosY, 40, 25), optionPOSITION, ""); // SELECT POSITION
        if (optionPOSITION)
        {
           
        }
        optionROTATION = GUI.Toggle(new Rect(102, togglePosY, 40, 25), optionROTATION, ""); // SELECT TAG
        if (optionROTATION)
        {

        }
        optionSCALE = GUI.Toggle(new Rect(198 , togglePosY, 40, 25), optionSCALE, ""); // selection
        if (optionSCALE)
        {
        }
        GUI.color = Color.white;
    }

    //--[ OPTIONS ITEMS ]--
    void Items()
    {


        posSubTogglePosy = 72;
        posSubTextPosy = 69;
        BgStyle.normal.background = DB.DBTexture[26];
        GUILayout.Box("", BgStyle, GUILayout.Height(1));
        BgStyle.normal.background = DB.DBTexture[7];
        GUILayout.Box("", BgStyle, GUILayout.Height(24));
        //포지션 분리토글

        GUI.color = Corr(120, 120, 120, 255);
        optionPOSX = GUI.Toggle(new Rect(7, posSubTogglePosy, 40, 25), optionPOSX, ""); // SELECT POSITION
        optionPOSY = GUI.Toggle(new Rect(47, posSubTogglePosy, 40, 25), optionPOSY, ""); // SELECT TAG
        optionPOSZ = GUI.Toggle(new Rect(87, posSubTogglePosy, 40, 25), optionPOSZ, ""); // selection
        GUI.color = Color.white;                                                                                 

        optionStyle.normal.textColor = Corr(193, 193, 193, 255);
        GUI.Label(new Rect(27, posSubTextPosy, 3, 21), "X", optionStyle);
        GUI.Label(new Rect(67, posSubTextPosy, 3, 21), "Y", optionStyle);
        GUI.Label(new Rect(107, posSubTextPosy, 3, 21),"Z", optionStyle);




        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

    }

    // --[ GUI ]--
    void ButtonTextChabge()
    {
        if (!enable) buttonText = "LOAD & ENABLE";
        else buttonText = "COMPLETE & DISABLE";
    }

    

    void OnGUI()
    {

        ButtonTextChabge();

        TopLine();
        LineSetting(boundarylineStyle, 29, Screen.width, 1); // 밝은 경계선
        Options();

       
        if (optionPOSITION) Items();
        else LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

        if (!enable && !optionPOSITION && !optionROTATION && !optionSCALE)
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
        // enable = EditorGUILayout.Toggle("Enable", enable); // enable Option

        if (GUILayout.Button(buttonText) && Selection.gameObjects.Length > 0 &&(optionPOSITION|| optionROTATION|| optionSCALE))
        {
            SELECTOBJECT = Selection.gameObjects.ToList();
            enable = !enable;
        }
        if (enable)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ArrowPlus);
            SceneView.RepaintAll();
        }

        GUI.color = Color.white;
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();

        // optionPOSX = EditorGUILayout.Toggle("Postion X", optionPOSX);
        // optionPOSY = EditorGUILayout.Toggle("Postion Y", optionPOSY);
        // optionPOSZ = EditorGUILayout.Toggle("Postion Z", optionPOSZ);
        // optionSCALE = EditorGUILayout.Toggle("Scale", optionSCALE);
        // optionROTATION = EditorGUILayout.Toggle("Rotation", optionROTATION);

        Footer();
    }

    //하단 상태바
    void Footer()
    {
       if(mouseOverWindow) Repaint();
        footerStyle.normal.background = DB.DBTexture[28];
        footerStyle.font = DB.DBFonts[1];
        footerStyle.alignment = TextAnchor.MiddleLeft;
        footerStyle.normal.textColor = Corr(90, 90, 90, 255);
        GUI.Box(new Rect(0, Screen.height - 54, 137, 31), " SELECT OBJECTS : ", footerStyle);
        footerStyle.normal.background = DB.DBTexture[28];
        GUI.Box(new Rect(137, Screen.height - 54, Screen.width, 31), "", footerStyle);
        footerStyle.font = DB.DBFonts[0];
        footerStyle.fontSize = 11;
        footerStyle.normal.background = null;
        footerStyle.alignment = TextAnchor.MiddleLeft;

        if (Selection.gameObjects.Length > 0) SelectObjectCount = Selection.gameObjects.Length;
        else SelectObjectCount = 0;

        GUI.Box(new Rect(125, Screen.height - 54, 32, 32), SelectObjectCount.ToString(), footerStyle);

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


    float TargetPosition(bool option, Transform TargetPos, Transform MyPos, string type)
    {
        if (option)
        {
            switch (type)
            {
                case "x":
                    selectObjPos = TargetPos.position.x;
                    break;
                case "y":
                    selectObjPos = TargetPos.position.y;
                    break;
                case "z":
                    selectObjPos = TargetPos.position.z;
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case "x":
                    selectObjPos = MyPos.position.x;
                    break;
                case "y":
                    selectObjPos = MyPos.position.y;
                    break;
                case "z":
                    selectObjPos = MyPos.position.z;
                    break;
            }
        }
        return selectObjPos;
    }
    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[5] = false;
    }


}
