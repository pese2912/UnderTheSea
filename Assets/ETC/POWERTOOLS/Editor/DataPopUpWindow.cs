using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;

public class DataPopUpWindow : EditorWindow
{
    ToDoData ContentsDatas;
    PTData DB;

    ToDoSubCheckList subCheckListDATA;

    TextEditorData TEDB; //텍스트 에디터 셋팅 데이터

    string startTime = string.Empty; // 계산
    string startTimeViews = string.Empty;// VIEW
    string completeTimeViews = string.Empty;
    string currentTime = string.Empty;
    string endTime = string.Empty;



    int elapseDay;
    int elapseHour;
    int elapseMinite;

    string elapseDayText = string.Empty;
    string elapseHourText = string.Empty;
    string elapseMiniteText = string.Empty;

    public int cycleNum;

    float progess;
    string progessText = string.Empty;

    float gap;

    //- 체크 리스트 추가
    // subCheckListDATA
    float checkListPosY;
    string addCheckList = "ADD CHECK LIST";
    
    //------------------

    bool settingEnable;

    Font resultFont;
    Font kfont;

    GUIStyle textStyle = new GUIStyle(); // 텍스트

    GUIStyle headLineGS = new GUIStyle();  // 헤드라인 25p
    GUIStyle timeLineGS = new GUIStyle();  // 타임라인 25p
    GUIStyle boundaryLineGS = new GUIStyle(); // 경계선 3p
    GUIStyle boundaryLine_02GS = new GUIStyle(); // 경계선 3p
    GUIStyle elapseLineGS = new GUIStyle();
    GUIStyle subTitleLineGS = new GUIStyle();
    GUIStyle textArea = new GUIStyle();

    GUIStyle editTextLine = new GUIStyle();

    GUIStyle footer = new GUIStyle();

    GUIStyle myhorizontalSliderThumb2 = new GUIStyle();

    GUIStyle nullStyle = new GUIStyle();
    GUIStyle bgStyle = new GUIStyle();

    GUIStyle checkListStyle = new GUIStyle();
    GUIStyle checkListTexStyle = new GUIStyle();
    GUIStyle checklistIconStyle = new GUIStyle();

    GUIStyle checkListEditMode = new GUIStyle();

    GUIStyle checkListBtnStyle = new GUIStyle();


    Texture2D heaLineTex;
    Texture2D timeLineTex;
    Texture2D boundaryLineTex;
    Texture2D boundaryLineDotTex;
    Texture2D boundaryLineTex_02;
    Texture2D elapseBgTex;
    Texture2D timelineIcon_01Tex;
    Texture2D timelineIcon_02Tex;
    Texture2D clockIconTex;
    Texture2D elapseBoundaryLineTex;
    Texture2D boundaryDotLineTex;

    Texture2D elapseTimeDotTex;
    Texture2D elapseUnderLine_01Tex;
    Texture2D elapseUnderLine_02Tex;
    Texture2D elapseUnderLine_03Tex;
    Texture2D elapseTextTex;
    Texture2D elapseTextTexBg;

    Texture2D subTitleLineTex;
    Texture2D subTitleIconTex;

    Texture2D contentsTextBgTex;

    Texture2D DcTextBG;
    Texture2D DcTextEndBG;
    Texture2D DcTextIcon;

    Texture2D DcFooterStartIcon;
    Texture2D footerBg;
    Texture2D settingIcon;

    Rect rect;

    Vector2 scrollView;

    bool DCSwitch; //세부내용 리로딩
    bool isPlay;

    //셋팅부분 추가
    Color32 bgColr;
    Color32 textColor;
    int bgFontSize;
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle textFildStyleNumber = new GUIStyle();
    float lastRectPosY;
    float posXgap;
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();


    bool ReloadSwitch;

    //생성자
    public DataPopUpWindow(int num )//, string CTIME, string ETIME)
    {
        
        //startTime = STIME;
        cycleNum = num;

        //currentTime = CTIME;
        //endTime = ETIME;      
        startTimeViews = ContentsDatas.startTimeView[cycleNum];
        startTime = ContentsDatas.startTime[cycleNum];

        //체크리스트 데이터 로드
        LoadCheckListData();
    }


    void ScrollBarReset()
    {
        myhorizontalSliderThumb2 = GUI.skin.horizontalScrollbarThumb;
        myhorizontalSliderThumb2.fixedWidth = GUI.skin.horizontalScrollbarThumb.fixedHeight;
        //myhorizontalSliderThumb2.fixedHeight = 25;
        // myhorizontalSliderThumb2.margin.left = -100;
        myhorizontalSliderThumb2.normal.background = GUI.skin.horizontalScrollbarThumb.normal.background;
    }

    //데이터 리셋
    void ResetTextEditorData()
    {
        TEDB.textSizeData = 13;
        TEDB.bgColorData = new Color32(39, 40, 40, 255);
        TEDB.textColorData = new Color32(114, 114, 114, 255);
        EditorUtility.SetDirty(TEDB);

        bgFontSize = TEDB.textSizeData;
        textColor = TEDB.textColorData;
        bgColr = TEDB.bgColorData;
    }

    // 사용자가 플레이를 중단 했을때
    void ReloadSetting()
    {
        bgFontSize = TEDB.textSizeData;
        textColor = TEDB.textColorData;
        bgColr = TEDB.bgColorData;

        // 사용자 임의 컬러
        contentsTextBgTex = MakeTex(1, 1, bgColr); // 배경색
        textArea.normal.textColor = textColor;
        textArea.fontSize = bgFontSize; // 텍스트 area 폰트 사이즈 // 폰트 사이즈
       
    }

    void SaveTextEditorData()
    {
        TEDB.textSizeData = bgFontSize;
        TEDB.textColorData = textColor;
        TEDB.bgColorData = bgColr;
        EditorUtility.SetDirty(TEDB);
    }


    void OnEnable()
    {
      


        try
        {
            

           
            // 저장된 데이터 로드
            LoadData();
         


            bgFontSize = TEDB.textSizeData;
            textColor = TEDB.textColorData;
            bgColr = TEDB.bgColorData;



            // -- [ Texture Get ] --
            heaLineTex = TEXGET("headline.png");
            timeLineTex = TEXGET("timeLine.png");
            boundaryLineTex = TEXGET("boundaryLine.png");
            boundaryLineTex_02 = TEXGET("boundaryLine_02.png");

            elapseBgTex = DB.DBTexture[80];// MakeTex(1, 49, new Color32(48, 49, 49, 255)); //line

            timelineIcon_01Tex = TEXGET("timeline_left_icon_01.png");
            timelineIcon_02Tex = TEXGET("timeline_left_icon_02.png");
            clockIconTex = TEXGET("clock_icon.png");
            elapseBoundaryLineTex = TEXGET("elapseBoundaryLine.png");
            elapseTimeDotTex = TEXGET("elapseTimeDot.png");
            elapseTextTex = TEXGET("elapseText.png");

            elapseTextTexBg = DB.DBTexture[75];             // MakeTex(1, 22, new Color32(51, 57, 63, 255)); //--

            elapseUnderLine_01Tex = DB.DBTexture[76];       // MakeTex(1, 3, new Color32(96, 59, 80, 255)); //--

            elapseUnderLine_02Tex = DB.DBTexture[77];       // MakeTex(1, 3, new Color32(84, 59, 96, 255)); // --

            elapseUnderLine_03Tex = DB.DBTexture[78];       // MakeTex(1, 3, new Color32(58, 76, 96, 255)); //--

            subTitleLineTex = DB.DBTexture[79];             // MakeTex(1, 39, new Color32(39, 40, 40, 255)); //-- 서브 타이틀 라인


            subTitleIconTex = TEXGET("subTitleIcon.png");
            boundaryLineDotTex = TEXGET("boundaryLineDot.png");
            boundaryDotLineTex = TEXGET("boundaryDotLine.png");


            // 컨덴츠 컬러!!! 사용자 임의 조정
            contentsTextBgTex = MakeTex(1, 1, bgColr); 


            footerBg = TEXGET("footer_Bg.png");
            settingIcon= TEXGET("settingIcon.png"); 
            DcFooterStartIcon = TEXGET("DC_Footer_Start_Icon.png");
            gap = 23;

            // --[ text edit ]--
            DcTextBG = TEXGET("DC_Text_BG.png");
            DcTextEndBG = TEXGET("DC_Text_EndBG.png");
            DcTextIcon = TEXGET("DC_Text_Icon.png");

            // -- [ Font Get ] --
            resultFont = (Font)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/font/KATAHDINROUND.OTF", typeof(Font));

            //-- [edit text]--
            editTextLine.font = (Font)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/font/DungGeunMo.otf", typeof(Font));
            editTextLine.alignment = TextAnchor.MiddleLeft;
            editTextLine.normal.textColor = new Color32(225, 225, 225, 255);
            editTextLine.fontSize = 11;

            // -- [ GUIStyle Set ] -
            elapseLineGS.font = resultFont;
            elapseLineGS.fontSize = 27;
            elapseLineGS.alignment = TextAnchor.MiddleCenter;
            elapseLineGS.normal.textColor = new Color32(157, 71, 120, 255);

            headLineGS.normal.background = heaLineTex;
            //headLineGS.alignment = TextAnchor.MiddleLeft;
            headLineGS.normal.textColor = new Color32(198, 198, 198, 255);

            boundaryLineGS.normal.background = boundaryLineTex;
            boundaryLine_02GS.normal.background = boundaryLineTex_02;

            subTitleLineGS.font = resultFont;
            subTitleLineGS.fontSize = 14;
            subTitleLineGS.alignment = TextAnchor.MiddleLeft;
            subTitleLineGS.normal.textColor = new Color32(157, 99, 71, 255);

            //elapseBGGS.normal.background = elapseBgTex;

            timeLineGS.alignment = TextAnchor.MiddleLeft; // 텍스트 정렬방식
            timeLineGS.normal.textColor = new Color32(5, 5, 5, 255);

            textArea.alignment = TextAnchor.UpperLeft;
            textArea.focused.textColor = new Color32(255, 0, 0, 255);
            textArea.normal.textColor = textColor;
            textArea.font = (Font)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/font/NANUMBARUNGOTHIC.TTF", typeof(Font));
            textArea.fontSize = bgFontSize; 
            textArea.margin.left = 20;
            textArea.margin.top = 10;

            //footer.normal.background = DcFooterStartIcon;
            footer.font = resultFont;
            footer.fontSize = 10;
            footer.normal.textColor = new Color32(46, 68, 100, 255);

            nullStyle.normal.background = null;

        }
        catch
        {
            this.Close();
        }




    }

    Texture2D TEXGET(string name)
    {
        return (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/ToDoInfo/" + name, typeof(Texture2D));
    }

    // MAKE TEXTURE
    Texture2D MakeTex(int width, int height, Color32 col)
    {
        Color32[] pix = new Color32[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels32(pix);
        result.Apply();
        return result;
    }

    //서브 체크리스트 데이터로드
    void LoadCheckListData()
    {
        subCheckListDATA = (ToDoSubCheckList)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/SCLData/SCLD_"+ cycleNum.ToString() + ".asset", typeof(ToDoSubCheckList));

        if (subCheckListDATA == null) CreateCheckListData();
       
    }
    
    // 체크리스트 데이터 생성
    void CreateCheckListData()
    {
        subCheckListDATA = ScriptableObject.CreateInstance<ToDoSubCheckList>();
        AssetDatabase.CreateAsset(subCheckListDATA, "Assets/POWERTOOLS/Data/SCLData/SCLD_" + cycleNum.ToString() + ".asset");
    }


    void LoadData()
    {
        ContentsDatas = (ToDoData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/TODODATA.asset", typeof(ToDoData));
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
        TEDB = (TextEditorData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/TEDB.asset", typeof(TextEditorData));

        if (ContentsDatas == null || DB == null)
        {
            Debug.Log("테이터가 없습니다.");
        }
        
    }

    void EditTextLine()
    {

        try
        { 
            GUILayout.BeginHorizontal();
            editTextLine.normal.background = DcTextIcon; // - 로드 문제 부분
            GUI.Box(new Rect(0, 0, 27, 33), "", editTextLine);

            editTextLine.normal.background = DcTextBG;
            GUI.Box(new Rect(27, 0, Screen.width - 7, 33), "", editTextLine);

            GUILayout.BeginArea(new Rect(27, 7, Screen.width - 7, 33));
            editTextLine.normal.background = null;
            ContentsDatas.Contents[cycleNum] = EditorGUILayout.TextField(ContentsDatas.Contents[cycleNum], editTextLine);
            //ContentsDatas.Contents[cycleNum] = GUI.TextField(new Rect(27,0,Screen.width-7,33), ContentsDatas.Contents[cycleNum], editTextLine);
            GUILayout.EndArea();

            editTextLine.normal.background = DcTextEndBG;
            GUI.Box(new Rect(Screen.width - 7, 0, 7, 33), "", editTextLine);
            GUILayout.EndHorizontal();
        }
        catch
        {
            // return 
        }

    }

    // 창에서 포커스 떠났을때.
    void OnLostFocus()
    {
        GUI.FocusControl("");// 포커스 아웃.
    }

    // 유니티를 껐을때 윈도우도 닫히게
    void ExitProject()
    {

    }
 
    
    void OnGUI()
    {

        // 포커스 될때 세부내용 바로 변하도록.
        //if (EditorWindow.focusedWindow)
        //{
        //    //GUI.FocusControl("");
        //}
        

        Event e = Event.current;

        if (e.type == EventType.ValidateCommand)
        { // Validate the command

            if (e.commandName == "Copy" || e.commandName == "Paste")
            {
                e.Use(); // without this line we won't get ExecuteCommand
            }
        }
        else if (e.type == EventType.ExecuteCommand)
        { // Execute the command

            if (e.commandName == "Copy")
            {
                //OnCopy
            }
            else if (e.commandName == "Paste")
            {
                //OnPaste
            }
        }


        //TimeCal();
        EditTextLine();


        // progress top Line
        GUILayout.BeginHorizontal();
        GUI.Box(new Rect(0, 33, 14, 25), "", headLineGS);
        headLineGS.normal.textColor = new Color32(56,16,39,200);
        headLineGS.alignment = TextAnchor.MiddleLeft;
        
       // headLineGS.font = kfont;
        headLineGS.fontSize = 10;
        GUI.Box(new Rect(14, 33, Screen.width, 25), "#"+(cycleNum + 1)+"- INFOMATION", headLineGS);//["+(cycleNum + 1)+"]"
        GUILayout.EndHorizontal();

        Repaint();
        ElapseLine();

        DetailContents();

        Footer();



        IsStopCheck();

    }

    //데이터 리로딩
    void IsStopCheck()
    {
        if (Application.isPlaying)
        {
            isPlay = true;
        }
        if(Application.isPlaying == false && isPlay == true)
        {
            ReloadSetting();
            isPlay = false;
        }
    }

    void Footer()
    {
        GUILayout.BeginHorizontal();
        footer.alignment = TextAnchor.MiddleLeft;
        footer.normal.background = DcFooterStartIcon;
        GUI.Box(new Rect(0, Screen.height - 52, 103, 30), "", footer);
        footer.normal.background = footerBg;
        GUI.Box(new Rect(103, Screen.height - 52, Screen.width, 30), startTimeViews, footer);

        if(!settingEnable) footer.normal.background = DB.DBTexture[44];
        else footer.normal.background = DB.DBTexture[45];

        if ( GUI.Button(new Rect(Screen.width - 77, Screen.height - 52, 77, 29),"", footer))
        {
            EditorUtility.DisplayDialog("Message", "Next Veision Supported", "OK");
            //if (settingEnable) SaveTextEditorData();
            //else GUI.FocusControl("");// 포커스
            //settingEnable = !settingEnable;
        }

        GUILayout.EndHorizontal();
        Setting();

    }


    void Setting()
    {
        if (!settingEnable) return;
        SettingMenu();

        // 사용자 임의 컬러
        contentsTextBgTex = MakeTex(1, 1, bgColr); // 배경색
        textArea.normal.textColor = textColor;
        textArea.fontSize = bgFontSize; // 텍스트 area 폰트 사이즈 // 폰트 사이즈

    }

    void TextFiled()
    {
        textFildStyleNumber = new GUIStyle(GUI.skin.textField);
        textFildStyleNumber.font = DB.DBFonts[1];
        textFildStyleNumber.fontSize = 11;
        textFildStyleNumber.alignment = TextAnchor.MiddleCenter;
    }

    void SettingMenu()
    {
       

        GUILayout.Box("", nullStylle, GUILayout.Height(Screen.height - 154));

        posXgap = 17;
        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        EditorGUILayout.BeginHorizontal();
        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일
        GUILayout.Box("TEXT EDITOR SETTING", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();
        LineSetting(optionStyle, 38, Screen.width, 1); // 한줄 라인

        LineSetting(optionStyle, 7, Screen.width, 25); // 한줄 라인


        optionStyle.normal.textColor = Corr(120, 120, 120, 255);
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y; //LAST RECT POS
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "FONT SIZE / DEFAULT VALUE", optionStyle);

        TextFiled();
        GUI.color = Corr(150, 150, 150, 255);
        GUI.SetNextControlName("TEXTSIZECONTROL");
        bgFontSize = EditorGUI.IntField(new Rect(Screen.width - 110, lastRectPosY + 5, 35, 17), bgFontSize, textFildStyleNumber); // 폰트 사이즈
        GUI.color = Color.white;
        //리셋 버튼
        GUI.color = Corr(160, 120, 120, 255);
        if (GUI.Button(new Rect(Screen.width - 70, lastRectPosY+5 , 47, 18), "RESET", buttonStyle))
        {
            ResetTextEditorData();
        }
        GUI.color = Color.white;

        LineSetting(optionStyle, 11, Screen.width, 3); // 경계선

        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인
        optionStyle.normal.background = null;
        optionStyle.normal.textColor = Corr(120, 120, 120, 255);

        //LAST RECT POS
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;
        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 3, 3, 21), "TEXT COLOR", optionStyle);



        //-- [ TEXT COLOR ]--
        GUI.color = Color.white;
        textColor = EditorGUI.ColorField(new Rect(Screen.width - 110, lastRectPosY + 5, 104, 17), textColor);

        //경계선
        LineSetting(optionStyle, 11, Screen.width, 3); // 한줄 라인
        //BOX
        LineSetting(optionStyle, 12, Screen.width, 25); // 한줄 라인
        lastRectPosY = GUILayoutUtility.GetLastRect().position.y;

        //LABEL;
        GUI.Label(new Rect(10 + posXgap, lastRectPosY + 2, 3, 21), "BG COLOR", optionStyle);

        //-- [ BG COLOR ]--
        bgColr = EditorGUI.ColorField(new Rect(Screen.width - 110, lastRectPosY + 3, 104, 17), bgColr);
        
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

    void TimeLine(float posy, Texture2D icon ,string time)
    {
        GUILayout.BeginHorizontal();
        timeLineGS.normal.background = icon;
        GUI.Box(new Rect(0, posy, 42, 25), "", timeLineGS);
        timeLineGS.normal.background = timeLineTex;
        GUI.Box(new Rect(42, posy, Screen.width, 25), time, timeLineGS);
        GUILayout.EndHorizontal();
    }

    void ProgressCal()
    {
        progess = Mathf.Floor(ContentsDatas.progress[cycleNum]);
        if (float.IsNaN(progess)) progess = 0;
        progessText = TextSum(progess, progessText);
        
    }

    // 시간 계산.
    void TimeCal()
    {
        try
        {

            //시작시간 계산해야함
            if (ContentsDatas.progress[cycleNum] <= 0)
            {
                elapseDayText = "00";
                elapseHourText = "00";
                elapseMiniteText = "00";
                startTimeViews = "STANDBY";
            }
            else
            {
                startTime = ContentsDatas.startTime[cycleNum];
                startTimeViews = ContentsDatas.startTimeView[cycleNum];

                if (ContentsDatas.progress[cycleNum] > 99)
                {
                   if(ContentsDatas.completeTime[cycleNum] != null) currentTime = ContentsDatas.completeTime[cycleNum];
                }
                else if (ContentsDatas.progress[cycleNum] <= 0)
                {
                    currentTime = ContentsDatas.startTime[cycleNum];
                }
                else
                {
                    currentTime = System.DateTime.Now.ToString("u");
                }

                if (ContentsDatas.startTime[cycleNum] != "0000-00-00 00:00:00Z")
                {

                    System.DateTime StartDate = System.Convert.ToDateTime(startTime);
                    System.DateTime currentDate = System.Convert.ToDateTime(currentTime);
                    System.TimeSpan timeCal = currentDate - StartDate;  //TimeSpan 시간계산 메소드

                    // 날자 , 시간 , 분 단위로 세분화 가능
                    elapseDay = timeCal.Days;
                    elapseHour = timeCal.Hours;
                    elapseMinite = timeCal.Minutes;

                    elapseDayText = TextSumTime((float)elapseDay, elapseDayText);
                    elapseHourText = TextSumTime((float)elapseHour, elapseHourText);
                    elapseMiniteText = TextSumTime((float)elapseMinite, elapseMiniteText);
                }
            }

        }
        catch
        {
            this.Close();
        }


    }


    string TextSum(float value, string textValue)
    {
        if (value <= 9) textValue = "00" + value;
        else if (value <= 99) textValue = "0" + value;
        else textValue = value.ToString();
        return textValue;
    }

    string TextSumTime(float value, string textValue)
    {
        if (value <= 9) textValue = "0" + value;
        else textValue = value.ToString();
        return textValue;
    }

    string listNumber;

    void ElapseLine()
    {
        ProgressCal();
        //TimeCal();

        GUILayout.BeginHorizontal();
        elapseLineGS.normal.background = clockIconTex; // icon
        GUI.Box(new Rect(0, 81-gap, 44, 49), "", elapseLineGS);

        // 진행률
        elapseLineGS.normal.textColor = new Color32(157, 71, 120, 255);
        elapseLineGS.normal.background = elapseBgTex;
        GUI.Box(new Rect(33, 81 - gap, 113, 49), progessText+"%", elapseLineGS);
        
        // 경계선
        elapseLineGS.normal.background = elapseBoundaryLineTex;
        GUI.Box(new Rect(146, 81 - gap, 3, 49), "", elapseLineGS);

        // 경과 날자에서 리스트 수로 변경
        elapseLineGS.normal.background = elapseBgTex;
        elapseLineGS.normal.textColor = new Color32(130, 71, 157, 255);

        if (subCheckListDATA.subCheckList.Count > 0)
        {
            if (subCheckListDATA.subCheckList.Count < 10) listNumber = "0" + subCheckListDATA.subCheckList.Count;
            else listNumber = subCheckListDATA.subCheckList.Count.ToString();
        }
        else listNumber = "00";
        GUI.Box(new Rect(149, 81 - gap, 62, 49), listNumber, elapseLineGS); //리스트 수

        // 경계선
        elapseLineGS.normal.background = elapseBoundaryLineTex;
        GUI.Box(new Rect(211, 81 - gap, 3, 49), "", elapseLineGS);

       
        //경과 시간
        elapseLineGS.normal.background = elapseBgTex;
        elapseLineGS.normal.textColor = new Color32(71, 111, 157, 255);
        GUI.Box(new Rect(214, 81 - gap, 56, 49), "", elapseLineGS);
        //GUI.Box(new Rect(214, 81 - gap, 56, 49), elapseHourText, elapseLineGS);

        // : 
        //elapseLineGS.normal.background = elapseTimeDotTex;
        GUI.Box(new Rect(270, 81 - gap, 5, 49), "", elapseLineGS);

        //경과 분
        //elapseLineGS.normal.background = elapseBgTex;
        GUI.Box(new Rect(275, 81 - gap, 50, 49), "", elapseLineGS);
        //GUI.Box(new Rect(275, 81 - gap, 50, 49), elapseMiniteText, elapseLineGS);
        GUI.Box(new Rect(325, 81 - gap, Screen.width, 49), "", elapseLineGS);

        // TIME CHECK 버튼 타임체크 버튼
        elapseLineGS.normal.background = DB.DBTexture[120];
        GUI.color = Corr(255, 255, 255, 150);
        if( GUI.Button(new Rect(223, 92 - gap,96, 31), "", elapseLineGS))
        {
            EditorUtility.DisplayDialog("Message", "Next Veision Supported", "OK");
        }
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();

        //underLine
        GUILayout.BeginHorizontal();
        elapseLineGS.normal.background = elapseUnderLine_01Tex;
        GUI.Box(new Rect(0, 130 - gap, 147, 3), "", elapseLineGS);

        elapseLineGS.normal.background = elapseUnderLine_02Tex;
        GUI.Box(new Rect(147, 130 - gap, 66, 3), "", elapseLineGS);

        elapseLineGS.normal.background = elapseUnderLine_03Tex;
        GUI.Box(new Rect(147+66, 130 - gap, Screen.width, 3), "", elapseLineGS);
        EditorGUILayout.EndHorizontal();

        GUI.Box(new Rect(0, 133 - gap, Screen.width, 3), "", boundaryLineGS);

        //underLine Text       
        GUILayout.BeginHorizontal();
        elapseLineGS.normal.background = elapseTextTex;
        GUI.Box(new Rect(0, 136 - gap, 325, 22), "", elapseLineGS);
        elapseLineGS.normal.background = elapseTextTexBg;
        GUI.Box(new Rect(325, 136 - gap, Screen.width, 22), "", elapseLineGS);
        EditorGUILayout.EndHorizontal();
        GUI.Box(new Rect(0, 158 - gap, Screen.width, 3), "", boundaryLine_02GS);
        
    }


    //텍스트 에디터 부분

    float checkListGap;
    float listLastPosY;
    float memoLastPosY;
    float listLastLinePosY;
    float scrollBarPosX;

    Vector2 scrollCheckList;
    Vector2 scrollTextList;

    GUIStyle textFiledStyle = new GUIStyle();

    bool listComplete;
    bool listEdit;
    List<bool> completeCount = new List<bool>();

    void AddCheckList()
    {

        // subCheckListDATA
        // checkListPosY;
        // addCheckList;

        textFiledStyle = new GUIStyle(GUI.skin.textField);
        textFiledStyle.font = DB.DBFonts[1];
        textFiledStyle.fontSize = 11;
        textFiledStyle.normal.textColor = Corr(0, 0, 0, 255);
        textFiledStyle.alignment = TextAnchor.MiddleLeft;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        


        GUILayout.Box("", nullStyle, GUILayout.Height(139)); //공백


        //GUILayout.BeginHorizontal();

        //if (GUILayout.Button("ADD",buttonStyle))
        //{

        //}

        //GUILayout.EndHorizontal();
        subTitleLineGS.normal.background = subTitleLineTex;
        GUI.Box(new Rect(0, 161 - gap, Screen.width, Screen.height), "", subTitleLineGS); // 배경색
        bgStyle.normal.background = DB.DBTexture[104];
        GUILayout.Box("", bgStyle, GUILayout.Height(24));

        GUILayout.BeginHorizontal();
        

     

        GUI.color = Corr(150,150,150,255);
        addCheckList = GUI.TextField(new Rect(4, 163 - gap, Screen.width-59, 18), addCheckList, textFiledStyle);
        GUI.color = Corr(150,150,150,255);
        if(GUI.Button(new Rect(Screen.width-50, 163 - gap, 45, 18), "ADD", buttonStyle))
        {

            if (addCheckList != "ADD CHECK LIST" && addCheckList != null)
            {
                subCheckListDATA.subCheckList.Add(addCheckList);
                subCheckListDATA.checkComplete.Add(listComplete);
                subCheckListDATA.listEditMode.Add(listEdit); /// 텍스트 에디트 모드 !!!

                EditorUtility.SetDirty(subCheckListDATA);
            }

            addCheckList = "ADD CHECK LIST";
            CompleteCal();
            GUI.FocusControl("");
            
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();

        //스크롤바 시작
        GUI.color = Corr(120, 120, 120, 255);
        scrollCheckList = GUILayout.BeginScrollView(scrollCheckList);
        GUI.color = Color.white;
        bgStyle.normal.background = DB.DBTexture[105];
        GUILayout.Box("", bgStyle, GUILayout.Height(1));

        // 리스트 폰트 스타일
        checkListTexStyle.font = DB.DBFonts[3];
        checkListTexStyle.fontSize = 10;
        checkListTexStyle.alignment = TextAnchor.MiddleLeft;

        // 체크리스트 에디트모드 스타일
        checkListEditMode = new GUIStyle(GUI.skin.textField);
        checkListEditMode.font = DB.DBFonts[3];
        checkListEditMode.fontSize = 10;
        checkListEditMode.alignment = TextAnchor.MiddleLeft;

       
        // LIST 화면에 뿌려주기.
        if (subCheckListDATA.subCheckList.Count > 0)
        {
            
            for(int i = 0; i < subCheckListDATA.subCheckList.Count; i ++)
            {
               
                // 텍스트 색상 , 아이콘
                checkListTexStyle.normal.textColor = subCheckListDATA.checkComplete[i] ? Corr(88, 169, 200, 255) : Corr(210, 210, 210, 255);
                checklistIconStyle.normal.background = subCheckListDATA.checkComplete[i] ? checklistIconStyle.normal.background = DB.DBTexture[109] : checklistIconStyle.normal.background = DB.DBTexture[108];


                if (i%2 == 0) checkListStyle.normal.background = DB.DBTexture[106];
                else checkListStyle.normal.background = DB.DBTexture[107];
                GUILayout.Box("", checkListStyle, GUILayout.Height(18));
                listLastPosY = GUILayoutUtility.GetLastRect().position.y;
                

                GUI.color = Corr(180, 180, 180, 255);
                //아이콘
                GUI.Box(new Rect(4, listLastPosY+3,12,12),"", checklistIconStyle);
                //완료 토글
                //subCheckListDATA.checkComplete[i] = GUI.Toggle(new Rect(Screen.width - 58, listLastPosY +1, 15, 15), subCheckListDATA.checkComplete[i], "");
                GUI.color = Color.white;

                //리스트 내용
                if (!subCheckListDATA.listEditMode[i])
                {
                    GUI.Label(new Rect(20, listLastPosY - 1, Screen.width - 100, 18), subCheckListDATA.subCheckList[i], checkListTexStyle);
                }

                else
                {
                    subCheckListDATA.subCheckList[i] = GUI.TextField(new Rect(20, listLastPosY - 1, Screen.width - 100, 18), subCheckListDATA.subCheckList[i], checkListEditMode);
                }


                // 완료 버튼
                checkListBtnStyle.normal.background = subCheckListDATA.checkComplete[i] ? DB.DBTexture[110] : DB.DBTexture[111];
                if (GUI.Button(new Rect(Screen.width - (50+ scrollBarPosX), listLastPosY + 3, 13, 13), "", checkListBtnStyle))
                {
                    subCheckListDATA.checkComplete[i] = !subCheckListDATA.checkComplete[i];
                    // 퍼센트계산
                    CompleteCal();
                }


                //텍스트 에디트 버튼
                checkListBtnStyle.normal.background = subCheckListDATA.listEditMode[i] ? DB.DBTexture[113] : DB.DBTexture[114];
                if (GUI.Button(new Rect(Screen.width - (34 + scrollBarPosX), listLastPosY + 3, 13, 13), "", checkListBtnStyle))
                {
                    if (subCheckListDATA.listEditMode[i])
                    {
                        EditorUtility.SetDirty(subCheckListDATA);
                        GUI.FocusControl("");
                    }
                    subCheckListDATA.listEditMode[i] = !subCheckListDATA.listEditMode[i];
                }


                // 삭제 버튼
                checkListBtnStyle.normal.background = DB.DBTexture[112];
                if (GUI.Button(new Rect(Screen.width - (18 + scrollBarPosX), listLastPosY + 3, 12, 13), "", checkListBtnStyle))
                {
                    subCheckListDATA.subCheckList.RemoveAt(i);
                    subCheckListDATA.checkComplete.RemoveAt(i);
                    subCheckListDATA.listEditMode.RemoveAt(i);
                    CompleteCal();
                   
                }
            }
        }




        GUILayout.Box("", nullStyle, GUILayout.Height(15));//여백

   
        if (GUILayoutUtility.GetLastRect().position.y > 0) listLastLinePosY = GUILayoutUtility.GetLastRect().position.y;
        GUILayout.EndScrollView();
        
        // 메모 영역
        timeLineGS.normal.background = DB.DBTexture[115];
        GUILayout.Box("", timeLineGS, GUILayout.Height(2)); //라인
        if(GUILayoutUtility.GetLastRect().position.y > 0) memoLastPosY = GUILayoutUtility.GetLastRect().position.y;

        //스크롤바 영역 계산(가로)
        if ((memoLastPosY - listLastLinePosY) < 177) scrollBarPosX = 15;
        else scrollBarPosX = 0;



        bgStyle.normal.background = DB.DBTexture[116];
        GUI.Box(new Rect(0, memoLastPosY + 2, Screen.width, Screen.height), "", bgStyle);

        //메모 아이콘
        bgStyle.normal.background = DB.DBTexture[117];
        GUILayout.Box("", nullStyle, GUILayout.Height(2));
        GUILayout.BeginHorizontal();
        GUILayout.Box("", nullStyle, GUILayout.Width(2));
        GUILayout.Box("", bgStyle, GUILayout.Width(61), GUILayout.Height(18));
        GUILayout.EndHorizontal();

        GUI.color = Corr(50, 50, 50, 255);
        scrollTextList = GUILayout.BeginScrollView(scrollTextList);
        GUI.color = Color.white;
        ContentsDatas.detailContent[cycleNum] = EditorGUILayout.TextArea(ContentsDatas.detailContent[cycleNum], textArea);
        GUILayout.EndScrollView();

        // 스크롤바 끝
        

        GUILayout.Box("",  GUILayout.Height(22));
        checkListGap = 50;
        
      
        
    }

    // 백분율(퍼센트) 계산
    void CompleteCal()
    {
        completeCount = subCheckListDATA.checkComplete.Where(g => g == true).ToList();
        ContentsDatas.progress[cycleNum] = ((float)completeCount.Count / (float)subCheckListDATA.subCheckList.Count * 100f);
    }
    void DetailContents()
    {
        AddCheckList();

 
        //// 이전 메모 부분.
        //GUILayout.BeginHorizontal();
        //subTitleLineGS.normal.background = subTitleLineTex;
        //GUI.Box(new Rect(0, 161 - gap + checkListGap, 20, 39), "", subTitleLineGS);
        //subTitleLineGS.normal.background = subTitleIconTex;
        //GUI.Box(new Rect(20, 161 - gap + checkListGap, 15, 39), "", subTitleLineGS);
        //subTitleLineGS.normal.background = subTitleLineTex;
        //GUI.Box(new Rect(35, 161 - gap + checkListGap, Screen.width, 39), "MEMO", subTitleLineGS);//+ (cycleNum+1), subTitleLineGS);
        //GUILayout.EndHorizontal();

        //// 상세 텍스트
        ////boundaryDotLineTex
        //subTitleLineGS.normal.background = boundaryDotLineTex;
        //GUI.Box(new Rect(0, 200 - gap + checkListGap, 1296, 1), "", subTitleLineGS);

        ////subTitleLineGS.normal.background = boundaryLineDotTex;
        ////GUI.Box(new Rect(0, 200, Screen.width, 1), "", subTitleLineGS);

        //subTitleLineGS.normal.background = contentsTextBgTex;
        //GUI.Box(new Rect(0, 201 - gap + checkListGap, Screen.width, Screen.height), "", subTitleLineGS);



        ////- [서브 텍스트 영역 ]--

        //// GUILayout.BeginHorizontal();

        //if (settingEnable) GUILayout.BeginArea(new Rect(0, 200 - gap + checkListGap, Screen.width, Screen.height - (226 + 92)));
        //else GUILayout.BeginArea(new Rect(0, 200 - gap + checkListGap, Screen.width, Screen.height - 226));

        //GUI.color = new Color32(39, 40, 40, 150);
        //scrollView = GUILayout.BeginScrollView(scrollView);
        //GUI.color = Color.white;

        //GUI.SetNextControlName("contents");
        //ContentsDatas.detailContent[cycleNum] = EditorGUILayout.TextArea(ContentsDatas.detailContent[cycleNum], textArea);//no

        ////ContentsDatas.detailContent[cycleNum] = GUILayout.TextArea(ContentsDatas.detailContent[cycleNum], textArea);//yes

        //GUILayout.EndScrollView();
        //GUILayout.EndArea();

    }


    void Check()
    {
        foreach(var t in ContentsDatas.detailContent[cycleNum])
        {
            //if (t.ToString() == "\n") ;// Debug.Log("sss");
        }
    }

    // 포커스 될때 텍스트 에디터 부분 자동 선택 되도록
    void OnFocus()
    {
        // 성결대학교 4학년  4월 5일 화요일 목 4주 
        // 화 ,수

        EditorGUI.FocusTextInControl("contents");
        //GUI.FocusControl("contents");

    }



}
