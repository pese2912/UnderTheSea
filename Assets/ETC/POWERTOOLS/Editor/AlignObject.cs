using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class AlignObject : EditorWindow
{


    PTData DB;

    List<GameObject> SELECTGAMEOBJECTS = new List<GameObject>();
    List<GameObject> FLOORGAMEOBJECTS = new List<GameObject>();

    GameObject test;
    GameObject sets;


    Vector3 standardPoint;
    Vector3 distance;
    Vector3 pervPos;
    Vector3 dRot; // 써클러 개별 로테이션 조절
    

    GameObject floor;

    int num;
    int SelectObjectCount;
    bool enable;
    float x, y, z;
    string live;

    int posxGap2 = 55;

    float minValue = -3;
    float maxValue = 3;
    float optionsPosx;
    float simpleValue;

    float disX, disY, disZ;

    float lastRectPosY;

    //- circler 옵션

    float angle;
    float radius;
    float rot;
    float cirRot;
    float scl;
    float dis; //distance
    Vector3 newPos;

    //----------


    float objRotX, objRotY, objRotZ;

    bool optionLinear, optionCircler; // 메인 옵션
    bool optLineSimle, optLineDetaile; // liner 세부 옵션
    bool optCirclerSimple, optionCircleDetaile; // circler 세부 옵션


    bool cirSimpleX, cirSimpleY, cirSimpleZ; // 써클 심플
    bool cirDetailX, cirDetailY, cirDetailZ; // 써클 디테일



    bool optionSimple;
    bool optionDetaile;

    bool detailX, detailY, detailZ;
    bool simpleX, simpleY, simpleZ;



    float cirRadius;
    float cirValMin;
    float cirValMax;

    float cirAngler;
    float cirAngValMin;
    float cirAngValMax;


    float detailXMin, detailYMin, detailZMin;
    float detailXMax, detailYMax, detailZMax;
    float detailXV, detailYV, detailZV;

    float posy;

    float togglePosY, textPosY;

    string buttonText;

    int selectedObjNum; // 선택된 게임 오브젝트 숫자.

    GUIStyle BgStyle = new GUIStyle();
    GUIStyle bgStyle = new GUIStyle();
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle footerStyle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle fontFildStyle = new GUIStyle();
    GUIStyle sliderStyle = new GUIStyle();
    GUIStyle sceneStyle = new GUIStyle();
    GUIStyle sceneIcon = new GUIStyle();

    Vector2 pos;

    void OnEnable()
    {
        LoadData();  //데이터 로드
        nullStylle.normal.background = null;
        // 사이즈 지정
        //this.minSize = new Vector2(200, 185); 
         optionsPosx = 18;

        detailXMin = -3;
        detailYMin = -3;
        detailZMin = -3;

        detailXMax = 3;
        detailYMax = 3;
        detailZMax = 3;

        simpleValue = 2;

        radius = 3;

        optLineSimle = true;
        optionCircleDetaile = true;

        cirValMax = 10;
        cirAngValMax = 360;

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

        try
        {

            foreach (var t in FLOORGAMEOBJECTS)
            {
                pos = HandleUtility.WorldToGUIPoint(t.transform.position);
                GUI.color = Corr(255, 255, 255, 170);
                GUI.Box(new Rect(pos.x - 5, pos.y - 5, 10, 10), "", sceneIcon);
                GUI.color = Color.white;
            }
        }
        catch
        {
            ResetOBJ();
        }
    }

    void ResetOBJ()
    {
        enable = false;
        Reset();
        FLOORGAMEOBJECTS.Clear();
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
            GUI.Label(new Rect(25, 25, 200, 100), "[ ALIGNER MODE ]", sceneStyle);
            GizmoChange();
            Tools.current = Tool.None;  // 기즈모 안보이게
        }
        Handles.EndGUI();
    }

        //--[ 타이틀 라인 ]--
        void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "ALIGNER", fontNomalStyle);
    }

    //--[  LINEAR | CIRCLER ]-- 토글 옵션들 
    void Options()
    {
        bgStyle.normal.background = DB.DBTexture[16];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));


        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(193, 193, 193, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        // label
        GUI.Label(new Rect(25, 24, 40, 25), "LINEAR", fontStyle);
        GUI.Label(new Rect(93 + optionsPosx, 24, 40, 25), "CIRCLER", fontStyle);

        // 경계선
        boundarylineStyle.normal.background = DB.DBTexture[20];
        GUI.Box(new Rect(61 + optionsPosx, 24, 3, 25), "", boundarylineStyle);

        // toggle
        GUI.color = Corr(120, 120, 120, 255);
        optionLinear = GUI.Toggle(new Rect(7, 28, 50, 25), optionLinear, "");
        if (optionLinear)
        {
            optionCircler = false;
        }
        optionCircler = GUI.Toggle(new Rect(75 + optionsPosx, 28, 50, 25), optionCircler, "");
        if (optionCircler)
        {
            optionLinear = false;
        }
        GUI.color = Color.white;
    }


    //--[ OPTION LINEAR ]--
    void OptionLinear()
    {

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("OPTIONS", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 25); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(25, 72, 40, 25), "SIMPLE", optionStyle);
        GUI.Label(new Rect(93+ optionsPosx, 72, 40, 25), "DETAIL", optionStyle);

        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(61+ optionsPosx, 71, 2, 25), "", boundarylineStyle);


        // toggle
        GUI.color = Corr(120, 120, 120, 255);
        optLineSimle = GUI.Toggle(new Rect(7, 77, 50, 25), optLineSimle, "");
        if (optLineSimle)
        {
            optLineDetaile = false;
        }
        optLineDetaile = GUI.Toggle(new Rect(75 + optionsPosx, 76, 50, 25), optLineDetaile, "");
        if (optLineDetaile)
        {
            optLineSimle = false;
        }
        GUI.color = Color.white;

        if(optLineSimle || optLineDetaile) LineSetting(optionStyle, 43, Screen.width, 3);

        if (optLineSimle) OptionLinearSimple();
        if (optLineDetaile) OptionLineDetail();

    }

    //--[ OPTION LINEAR SIMPLE ]--
    void OptionLinearSimple()
    {

        LineSetting(optionStyle, 19, Screen.width, 25); // 한줄 라인

        lastRectPosY = 28;

        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(10, 73+ lastRectPosY, 3, 21), "DISTANCE", optionStyle);



        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(109, 71+ lastRectPosY, 2, 25), "", boundarylineStyle);
    

        //VALUE
        fontFildStyle = new GUIStyle(GUI.skin.textField);
        FontSetting(fontFildStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);
        GUI.color = Corr(180, 180, 180, 255);
        simpleValue = EditorGUI.FloatField(new Rect(72, 76+ lastRectPosY, 28, 16),simpleValue, fontFildStyle);
        GUI.color = Color.white;

        //BUTTONS
        if(!simpleX) buttonStyle.normal.background = DB.DBTexture[31];
        else buttonStyle.normal.background = DB.DBTexture[34];
        if (GUI.Button(new Rect(121, 75+ lastRectPosY, 23, 18), "", buttonStyle) && FLOORGAMEOBJECTS.Count > 0)
        {
            simpleX = !simpleX;
        }

        if (!simpleY) buttonStyle.normal.background = DB.DBTexture[32];
        else buttonStyle.normal.background = DB.DBTexture[35];
        if (GUI.Button(new Rect(154, 75+ lastRectPosY, 23, 18), "", buttonStyle) && FLOORGAMEOBJECTS.Count > 0)
        {
            simpleY = !simpleY;
        }

        if (!simpleZ) buttonStyle.normal.background = DB.DBTexture[33];
        else buttonStyle.normal.background = DB.DBTexture[36];
        if (GUI.Button(new Rect(187, 75+ lastRectPosY, 23, 18), "", buttonStyle) && FLOORGAMEOBJECTS.Count > 0)
        {
            simpleZ = !simpleZ;
        }

        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

        if(optionLinear && enable && FLOORGAMEOBJECTS.Count > 0)
        {
            if (simpleX) x = simpleValue; else x = 0;
            if (simpleY) y = simpleValue; else y = 0;
            if (simpleZ) z = simpleValue; else z = 0;

            distance = new Vector3(x, y, z);
            // 게임오브젝트간 거리 정렬
            MapDistance(distance);
            SceneView.RepaintAll();
        }
    }

    //--[ OPTION LINEAR DETAIL ]--
    void OptionLineDetail()
    {
        lastRectPosY = 28;

        fontFildStyle = new GUIStyle(GUI.skin.textField);
        FontSetting(fontFildStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);

        

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(10, 71 + lastRectPosY, 3, 21), "MIN", optionStyle);
        GUI.Label(new Rect(54, 71 + lastRectPosY, 3, 21), "MAX", optionStyle);
        GUI.Label(new Rect(111, 71 + lastRectPosY, 3, 21), "X", optionStyle);
        GUI.Label(new Rect(149, 71 + lastRectPosY, 3, 21), "Y", optionStyle);
        GUI.Label(new Rect(187, 71 + lastRectPosY, 3, 21), "Z", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(40, 71 + lastRectPosY, 2, 21), "", boundarylineStyle);

        //simpleX, simpleY, simpleZ;
        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        detailX = GUI.Toggle(new Rect(93, 73 + lastRectPosY, 40, 25), detailX, ""); // SELECT NAME

        detailY = GUI.Toggle(new Rect(131, 73 + lastRectPosY, 40, 25), detailY, ""); // SELECT TAG
        if (detailY)
        {
        }
        detailZ = GUI.Toggle(new Rect(169, 73 + lastRectPosY, 40, 25), detailZ, ""); // selection

        GUI.color = Color.white;

        LineSetting(boundarylineStyle, 22, Screen.width, 1);
        //    float detailXMin, detailXMax, detailYMin, detailYMax, detailZMin, detailZMax;

        if (detailX)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(25));

            posy = GUILayoutUtility.GetLastRect().position.y + 5;

            fontFildStyle.alignment = TextAnchor.MiddleCenter;

            GUI.color = Corr(200, 200, 200, 255);
            detailXMin = EditorGUI.FloatField(new Rect(4, posy, 35, 16), detailXMin, fontFildStyle);
            detailXMax = EditorGUI.FloatField(new Rect(47-4, posy, 35, 16), detailXMax, fontFildStyle);
            GUI.color = Color.white;

            //cirAngler = GUI.HorizontalSlider(new Rect(95, posy, Screen.width - 175, 16), cirAngler, cirAngValMin, cirAngValMax);
            //cirAngler = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16), Mathf.Floor((cirAngler * 100.0f) + 0.5f) / 100.0f); //소수점 정리
            //if (cirAngler > cirAngValMax) cirAngler = cirAngValMax;

            detailXV = GUI.HorizontalSlider(new Rect(95, posy, Screen.width-175, 16),detailXV,detailXMin,detailXMax);
            detailXV = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16), Mathf.Floor((detailXV * 100.0f) + 0.5f) / 100.0f);
            if (detailXV < detailXMin) detailXV = detailXMin;
            if (detailXV > detailXMax) detailXV = detailXMax;


            optionStyle.font = DB.DBFonts[0];
            optionStyle.normal.textColor = Corr(150,150,150,255);
            GUI.Label(new Rect(Screen.width-17, posy-4, 25, 25), "X", optionStyle);

        }
        if (detailY)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(25));

            posy = GUILayoutUtility.GetLastRect().position.y + 5;

            fontFildStyle.alignment = TextAnchor.MiddleCenter;

            GUI.color = Corr(200, 200, 200, 255);
            detailYMin = EditorGUI.FloatField(new Rect(4, posy, 35, 16), detailYMin, fontFildStyle);
            detailYMax = EditorGUI.FloatField(new Rect(47 - 4, posy, 35, 16), detailYMax, fontFildStyle);
            GUI.color = Color.white;

           // detailYV = EditorGUI.Slider(new Rect(95, posy, Screen.width - 120, 16), detailYV, detailYMin, detailYMax);

            detailYV = GUI.HorizontalSlider(new Rect(95, posy, Screen.width - 175, 16), detailYV, detailYMin, detailYMax);
            detailYV = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16), Mathf.Floor((detailYV * 100.0f) + 0.5f) / 100.0f);
            if (detailYV < detailYMin) detailYV = detailYMin;
            if (detailYV > detailYMax) detailYV = detailYMax;


            optionStyle.font = DB.DBFonts[0];
            optionStyle.normal.textColor = Corr(150, 150, 150, 255);
            GUI.Label(new Rect(Screen.width - 17, posy - 4, 25, 25), "Y", optionStyle);
        }

        if (detailZ)
        {

            GUILayout.Box("", nullStylle, GUILayout.Height(25));

            posy = GUILayoutUtility.GetLastRect().position.y + 5;

            fontFildStyle.alignment = TextAnchor.MiddleCenter;

            GUI.color = Corr(200, 200, 200, 255);
            detailZMin = EditorGUI.FloatField(new Rect(4, posy, 35, 16), detailZMin, fontFildStyle);
            detailZMax = EditorGUI.FloatField(new Rect(47 - 4, posy, 35, 16), detailZMax, fontFildStyle);
            GUI.color = Color.white;

            //detailZV = EditorGUI.Slider(new Rect(95, posy, Screen.width - 120, 16), detailZV, detailZMin, detailZMax);

            detailZV = GUI.HorizontalSlider(new Rect(95, posy, Screen.width - 175, 16), detailZV, detailZMin, detailZMax);
            detailZV = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16), Mathf.Floor((detailZV * 100.0f) + 0.5f) / 100.0f);
            if (detailZV < detailZMin) detailZV = detailZMin;
            if (detailZV > detailZMax) detailZV = detailZMax;

            optionStyle.font = DB.DBFonts[0];
            optionStyle.normal.textColor = Corr(150, 150, 150, 255);
            GUI.Label(new Rect(Screen.width - 17, posy - 4, 25, 25), "Z", optionStyle);
        }


        if (enable && FLOORGAMEOBJECTS.Count > 0)
        {
            if (detailX) x = detailXV; else x = 0;
            if (detailY) y = detailYV; else y = 0;
            if (detailZ) z = detailZV; else z = 0;

            distance = new Vector3(x, y, z);
            // 게임오브젝트간 거리 정렬
            MapDistance(distance);
            SceneView.RepaintAll();
        }

    }


    //--[ OPTION CIRCLER ]--
    void OptionCircler()
    {

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("OPTIONS", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 25); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(25, 72, 40, 25), "SIMPLE", optionStyle);
        GUI.Label(new Rect(93 + optionsPosx, 72, 40, 25), "DETAIL", optionStyle);

        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(61 + optionsPosx, 71, 2, 25), "", boundarylineStyle);


        // toggle
        GUI.color = Corr(120, 120, 120, 255);
        optCirclerSimple = GUI.Toggle(new Rect(7, 77, 50, 25), optCirclerSimple, "");
        if (optCirclerSimple)
        {
            optionCircleDetaile = false;
        }
        optionCircleDetaile = GUI.Toggle(new Rect(75 + optionsPosx, 76, 50, 25), optionCircleDetaile, "");
        if (optionCircleDetaile)
        {
            optCirclerSimple = false;
        }
        GUI.color = Color.white;

        if (optCirclerSimple || optionCircleDetaile) LineSetting(optionStyle, 43, Screen.width, 3);

        if (optCirclerSimple) OptionCirclerSimple();
        if (optionCircleDetaile) OptionCirclerDetail();

    }

    //--[ OPTION CIRCLER SIMPLE ]--
    void OptionCirclerSimple()
    {
        lastRectPosY = 28;

        LineSetting(optionStyle, 19, Screen.width, 25); // 한줄 라인

        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(10, 73 + lastRectPosY, 3, 21), "DISTANCE", optionStyle);



        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(109, 71 + lastRectPosY, 2, 25), "", boundarylineStyle);


        //VALUE
        fontFildStyle = new GUIStyle(GUI.skin.textField);
        FontSetting(fontFildStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);
        GUI.color = Corr(180, 180, 180, 255);
        radius = EditorGUI.FloatField(new Rect(72, 76 + lastRectPosY, 28, 16), radius, fontFildStyle);
        GUI.color = Color.white;

        //BUTTONS
        if (!cirSimpleX) buttonStyle.normal.background = DB.DBTexture[31];
        else buttonStyle.normal.background = DB.DBTexture[34];
        if (GUI.Button(new Rect(121, 75 + lastRectPosY, 23, 18), "", buttonStyle) && FLOORGAMEOBJECTS.Count > 0)
        {
            cirSimpleX = !cirSimpleX;// 이미지 체인지
            cirSimpleY = false;
            cirSimpleZ = false;
        }

        if (!cirSimpleY) buttonStyle.normal.background = DB.DBTexture[32];
        else buttonStyle.normal.background = DB.DBTexture[35];
        if (GUI.Button(new Rect(154, 75 + lastRectPosY, 23, 18), "", buttonStyle) && FLOORGAMEOBJECTS.Count > 0)
        {
            cirSimpleY = !cirSimpleY;// 이미지 체인지
            cirSimpleX = false;
            cirSimpleZ = false;
        }

        if (!cirSimpleZ) buttonStyle.normal.background = DB.DBTexture[33];
        else buttonStyle.normal.background = DB.DBTexture[36];
        if (GUI.Button(new Rect(187, 75 + lastRectPosY, 23, 18), "", buttonStyle) && FLOORGAMEOBJECTS.Count > 0)
        {
            cirSimpleZ = !cirSimpleZ;// 이미지 체인지
            cirSimpleX = false;
            cirSimpleY = false;
        }

        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

        if (optionCircler && enable && FLOORGAMEOBJECTS.Count > 0)
        {
            
            if (cirSimpleX || cirSimpleY || cirSimpleZ)  CircleSimpleCal();
            SceneView.RepaintAll();
        }
        if (!enable)
        {
            cirSimpleX = false;
            cirSimpleY = false;
            cirSimpleZ = false;
        }
    }


    void CircleSimpleCal()
    {
        try
        {
            for (int i = 0; i < FLOORGAMEOBJECTS.Count; i++)
            {
                dis = (360 / FLOORGAMEOBJECTS.Count) * i; // 간격 * i
                rot = Mathf.Sin((angle + dis) * Mathf.Deg2Rad) * radius; // 높낮이(회전)
                scl = Mathf.Cos((angle + dis) * Mathf.Deg2Rad) * radius; //넓이(크기) 

                if (cirSimpleX) newPos = new Vector3(scl, 0, rot);
                if (cirSimpleY) newPos = new Vector3(0, scl, rot);
                if (cirSimpleZ) newPos = new Vector3(rot, scl, 0);
                FLOORGAMEOBJECTS[i].transform.position = newPos;
            }
        }
        catch
        {
            //사용자가 임의로 지웠을때
        }
    }


    //--[ OPTION CIRCLER DETAIL ]--
    void OptionCirclerDetail()
    {
        lastRectPosY = 28;

        fontFildStyle = new GUIStyle(GUI.skin.textField);
        FontSetting(fontFildStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);



        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(10, 71 + lastRectPosY, 3, 21), "MIN", optionStyle);
        GUI.Label(new Rect(54, 71 + lastRectPosY, 3, 21), "MAX", optionStyle);
        GUI.Label(new Rect(111, 71 + lastRectPosY, 3, 21), "X", optionStyle);
        GUI.Label(new Rect(149, 71 + lastRectPosY, 3, 21), "Y", optionStyle);
        GUI.Label(new Rect(187, 71 + lastRectPosY, 3, 21), "Z", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(40, 71 + lastRectPosY, 2, 21), "", boundarylineStyle);

        //simpleX, simpleY, simpleZ;
        //--[ TOGGLE ]---
        GUI.color = Corr(170, 170, 170, 255);
        cirDetailX = GUI.Toggle(new Rect(93, 73 + lastRectPosY, 40, 25), cirDetailX, ""); // X 
        if (cirDetailX)
        {
            cirDetailY = false;
            cirDetailZ = false;
        }

        cirDetailY = GUI.Toggle(new Rect(131, 73 + lastRectPosY, 40, 25), cirDetailY, ""); // Y
        if (cirDetailY)
        {
            cirDetailX = false;
            cirDetailZ = false;
        }

        cirDetailZ = GUI.Toggle(new Rect(169, 73 + lastRectPosY, 40, 25), cirDetailZ, ""); // Z
        if (cirDetailZ)
        {
            cirDetailY = false;
            cirDetailX = false;
        }

        GUI.color = Color.white;

        LineSetting(boundarylineStyle, 22, Screen.width, 1);

        if (cirDetailX || cirDetailY || cirDetailZ)
        {

            fontFildStyle.alignment = TextAnchor.MiddleCenter;

            // RADIUS SLIDER
            GUILayout.Box("", nullStylle, GUILayout.Height(25));
            posy = GUILayoutUtility.GetLastRect().position.y + 5;


            GUI.color = Corr(200, 200, 200, 255);
            cirValMin = EditorGUI.FloatField(new Rect(4, posy, 35, 16), cirValMin, fontFildStyle);
            cirValMax = EditorGUI.FloatField(new Rect(47 - 4, posy, 35, 16), cirValMax, fontFildStyle);
            GUI.color = Color.white;

            cirRadius = GUI.HorizontalSlider(new Rect(95, posy, Screen.width - 175, 16), cirRadius, cirValMin, cirValMax);
            //cirRadius = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16),   Mathf.Round(cirRadius/.01f)*.01f );
            cirRadius = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16), Mathf.Floor((cirRadius *100.0f) + 0.5f) /100.0f);
            if (cirRadius > cirValMax) cirRadius = cirValMax;

            //cirRadius =EditorGUI.Slider(new Rect(95, posy, Screen.width - 175, 16), cirRadius, cirValMin, cirValMax);

            optionStyle.font = DB.DBFonts[0];
            optionStyle.normal.textColor = Corr(150, 150, 150, 255);
            GUI.Label(new Rect(Screen.width - 17, posy - 4, 25, 25), "R", optionStyle);

            if (cirValMin < 0) cirValMin = 0; // 최소값 제한

            // ANGLE SLIDER
            GUILayout.Box("", nullStylle, GUILayout.Height(25));
            posy = GUILayoutUtility.GetLastRect().position.y ;


            GUI.color = Corr(200, 200, 200, 255);
            cirAngValMin = EditorGUI.FloatField(new Rect(4, posy, 35, 16), cirAngValMin, fontFildStyle);
            cirAngValMax = EditorGUI.FloatField(new Rect(47 - 4, posy, 35, 16), cirAngValMax, fontFildStyle);
            GUI.color = Color.white;

            cirAngler = GUI.HorizontalSlider(new Rect(95, posy, Screen.width - 175, 16), cirAngler, cirAngValMin, cirAngValMax);
            cirAngler = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16), Mathf.Floor((cirAngler * 100.0f) + 0.5f) / 100.0f); //소수점 정리
            if (cirAngler > cirAngValMax) cirAngler = cirAngValMax;


            optionStyle.font = DB.DBFonts[0];
            optionStyle.normal.textColor = Corr(150, 150, 150, 255);
            GUI.Label(new Rect(Screen.width - 17, posy - 4, 25, 25), "A", optionStyle);

            if (cirAngValMin < 0) cirAngValMin = 0; // 최소값 제한

            ContentsBLine(); // 경계선

            
            GUILayout.Box("", nullStylle, GUILayout.Height(8)); // 여백

            // OBJECT ROTATION
            ObjRotation(ref objRotX, "X" ,"ROTATION"); 
            ObjRotation(ref objRotY, "Y","");
            ObjRotation(ref objRotZ, "Z","");
            
            foreach (var t in FLOORGAMEOBJECTS)
            {
                t.transform.rotation = Quaternion.Euler(new Vector3(objRotX, objRotY, objRotZ));
            }


            GUILayout.Box("", nullStylle, GUILayout.Height(10));

        }

        if (optionCircler && enable && FLOORGAMEOBJECTS.Count > 0)
        {

            if (cirDetailX || cirDetailY || cirDetailZ) CircleDrtailCal();
            SceneView.RepaintAll();
        }
      

    }

    void ObjRotation(ref float rot, string dir, string title)
    {
        GUILayout.Box("", nullStylle, GUILayout.Height(22));
        posy = GUILayoutUtility.GetLastRect().position.y;

        GUI.color = Corr(130, 130, 130, 100);
        EditorGUI.LabelField(new Rect(4, posy, 70, 16), title);
        GUI.color = Color.white;

        rot = GUI.HorizontalSlider(new Rect(95, posy, Screen.width - 175, 16), rot, 0, 360);
        rot = EditorGUI.FloatField(new Rect(Screen.width - 75, posy, 50, 16), Mathf.Floor((rot * 100.0f) + 0.5f) / 100.0f); //소수점 정리
        

        optionStyle.font = DB.DBFonts[0];
        optionStyle.normal.textColor = Corr(150, 150, 150, 255);
        GUI.Label(new Rect(Screen.width - 17, posy - 4, 25, 25), dir, optionStyle);

        
    }

    void CircleDrtailCal()
    {
        try
        {
            for (int i = 0; i < FLOORGAMEOBJECTS.Count; i++)
            {
                dis = (360 / FLOORGAMEOBJECTS.Count) * i; // 간격 * i
                rot = Mathf.Sin((cirAngler + dis) * Mathf.Deg2Rad) * cirRadius; // 높낮이(회전)
                scl = Mathf.Cos((cirAngler + dis) * Mathf.Deg2Rad) * cirRadius; //넓이(크기) 

                if (cirDetailX) newPos = new Vector3(scl, 0, rot);
                if (cirDetailY) newPos = new Vector3(0, scl, rot);
                if (cirDetailZ) newPos = new Vector3(rot, scl, 0);
                FLOORGAMEOBJECTS[i].transform.position = newPos;
            }
        }
        catch
        {
            // 사용자가 임의로 오브젝트 지웠을때
        }
    }



    void ButtonTextChabge()
    {
        if (!enable) buttonText = "LOAD & ENABLE";
        else buttonText = "COMPLETE & DISABLE";
    }

    // 리셋
    void Reset()
    {
        simpleX = false;
        simpleY = false;
        simpleZ = false;

        detailXV = 0;
        detailYV = 0;
        detailZV = 0;

        cirSimpleX = false;
        cirSimpleY = false;
        cirSimpleZ = false;

        
    }

    void TryObj()
    {
        if (!enable) return;
        if(FLOORGAMEOBJECTS.Count != selectedObjNum)
        {
            FLOORGAMEOBJECTS.Clear();
            selectedObjNum = 0;
        }
    }

    void OnGUI()
    {
        TryObj();

        ButtonTextChabge();
        //FOLDER = (Object)EditorGUILayout.ObjectField("ObjectField", FOLDER, typeof(Object), false); //booltype : scene의 오브젝트를 드래그 해서 넣을 수 있는가?
        //floor = (GameObject)EditorGUILayout.ObjectField("floor", floor, typeof(GameObject), false);
        //distance = EditorGUILayout.Vector3Field("Distance", distance);
        //num = EditorGUILayout.IntField("num", num);
        TopLine();
        Options();
        // 밝은 경계선
        if (optionLinear || optionCircler) LineSetting(boundarylineStyle, 29, Screen.width, 1); 
        else LineSetting(boundarylineStyle, 22, Screen.width, 1);

        if (optionLinear) OptionLinear();
        if (optionCircler) OptionCircler();

        //if (optionSimple) OptionSimple();
        //if (optionDetaile) OptionDetail();


        // 시작 메세지
        if (!optionLinear && !optionCircler)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(7));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", nullStylle, GUILayout.Width(5));

            GUI.color = Corr(153, 153, 153, 255);
            EditorGUILayout.HelpBox("ALIGNER IS... SELECT OPTION", MessageType.Info);
            GUI.color = Color.white;

            GUILayout.Box("", nullStylle, GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();

            GUILayout.Box("", nullStylle, GUILayout.Height(17));
        }

        // 컨덴츠 여백
        if (optionSimple || optionDetaile)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(10));
        }

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

        //  GUI.Box(new Rect(0,145,Screen.width,Screen.height),"", BgStyle);

        GUI.color = enable ? Corr(145, 39, 74, 180) : Corr(180, 180, 180, 255);
        if (GUILayout.Button(buttonText, buttonStyle, GUILayout.MinHeight(20)) && (optionLinear || optionCircler) )
        {

            
            if (!enable && Selection.gameObjects.Length > 0)
            {
                ObjectsInput();
                selectedObjNum = FLOORGAMEOBJECTS.Count;
                enable = true;
            }
            else if(enable)
            {
                // 초기화
                Reset();
                FLOORGAMEOBJECTS.Clear();
                selectedObjNum = 0;
                enable = false;

            }
            //enable = !enable;
        }

        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();


        Footer();  // 하단 상태바





    }


    //하단 상태바
    void Footer()
    {
        if(mouseOverWindow)Repaint();
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

        if (Selection.objects.Length > 0) SelectObjectCount = Selection.gameObjects.Length;
        else SelectObjectCount = 0;

        GUI.Box(new Rect(125, Screen.height - 54, 32, 32), SelectObjectCount.ToString(), footerStyle);

    }


    void ObjectsInput()
    {
        // 하이어 라키 순서대로 정렬 한다.
        FLOORGAMEOBJECTS = Selection.gameObjects.Where(g => !AssetDatabase.Contains(g)).OrderBy(g => g.transform.GetSiblingIndex()).ToList();
        // 첫번째 선택된 오브젝를 기준점으로 잡는다.
        // standardPoint = Selection.activeGameObject.transform.position; // FLOORGAMEOBJECTS[0].transform.position; // 선택된 오브젝트중 0번째 오브젝트( 첫번째로 수정)
         standardPoint = FLOORGAMEOBJECTS[0].transform.position; // 선택된 오브젝트중 0번째 오브젝트( 첫번째로 수정)
    }

    void MapDistance(Vector3 pos)
    {
        try
        {

            for (int i = 0; i < FLOORGAMEOBJECTS.Count; i++)
            {
                FLOORGAMEOBJECTS[i].transform.position = standardPoint + (pos * i);
            }
        }
        catch
        {
            // 사용자가 임의로 오브젝트 지웠을때
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
        Icon_Menu.btnBools[9] = false;
    }
}
