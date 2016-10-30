using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Renamer : EditorWindow
{

    PTData DB;


    bool toggleUpLower, toggleNumber, toggleReplace, toggleAddDel;       // TOGGLE OPTION
    bool optionFLU, optionFLL, optionALU, optionALL;                     // UPPER & LOWER
    bool optionNumberNF, optionNumberNB, optionNumberTF, optionNumberTB; // NUMBERING
    bool optionAddFirst, optionAddBack;                                  // ADD & DELETE
    bool optionUndo;

    string findText = string.Empty;
    string changText = string.Empty;
    string addText;
    string manualText;
    string firstLetter;

    string[] STRING = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    string[] ROMA = { "","Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "Ⅹ", "ⅩⅠ", "ⅩⅡ", "ⅩⅢ", "ⅩⅣ", "ⅩⅤ", "ⅩⅥ", "ⅩⅦ", "ⅩⅧ", "ⅩⅨ", "ⅩⅩ", "ⅩⅩⅠ", "ⅩⅩⅡ", "ⅩⅩⅢ", "ⅩⅩⅣ", "ⅩⅩⅤ", "ⅩⅩⅥ", "ⅩⅩⅦ", "ⅩⅩⅧ", "ⅩⅩⅨ", "ⅩⅩⅩ", "ⅩⅩⅩⅠ", "ⅩⅩⅩⅡ", "ⅩⅩⅩⅢ", "ⅩⅩⅣ", "ⅩⅩⅤ", "ⅩⅩⅥ", "ⅩⅩⅩⅦ", "ⅩⅩⅩⅧ", "ⅩⅩⅩⅨ", "XL", "XLI", "XLII", "XLIII", "XLIV", "XLV", "XLVI", "XLVII", "XLVIII", "XLIX", "L" };
    

    string addLetter = string.Empty;
    string selectChr;
    string changeName;

    string letter;

    int number;
    int count;
    int romaCount;
    int textLength;


    int pNum = 2, nNum = 1, mnumber = 1;
    int numLetter;

    float topOptionPosY;
    float topOptionPosX1;
    float topOptionPosX2;
    float topOptionPosX3;
    float lastRectPosY;
    float listBtnPosX;
    float posXgap;


    enum OPTIONSTATE
    {
        NONE = 0,
        FIRSTUPPER,
        FIRSTLOWER,
        ALLUPPER,
        ALLLOWER,
        NUMBERINGF,
        NUMBERINGL,
        STRINGF,
        STRINGL,
        REPLACE,
        CHANGEALL,
        ADDF,
        ADDL,
        DELF,
        DELL
    }
    OPTIONSTATE optionState = OPTIONSTATE.NONE;


    static List<Object> LIVESELECTOBJECT = new List<Object>();
    List<Object> SELECTOBJECT = new List<Object>();
    List<GameObject> SORTOBJECT = new List<GameObject>();


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
    GUIStyle darkListBGLogoStyle = new GUIStyle();
    GUIStyle listBgStyle = new GUIStyle();
    GUIStyle darkListBGStyle = new GUIStyle();
    GUIStyle listBgLineStyle = new GUIStyle();
    GUIStyle listBtnStyle = new GUIStyle();
    GUIStyle textFildStyleNumber = new GUIStyle();
    GUIStyle textFildStyleAddDel = new GUIStyle();
    GUIStyle textFildStyleReplace = new GUIStyle();


    Vector2 scrollView;


    void OnEnable()
    {
        LoadData();
        topOptionPosY = 52;
        topOptionPosX1 = 70;
        topOptionPosX2 = 70;
        topOptionPosX3 = 275;
        posXgap = 17;

        listBtnStyle.normal.background = DB.DBTexture[6];
        darkListBGStyle.normal.background = DB.DBTexture[8];


        addText = "-";
    }
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
        GUI.Label(new Rect(9, 0, 80, 25), "RENAMER", fontNomalStyle);
    }

    //--[ TOGGLE OPTION ]-- 토글 옵션들 
    void TopOptionLine()
    {
        bgStyle.normal.background = DB.DBTexture[16];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        bgStyle.normal.background = DB.DBTexture[11];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));
        bgStyle.normal.background = DB.DBTexture[12];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));


        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(193, 193, 193, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        // label
        GUI.Label(new Rect(28 , 24, 40, 25), "UPPER & LOWER", fontStyle);
        GUI.Label(new Rect(96 + topOptionPosX1, 24, 40, 25), "REPLACE", fontStyle);
        GUI.Label(new Rect(28 , topOptionPosY, 40, 25), "ADD & REMOVE", fontStyle);
        GUI.Label(new Rect(96 + topOptionPosX2, topOptionPosY, 40, 25), "NUMBERING", fontStyle);


        // 경계선
        boundarylineStyle.normal.background = DB.DBTexture[20];
        GUI.Box(new Rect(61 + topOptionPosX1, 24, 3, 25), "", boundarylineStyle);
        boundarylineStyle.normal.background = DB.DBTexture[41];
        GUI.Box(new Rect(61 + topOptionPosX2, topOptionPosY, 3, 25), "", boundarylineStyle);


        // TOGGLE
        GUI.color = Corr(120, 120, 120, 255);
        toggleUpLower = GUI.Toggle(new Rect(7, 28, 50, 25), toggleUpLower, "");
        if (toggleUpLower)
        {
            //toggleReplace = false; toggleAddDel = false; toggleNumber = false;
        }

        toggleReplace = GUI.Toggle(new Rect(75 + topOptionPosX1, 28, 50, 25), toggleReplace, "");
        if (toggleReplace)
        {
            //toggleUpLower = false;  toggleAddDel = false; toggleNumber = false;
        }

        toggleAddDel = GUI.Toggle(new Rect(7, topOptionPosY+4, 50, 25), toggleAddDel, "");
        if (toggleAddDel)
        {
            //toggleUpLower = false; toggleReplace = false; toggleNumber = false;
        }

        toggleNumber = GUI.Toggle(new Rect(75 + topOptionPosX2, topOptionPosY+4, 50, 25), toggleNumber, "");
        if (toggleNumber)
        {
            //toggleUpLower = false; toggleReplace = false; toggleAddDel = false; 
        }
        GUI.color = Color.white;
    }

    void OptionUPnLOWER()
    {
        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;
        
        EditorGUILayout.BeginHorizontal();
        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일
        GUILayout.Box("OPTION <UPPER & LOWER>", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();
        LineSetting(optionStyle, 38, Screen.width, 1); // 한줄 라인
        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인
        optionStyle.normal.background = null;
        optionStyle.normal.textColor = Corr(120, 120, 120, 255);

        //LAST RECT POS
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10+ posXgap, lastRectPosY+3, 3, 21), "FIRST LETTER TO", optionStyle);

        //-- [ BUTTON ]--
        GUI.color = Corr(150, 120, 120, 255);
        if (GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 5, 50, 17), "UPPER", buttonStyle))
        {
            optionState = OPTIONSTATE.FIRSTUPPER;
            RunBtn();
        }

        GUI.color = Corr(120, 120, 150, 255);
        if (GUI.Button(new Rect(Screen.width - 56, lastRectPosY + 5, 50, 17), "LOWER", buttonStyle))
        {
            optionState = OPTIONSTATE.FIRSTLOWER;
            RunBtn();
        }
        GUI.color = Color.white;

        //경계선
        LineSetting(optionStyle, 11, Screen.width, 3); // 한줄 라인
        //BOX
        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인

        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10+ posXgap, lastRectPosY + 2, 3, 21), "ALL LETTER TO", optionStyle);
     
        //BUTTON 
        GUI.color = Corr(150, 120, 120, 255);
        if(GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 3, 50, 17), "UPPER", buttonStyle))
        {
            optionState = OPTIONSTATE.ALLUPPER;
            RunBtn();
        }

        GUI.color = Corr(120, 120, 150, 255);
        if(GUI.Button(new Rect(Screen.width - 56, lastRectPosY + 3, 50, 17), "LOWER", buttonStyle))
        {
            optionState = OPTIONSTATE.ALLLOWER;
            RunBtn();
        }

        GUI.color = Color.white;
    }

    void TextFiled()
    {
        textFildStyleNumber = new GUIStyle(GUI.skin.textField);
        textFildStyleNumber.font = DB.DBFonts[1];
        textFildStyleNumber.fontSize = 11;
        textFildStyleNumber.alignment = TextAnchor.MiddleCenter;
    }

    void OptionNUMBERING()
    {
        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        EditorGUILayout.BeginHorizontal();
        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일
        GUILayout.Box("OPTION <NUMBERING>", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();
        LineSetting(optionStyle, 38, Screen.width, 1); // 한줄 라인

        LineSetting(optionStyle, 7, Screen.width, 25); // 한줄 라인


        optionStyle.normal.textColor = Corr(120, 120, 120, 255);
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y; //LAST RECT POS
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "ADD TEXT", optionStyle);

        TextFiled();
        GUI.color = Corr(150, 150, 150, 255);
        addText = GUI.TextField(new Rect(Screen.width - 110, lastRectPosY + 5, 50, 17), addText, textFildStyleNumber);
        GUI.color = Color.white;

        LineSetting(optionStyle, 11, Screen.width, 3); // 경계선

        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인
        optionStyle.normal.background = null;
        optionStyle.normal.textColor = Corr(120, 120, 120, 255);

        //LAST RECT POS
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "ADD NUMBERING", optionStyle);



        //-- [ BUTTON ]--
        GUI.color = Corr(150, 120, 120, 255);
        if (GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 5, 50, 17), "FIRST", buttonStyle))
        {
            optionState = OPTIONSTATE.NUMBERINGF;
             RunBtn();
        }

        GUI.color = Corr(120, 120, 150, 255);
        if (GUI.Button(new Rect(Screen.width - 56, lastRectPosY + 5, 50, 17), "LAST", buttonStyle))
        {

            optionState = OPTIONSTATE.NUMBERINGL;
            RunBtn();
        }
        GUI.color = Color.white;

        //경계선
        LineSetting(optionStyle, 11, Screen.width, 3); // 한줄 라인
        //BOX
        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인

        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 2, 3, 21), "ADD STRING", optionStyle);

        //BUTTON 
        GUI.color = Corr(150, 120, 120, 255);
        if (GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 3, 50, 17), "FIRST", buttonStyle))
        {
            optionState = OPTIONSTATE.STRINGF;
            RunBtn();
        }

        GUI.color = Corr(120, 120, 150, 255);
        if (GUI.Button(new Rect(Screen.width - 56, lastRectPosY + 3, 50, 17), "LAST", buttonStyle))
        {
            optionState = OPTIONSTATE.STRINGL;
            RunBtn();
        }

        GUI.color = Color.white;
        
    }


    void OptionADDnDEL()
    {
        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        EditorGUILayout.BeginHorizontal();
        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일
        GUILayout.Box("OPTION <ADD&REMOVE>", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();
        LineSetting(optionStyle, 38, Screen.width, 1); // 한줄 라인

        LineSetting(optionStyle, 7, Screen.width, 25); // 한줄 라인


        optionStyle.normal.textColor = Corr(120, 120, 120, 255);
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y; //LAST RECT POS
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "LETTER", optionStyle);


        TextFiled();
        textFildStyleNumber.alignment = TextAnchor.MiddleLeft;
        GUI.color = Corr(150, 150, 150, 255);
        addLetter = GUI.TextField(new Rect(Screen.width - 110, lastRectPosY + 5, 104, 17), addLetter, textFildStyleNumber);
        GUI.color = Color.white;
        
        

        LineSetting(optionStyle, 11, Screen.width, 3); // 경계선

        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인
        optionStyle.normal.background = null;
        optionStyle.normal.textColor = Corr(120, 120, 120, 255);

        //LAST RECT POS
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
   
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "ADD LETTER", optionStyle);
        


        //-- [ BUTTON ]--
        GUI.color = Corr(150, 120, 120, 255);
        if (GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 5, 50, 17), "FIRST", buttonStyle))
        {
            optionState = OPTIONSTATE.ADDF;
            RunBtn();
        }

        GUI.color = Corr(120, 120, 150, 255);
        if (GUI.Button(new Rect(Screen.width - 56, lastRectPosY + 5, 50, 17), "LAST", buttonStyle))
        {

            optionState = OPTIONSTATE.ADDL;
            RunBtn();
        }
        GUI.color = Color.white;

        //경계선
        LineSetting(optionStyle, 11, Screen.width, 3); // 한줄 라인
        //BOX
        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인

        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 2, 3, 21), "REMOVE CHARACTER", optionStyle);

        //BUTTON 
        GUI.color = Corr(150, 120, 120, 255);
        if (GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 3, 50, 17), "FIRST", buttonStyle))
        {
            optionState = OPTIONSTATE.DELF;
            RunBtn();
        }

        GUI.color = Corr(120, 120, 150, 255);
        if (GUI.Button(new Rect(Screen.width - 56, lastRectPosY + 3, 50, 17), "LAST", buttonStyle))
        {
            optionState = OPTIONSTATE.DELL;
            RunBtn();
        }

        GUI.color = Color.white;


        //경계선
        LineSetting(optionStyle, 11, Screen.width, 3); // 한줄 라인
        //BOX
        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인

        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 2, 3, 21), "REMOVE UNITY NUMBERING", optionStyle);

        GUI.color = Corr(120, 120, 150, 255);
        if (GUI.Button(new Rect(Screen.width - 56, lastRectPosY + 3, 50, 17), "RUN", buttonStyle))
        {
            RemoveUnityNumbering();
        }

        GUI.color = Color.white;
    }


    void OptionREPLACE()
    {
        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        EditorGUILayout.BeginHorizontal();
        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일
        GUILayout.Box("OPTION <REPLACE>", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 38, Screen.width, 1); // 한줄 라인
        LineSetting(optionStyle, 7, Screen.width, 25); // 한줄 라인
        optionStyle.normal.textColor = Corr(120, 120, 120, 255);
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y; //LAST RECT POS
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "FIND LETTER", optionStyle);



        TextFiled();
        textFildStyleNumber.alignment = TextAnchor.MiddleLeft;


        GUI.color = Corr(150, 150, 150, 255);
        findText = GUI.TextField(new Rect(Screen.width - 110, lastRectPosY + 5, 104, 17), findText, textFildStyleNumber);
        GUI.color = Color.white;

        LineSetting(optionStyle, 11, Screen.width, 3); // 경계선

        LineSetting(optionStyle, 7, Screen.width, 25); // 한줄 라인
        optionStyle.normal.textColor = Corr(120, 120, 120, 255);
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y; //LAST RECT POS
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "REPLACE TO", optionStyle);
        GUI.color = Corr(150, 150, 150, 255);
        changText = GUI.TextField(new Rect(Screen.width - 110, lastRectPosY + 5, 104, 17), changText, textFildStyleNumber);
        GUI.color = Color.white;




        LineSetting(optionStyle, 11, Screen.width, 3); // 경계선

        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인
        optionStyle.normal.background = null;
        optionStyle.normal.textColor = Corr(120, 120, 120, 255);
        
        //LAST RECT POS
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "REPLACE", optionStyle);

        //-- [ BUTTON ]--
        GUI.color = Corr(150, 120, 120, 255);
        if (GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 5, 104, 17), "RUN", buttonStyle))
        {
            optionState = OPTIONSTATE.REPLACE;
            RunBtn();
        }
        
        GUI.color = Color.white;

        //경계선
        LineSetting(optionStyle, 11, Screen.width, 3); // 한줄 라인
        //BOX
        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인

        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 2, 3, 21), "CHANGE ALL", optionStyle);

        GUI.color = Corr(120, 120, 150, 255);
        if (GUI.Button(new Rect(Screen.width - 110, lastRectPosY + 3, 104, 17), "RUN", buttonStyle))
        {
            optionState = OPTIONSTATE.CHANGEALL;
            RunBtn();
        }

        GUI.color = Color.white;

        
    }





    void RunBtn()
    {
        GUI.FocusControl("");// 포커스 아웃

        if (SELECTOBJECT.Count <= 0) return;

        number = 1;
        count = 0;
        romaCount = 0;
        foreach (var t in SELECTOBJECT)
        {

            if (AssetDatabase.Contains(t)) { }
            else Undo.RecordObject(t, t.name);

            if (count > 25)
            {
                count = 0;
                romaCount++;
            }
            switch (optionState)
            {
                case OPTIONSTATE.FIRSTUPPER:
                    DataBaseGetTypeToAll(t, FirstLetterChange(t.name, "upper"));
                    break;

                case OPTIONSTATE.FIRSTLOWER:
                    DataBaseGetTypeToAll(t, FirstLetterChange(t.name, "lower"));
                    break;

                case OPTIONSTATE.ALLUPPER:
                    DataBaseGetTypeToAll(t, t.name.ToUpper());
                    break;

                case OPTIONSTATE.ALLLOWER:
                    DataBaseGetTypeToAll(t, t.name.ToLower());
                    break;
                
                 // 숫자 넘버링
                case OPTIONSTATE.NUMBERINGF: 
                    DataBaseGetTypeToAll(t, number + addText + t.name);
                    number++;
                    break;
                case OPTIONSTATE.NUMBERINGL:
                    DataBaseGetTypeToAll(t,  t.name + addText + number);
                    number++;
                    break;
                // 문자 넘버링
                case OPTIONSTATE.STRINGF:
                    DataBaseGetTypeToAll(t, STRING[count] + ROMA[romaCount] + addText + t.name);
                    count++;
                    break;
                case OPTIONSTATE.STRINGL:
                    DataBaseGetTypeToAll(t, t.name + addText + STRING[count] + ROMA[romaCount]);
                    count++;
                    break;
                
                //ADD LETTER
                case OPTIONSTATE.ADDF:
                    DataBaseGetTypeToAll(t, addLetter+t.name);
                    break;
                case OPTIONSTATE.ADDL:
                    DataBaseGetTypeToAll(t,  t.name+ addLetter);
                    break;
                //REMOVE CHARACTER
                case OPTIONSTATE.DELF:
                    DataBaseGetTypeToAll(t, RemoveCharacter(t.name,"first"));
                    break;
                case OPTIONSTATE.DELL:
                    DataBaseGetTypeToAll(t, RemoveCharacter(t.name, "last"));
                    break;
                case OPTIONSTATE.REPLACE:
                    DataBaseGetTypeToAll(t, t.name.Replace(findText, changText));
                    break;
                case OPTIONSTATE.CHANGEALL:
                    DataBaseGetTypeToAll(t, changText);
                    break;

                default:
                    DataBaseGetTypeToAll(t, changeName);
                    break;
            }
        }
        AssetDatabase.Refresh();

    }

    void TitleLine(string text)
    {
        EditorGUILayout.BeginHorizontal();
        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일
        GUILayout.Box(text, optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();
    }



    void TitleTex()
    {
        if (SELECTOBJECT.Count > 0) manualText = "";
        else manualText = "Please select a game object \nand press the Load button.";
    }

    void OnGUI()
    {
        TitleTex();

        TopLine();

        TopOptionLine();

        if (toggleAddDel || toggleNumber || toggleReplace || toggleUpLower)
        {
            LineSetting(boundarylineStyle, 25, Screen.width, 1); // 밝은 경계선
        }
        if (toggleUpLower) OptionUPnLOWER();
        if (toggleNumber) OptionNUMBERING();
        if (toggleAddDel) OptionADDnDEL();
        if (toggleReplace) OptionREPLACE();

        LineSetting(boundarylineStyle, 25, Screen.width, 1); // 밝은 경계선



        TitleLine("OBJECT LIST");
        LineStyle.normal.background = DB.DBTexture[40];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));

        //--[ 시작 설명 ]------------------
        darkListBGLogoStyle.normal.background = null;
        darkListBGLogoStyle.alignment = TextAnchor.MiddleCenter;
        darkListBGLogoStyle.normal.textColor = Corr(120, 120, 120, 255);
        GUI.Box(new Rect(0,GUILayoutUtility.GetLastRect().position.y, Screen.width, Screen.height- GUILayoutUtility.GetLastRect().position.y-50), manualText, darkListBGLogoStyle);
        //-----------------------------

        // 리스트
        GUILayout.BeginHorizontal();
        scrollView = GUILayout.BeginScrollView(scrollView);
        if (SELECTOBJECT != null || SORTOBJECT != null)
        {
            //GUILayout.Box("", nullStylle, GUILayout.Height(3));


            try
            {
                for (int i = 0; i < SELECTOBJECT.Count; i++)
                {
                    if (SELECTOBJECT[i] == null)
                    {
                        SELECTOBJECT.RemoveAt(i);

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
                    listBgStyle.normal.textColor = new Color32(0, 0, 0, 255);



                    listBgStyle.alignment = TextAnchor.MiddleLeft;

                    GUILayout.Box("       " + SELECTOBJECT[i].name, listBgStyle, GUILayout.MaxWidth(Screen.width - 26), GUILayout.Height(25));
                    GUILayout.Box("", listBgLineStyle, GUILayout.MaxWidth(2), GUILayout.Height(25));

                    listBtnPosX = GUILayoutUtility.GetLastRect().position.x;

                    GUILayout.Box("", listBgStyle, GUILayout.MaxWidth(26), GUILayout.Height(25));


                    if (GUI.Button(new Rect(listBtnPosX + 1, (i * 25), 26, 25), "", listBtnStyle))
                    {
                        SELECTOBJECT.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }
            }
            catch
            {
                SELECTOBJECT.Clear();
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        try
        {
            bgStyle.normal.background = null;
            GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(4));
            bgStyle.normal.background = DB.DBTexture[28];
            GUI.Box(new Rect(0, GUILayoutUtility.GetLastRect().position.y, Screen.width, 31), "", bgStyle);

            EditorGUILayout.BeginHorizontal();
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.font = DB.DBFonts[1];
            buttonStyle.fontSize = 11;
      
      
        //--[ LOAD ]--
        GUI.color = Corr(150, 150, 150, 255);
        if (GUILayout.Button("LOAD SELECTED OBJECT", buttonStyle, GUILayout.Height(17)) && Selection.objects.Length > 0)
        {
            if (SELECTOBJECT != null) SELECTOBJECT.Clear();
            SELECTOBJECT = Selection.objects.Where(g => AssetDatabase.Contains(g)).ToList();
            SORTOBJECT = Selection.gameObjects.Where(g => !AssetDatabase.Contains(g)).OrderBy(g => g.transform.GetSiblingIndex()).ToList();

            for (int i = 0; i < SORTOBJECT.Count; i++)
            {
                SELECTOBJECT.Add(SORTOBJECT[i]); // 하이어라키 순서대로 변경
            }
        }
        //--[ REMOVE ]--
        if (GUILayout.Button("REMOVE ALL", buttonStyle, GUILayout.Height(17), GUILayout.Width(100)) && SELECTOBJECT.Count > 0)
        {
            SELECTOBJECT.Clear();
        }
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
        GUILayout.Box("", nullStylle, GUILayout.Height(5));
        }
        catch
        {
            // user delete list object.
        }
    }



    //-- [ MESTHOD ZONE ] -----------------------------------------//
    // 사용자가 선택한 Object 가 Hireachy 혹은, Project 상인지 판단에 따른 명령 실행
    void DataBaseGetTypeToAll(Object obj, string name)
    {
        if (name == "") return; // 공백문자 체크
        if (AssetDatabase.Contains(obj))
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(obj), name);
            EditorUtility.SetDirty(obj);
        }
        else obj.name = name;
    }

    string FirstLetterChange(string text, string option)
    {
        firstLetter = option == "upper" ? firstLetter = text.Substring(0, 1).ToUpper() : firstLetter = text.Substring(0, 1).ToLower();
        text = text.Substring(1, text.Length - 1);
        text = firstLetter + text;
        return text;
    }

    string RemoveCharacter(string text, string option)
    {
        if (text.Length > 1)
        {

            if (option == "first")
            {
                textLength = text.Length;
                text = text.Substring(1, textLength - 1);
            }
            else
            {
                textLength = text.Length;
                text = text.Substring(0, textLength - 1);
            }
        }

        return text;
    }
    
    void RemoveUnityNumbering()
    {
        foreach (var t in SELECTOBJECT.ToList())
        {
            if (t == null)
            {
                SELECTOBJECT.Clear();
                return;
            }
            // int.TryParse 문자를 int 형으로 바꾸고 그 값을 bool 형으로 리턴한다.
            if ((t.name.Substring(t.name.Length - 1) == ")") && (int.TryParse(t.name.Substring(t.name.Length - 2, 1), out numLetter)))
            {
                //Debug.Log(t.name.Substring(t.name.Length - 2, 1));

                for (int i = 0; i < t.name.Length; i++)
                {
                    if (int.TryParse(t.name.Substring(t.name.Length - pNum, nNum), out numLetter))
                    {
                        pNum++;
                        nNum++;
                    }
                    else
                    {
                        t.name = t.name.Replace(t.name.Substring(t.name.Length - pNum), "");
                        break;
                    }
                }
            }


        }
    }
    
    
    //------------------------


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
        Icon_Menu.btnBools[8] = false;
    }
}
