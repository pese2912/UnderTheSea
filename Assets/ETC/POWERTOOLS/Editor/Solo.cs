using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Enum = System.Enum;

public class Solo : EditorWindow
{
    List<GameObject> DESELECTOBJECT = new List<GameObject>();

    PTData DB;

    enum SOLOSTATE 
    {
        NONE = 0,
        SOLO,
        UNSOLO
    }
    SOLOSTATE soloState = SOLOSTATE.SOLO;

    string buttonTitle, tagName;
    bool titleSwitch = false;
    bool unhideCamera;
    bool unhideLight;
    bool selectTag;
    bool parentView;

    bool enable;

    bool optionSelect;
    bool optionTag;

    float togglePosY, optionTextPosY;
    float tagLabelposy;

    float topOptionPosX;

    float lightPosX , parentPosX;

    int SelectObjectCount;

    GUIStyle BgStyle = new GUIStyle();
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle footerStyle = new GUIStyle();
    GUIStyle bgStyle = new GUIStyle();


    void OnEnable()
    {
        LoadData();  //데이터 로드
        topOptionPosX = 48;

        lightPosX = 15;
        parentPosX = 38;
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
        GUI.Label(new Rect(9, 0, 80, 25), "SOLO", fontNomalStyle);
    }

    //--[  SELECT OBJECT | TAG ]-- 토글 옵션들 
    void OptionLine()
    {
        bgStyle.normal.background = DB.DBTexture[16];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));


        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(193, 193, 193, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        // label
        GUI.Label(new Rect(25 + 3, 24, 40, 25), "SELECT OBJ", fontStyle);
        GUI.Label(new Rect(93 + 3+ topOptionPosX, 24, 40, 25), "SELECT TAG", fontStyle);

        // 경계선
        boundarylineStyle.normal.background = DB.DBTexture[20];
        GUI.Box(new Rect(61+ topOptionPosX, 24, 3, 25), "", boundarylineStyle);

        // toggle
        GUI.color = Corr(120, 120, 120, 255);
        optionSelect = GUI.Toggle(new Rect(7, 28, 50, 25), optionSelect, "");
        if (optionSelect)
        {
        }
        optionTag = GUI.Toggle(new Rect(72 + 3+ topOptionPosX, 28, 50, 25), optionTag, "");
        if (optionTag)
        {
        }

        GUI.color = Color.white;
    }
    //--[ OPTIONS ]--
    void Options()
    {
        togglePosY = 74;
        optionTextPosY = 71;


        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("UNHIDE OPTIONS", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인



        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(27, optionTextPosY, 3, 21), "CAMERA", optionStyle);
        GUI.Label(new Rect(122- lightPosX, optionTextPosY, 3, 21), "LIGHT", optionStyle);
        GUI.Label(new Rect(218- parentPosX, optionTextPosY, 3, 21), "PARENT", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(92- lightPosX, optionTextPosY, 2, 21), "", boundarylineStyle);
        GUI.Box(new Rect(188- parentPosX, optionTextPosY, 2, 21), "", boundarylineStyle);


        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        unhideCamera = GUI.Toggle(new Rect(7, togglePosY, 40, 25), unhideCamera, ""); // SELECT POSITION
        if (unhideCamera)
        {

        }
        unhideLight = GUI.Toggle(new Rect(102- lightPosX, togglePosY, 40, 25), unhideLight, ""); // SELECT TAG
        if (unhideLight)
        {

        }
        parentView = GUI.Toggle(new Rect(198- parentPosX, togglePosY, 40, 25), parentView, ""); // selection
        if (parentView)
        {
        }
        GUI.color = Color.white;

    }

    void SelectTag()
    {
         tagLabelposy = GUILayoutUtility.GetLastRect().position.y;

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(193, 193, 193, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        FontSetting(fontStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);

        //GUILayout.Label("SELECT TAG", fontStyle , GUILayout.Width(70));
        GUI.Label(new Rect(9, tagLabelposy + 4, 100, 25), "TAG", fontStyle);
        GUILayout.Box("", nullStylle, GUILayout.Width(35));
        // 태그선택
        GUI.color = Corr(180, 180, 180, 255);
        tagName = EditorGUILayout.TagField("", tagName, GUILayout.Width(Screen.width - 50));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    void ButtonText()
    {
        if(!enable) buttonTitle = "SOLO";
        else buttonTitle = "UNSOLO";
    }

    void OnGUI()
    {
        //버튼 텍스트 변경
        ButtonText();


        TopLine();
        OptionLine();

        if (optionSelect || optionTag)  LineSetting(boundarylineStyle, 29, Screen.width, 1);

        if (optionSelect&&!optionTag)
        {
            Options();
            LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
        }
        
        
        if (optionTag)
        {
            Options();
            LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
            SelectTag();
        }

        if (selectTag)
        {
            tagName = EditorGUILayout.TagField("select tag", tagName);
        }


        if (!enable && !optionSelect && !optionTag)
        {
            LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

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
        if (GUILayout.Button(buttonTitle) && (optionSelect || optionTag) )
        {
            

                enable = !enable;

                if (!titleSwitch)
                {
                    // 1. 카메라 포함,
                    if (unhideCamera && !unhideLight && !selectTag) DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !Selection.gameObjects.Contains(g) && g.gameObject.active == true && g.GetComponent<Camera>() == null).ToList();

                    //  2.라이트 포함, 
                    else if (!unhideCamera && unhideLight && !selectTag) DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !Selection.gameObjects.Contains(g) && g.gameObject.active == true && g.GetComponent<Light>() == null).ToList();

                    // 3. 카메라와 라이트 포함, 
                    else if (unhideCamera && unhideLight && !selectTag) DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !Selection.gameObjects.Contains(g) && g.gameObject.active == true && g.GetComponent<Light>() == null && g.GetComponent<Camera>() == null).ToList();

                    // 4. 태그만 포함
                    else if (!unhideCamera && !unhideLight && selectTag) DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !g.CompareTag(tagName)).ToList();

                    // 5. 카메라와 태그 포함
                    else if (unhideCamera && !unhideLight && selectTag) DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !g.CompareTag(tagName) && g.GetComponent<Camera>() == null).ToList();

                    // 6. 라이트와 태그 포함
                    else if (!unhideCamera && unhideLight && selectTag) DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !g.CompareTag(tagName) && g.GetComponent<Light>() == null).ToList();

                    // 6. 카메라, 라이트, 태그 포함
                    else if (unhideCamera && unhideLight && selectTag) DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !g.CompareTag(tagName) && g.GetComponent<Camera>() == null && g.GetComponent<Light>() == null).ToList();

                    //7.선택한 오브젝트만
                    else DESELECTOBJECT = DESELECTOBJECT = GameObject.FindObjectsOfType<GameObject>().Where(g => !Selection.gameObjects.Contains(g) && g.gameObject.active == true).ToList();

                    // 1. 부모 오브젝트 제외
                    // 2. 부모 오브젝트중 매쉬 렌더러 desable
                    DESELECTOBJECT.Where(g => g.transform.childCount == 0).ToList().ForEach(s => s.SetActive(false));
                    if (!parentView) DESELECTOBJECT.Where(g => g.transform.childCount >= 1 && g.GetComponent<MeshRenderer>() != null).ToList().ForEach(s => s.gameObject.GetComponent<MeshRenderer>().enabled = false);
                }

                else
                {
                    //for (int i = 0; i < DESELECTOBJECT.Count; i++)
                    //{
                    //    if (DESELECTOBJECT[i] == null) continue;
                    //    DESELECTOBJECT[i].SetActive(true);
                    //}
                    DESELECTOBJECT.ForEach(s => s.SetActive(true));
                    if (!parentView) DESELECTOBJECT.Where(g => g.transform.childCount >= 1 && g.GetComponent<MeshRenderer>() != null).ToList().ForEach(s => s.GetComponent<MeshRenderer>().enabled = true);
                }
                titleSwitch = !titleSwitch;

           
        }
       
        GUI.color = Color.white;
        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();
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


    void SoloBtnSwitch()
    {
        switch (titleSwitch)
        {
            case false:
                buttonTitle = "SOLO";
                break;
            case true:
                buttonTitle = "UNSOLO";
                break;
        }
    }

    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[6] = false;
    }


}
