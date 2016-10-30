using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;//  xml 추가





[ExecuteInEditMode]
public class ToDoList : EditorWindow
{

    [MenuItem("NEXTI/ToDoList")]
    static void ToolBarWindow()
    {
        ToDoList window = (ToDoList)EditorWindow.GetWindow(typeof(ToDoList));
        window.minSize = new Vector2(515f, 184f);
        window.Show();
    }


    int FirstNum = 1;
    string contentsField = "";
    Rect rect;

    // 스크립터블 오브젝트 데이터
    // ToDoData todoDatas;

    ToDoData todoDatas;
    jsonData jsonDatas;

    // 서브데이터
    Object subdata;
    
    // 팝업 윈도우

    PTData DBTextures;

    DataPopUpWindow PopUpWindow;
    ToDoSubCheckList subChecjList;

    // 리스트 !!

    //List<string> Contents = new List<string>();
    //List<float> progress = new List<float>();
    //List<string> startTime = new List<string>(); // 시작시간
    //List<string> completeTime = new List<string>(); // 완료 시간
    //List<string> currentTime = new List<string>(); // 경과된 시간



    Vector2 scrollViewPos;

    Texture2D bg_01;
    Texture2D bg_02;
    Texture2D bg_03;
    Texture2D bg_line;
    Texture2D bg_line_02;
    Texture2D bg_line_03;

    Texture2D btn_complete;
    Texture2D btn_complete_alpha;
    Texture2D btn_delete;
    Texture2D btn_delete_alpha;
    Texture2D btn_info;
    Texture2D btn_info_alpha;

    Texture2D slider_bg_01;
    Texture2D slider_bg_02;

    Texture2D slider_bg_complete;

    Texture2D tur;

    Texture2D addListIcon;
    Texture2D addListBg;
    Texture2D addListendLine;
    Texture2D addListButton;
    Texture2D scrollUpButton;
    Texture2D scrollDownButton;

    Texture2D addListButtonOver;
    Texture2D scrollUpButtonOver;
    Texture2D scrollDownButtonOver;


    Texture2D ListEndLine;
    Texture2D ListEndBg;

    Texture2D footerBg;
    Texture2D footerCurrentIcon;
    Texture2D footerCompleteIcon;
    Texture2D footerTotalIcon;
    Texture2D footerLogo;
    Texture2D footerBoundaryLine;
    Texture2D footerSaveData;

    Texture2D stateStart;
    Texture2D stateStop;

    Texture2D bgTEX;
    
    

    GUIStyle myStyle = new GUIStyle();
    GUIStyle myStyle_comp = new GUIStyle();
    GUISkin mySliderStyle = new GUISkin();

    GUIStyle myStyle_buttons = new GUIStyle();

    GUIStyle myhorizontalSlider = new GUIStyle();
    GUIStyle myhorizontalSliderThumb = new GUIStyle();

    GUIStyle fakemyhorizontalSlider = new GUIStyle();
    GUIStyle fakemyhorizontalSliderThumb = new GUIStyle();
    GUIStyle addList = new GUIStyle();
    GUIStyle ListEnd = new GUIStyle();
    GUISkin Slider = new GUISkin();

    GUIStyle footer = new GUIStyle();

    GUIStyle bgTexStyle = new GUIStyle();

    Font resultFont;

    int CompleteList;

   // bool completeSwitch;


    enum state
    {
        stop,
        start,
        complete
    }
    state stateInfo = state.stop;



    //float btn01_Alpha = 0.4f;
    //float btn02_Alpha = 0.4f;
    //float btn03_Alpha = 0.4f;

    float firstLineH = 20 + 13; // 파란라인
    float secontLineH = 45+ 13; // 컨덴츠

    float currentProgress;
    float startCurrentProgress;
    float totalProgress;
    float totalCurrentProgress;

    float footerYpos;

    FileInfo[] FILEINFO;
    DirectoryInfo DIRINFO;


    void OnEnable()
    {

        
        // 스크럽터블 오브젝트 로드
        LoadData();
        //Subdata
        DIRINFO = new DirectoryInfo("Assets/POWERTOOLS/Data/SCLData/");
        //RestartWin();


        addList.alignment = TextAnchor.MiddleLeft;
        addList.normal.textColor = new Color32(201,200,200,255);
        addList.margin.left = 20;

        ListEndLine = TEXGET("ListEndLine.png");
        ListEndBg = TEXGET("ListEndBg.png");

        addListIcon = TEXGET("addText_icon.png");
        addListBg = TEXGET("addText_Bg.png");
        addListendLine = TEXGET("addTextEndLine.png");
        addListButton = TEXGET("add_btn.png");
        scrollUpButton = TEXGET("scrollUpBtn.png");
        scrollDownButton = TEXGET("scrollDownBtn.png");

        addListButtonOver = TEXGET("add_btn_over.png");
        scrollUpButtonOver = TEXGET("scrollUpBtn_over.png");
        scrollDownButtonOver = TEXGET("scrollDownBtn_over.png");


        footerBg = TEXGET("footer_Bg.png");
        footerBoundaryLine = TEXGET("footer_boundary_line.png");
        footerCurrentIcon = TEXGET("footer_current_icon.png");
        footerCompleteIcon = TEXGET("footer_complete_icon.png");
        footerTotalIcon = TEXGET("footer_total_icon.png"); 
        footerLogo = TEXGET("nexti_logo.png");

        stateStart = TEXGET("btn_start.png");
        stateStop = TEXGET("btn_start_al.png");
        footerSaveData = TEXGET("saveDataIcon.png");

        // 이 부분 수정!!!!!!!!
        bgTEX = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/ToDoInfo/to_do_List_bg.png", typeof(Texture2D)) as Texture2D;

        resultFont = (Font)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/font/KATAHDINROUND.OTF", typeof(Font));



        bg_01 = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/bg_01.jpg",typeof(Texture2D)) as Texture2D;
        bg_02 = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/bg_02.jpg", typeof(Texture2D)) as Texture2D;
        bg_03 = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/bg_03.jpg", typeof(Texture2D)) as Texture2D;

        bg_line = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/bg_01_line.jpg", typeof(Texture2D)) as Texture2D;
        bg_line_02 = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/bg_02_line.jpg", typeof(Texture2D)) as Texture2D;
        bg_line_03 = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/bg_03_line.png", typeof(Texture2D)) as Texture2D;

        slider_bg_01 = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/slider_bg_1.png", typeof(Texture2D)) as Texture2D;
        slider_bg_02 = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/slider_bg_2.png", typeof(Texture2D)) as Texture2D;
        slider_bg_complete = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/slider_bg_complete.png", typeof(Texture2D)) as Texture2D;

        btn_complete = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/btn_complete.png", typeof(Texture2D)) as Texture2D;
        btn_complete_alpha = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/btn_complete_alpha.png", typeof(Texture2D)) as Texture2D;
        btn_delete = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/btn_delete.png", typeof(Texture2D)) as Texture2D;
        btn_delete_alpha = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/btn_delete_alpha.png", typeof(Texture2D)) as Texture2D;
        btn_info = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/btn_info.png", typeof(Texture2D)) as Texture2D;
        btn_info_alpha = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/btn_info_alpha.png", typeof(Texture2D)) as Texture2D;

        tur = AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/image/slider_btn.png", typeof(Texture2D)) as Texture2D;
        //EditorCoroutine.Start(WeatheInfo());

        // footer font set.
        footer.font = resultFont;
        footer.fontSize = 10;
        footer.alignment = TextAnchor.MiddleCenter;
        footer.normal.textColor = new Color32(133, 55, 99, 255);

        bgTexStyle.normal.background = bgTEX;
      


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

    //저장된 데이터 로드
    void LoadData()
    {
        todoDatas = (ToDoData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/TODODATA.asset", typeof(ToDoData));
        if(todoDatas == null)
        {
            CreateData();   
        } 
    }

    //데이터 파일 없을시 생성
    void CreateData()
    {
        todoDatas = ScriptableObject.CreateInstance<ToDoData>();
        AssetDatabase.CreateAsset(todoDatas, "Assets/POWERTOOLS/Data/TODODATA.asset");
    }




    // myStyle_buttons.normal.background = btn_complete_alpha;
    // myStyle_buttons.hover.background = btn_complete;


    //완료 시간.
    //System.DateTime.Now.ToString("u");



    string progeressText;

   




    void OnGUI()
    {

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", bgTexStyle);

              
       

        myhorizontalSlider.fixedWidth = 115;
        myhorizontalSlider.fixedHeight = 25;
        //myhorizontalSliderThumb = Slider.horizontalScrollbarThumb;
        myhorizontalSliderThumb.fixedWidth = 25;
        myhorizontalSliderThumb.fixedHeight = 25;
        myhorizontalSliderThumb.margin.left = -100;
        myhorizontalSliderThumb.normal.background = tur;


      

        myStyle.normal.textColor = new Color(0.3f, 0.3f, 0.3f);
        // 진척율 저장
        Event e = Event.current;
        if (e.type == EventType.MouseUp && e.button == 0)
        {
            EditorUtility.SetDirty(todoDatas); // 데이터 디스크 저장
            //AssetDatabase.SaveAssets();
        }

        for (int i = 0; i < todoDatas.Contents.Count; i++)
        {

            StartTime(i);

            if (i % 2 == 0) myStyle.normal.background = bg_02;
            else myStyle.normal.background = bg_03;
            if (i % 2 == 0) myhorizontalSlider.normal.background = slider_bg_01;
            else  myhorizontalSlider.normal.background = slider_bg_02;



            EditorGUILayout.BeginHorizontal();
            //넘버링
            GUI.Box(new Rect(0, secontLineH+(i*25), 26, 25), (FirstNum+i).ToString(), myStyle);
            
            //그룹 라인
            ContentsLine(26, secontLineH + (i * 25), i);
            
            // 컨덴츠
            myStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Box(new Rect(28, secontLineH + (i * 25), Screen.width - 20 - 130 - 50 - (20 * 3), 25), todoDatas.Contents[i], myStyle);
            myStyle.alignment = TextAnchor.MiddleCenter;

            //그룹 라인
            ContentsLine(Screen.width - 120 - 50 - 2 - (20 * 3), secontLineH + (i * 25), i);


            //myStyle.normal.background = slider_bg_01;
            GUI.Box(new Rect(Screen.width - 120 - 50 - (20 * 3), secontLineH + (i * 25), 115, 25), "", myhorizontalSlider);
            
            // 슬라이더바 배경(천천히 나타나는)
            GUI.color = new Color(1, 1, 1, todoDatas.progress[i] / 100);
            myStyle_comp.normal.background = slider_bg_complete;
            GUI.Box(new Rect(Screen.width - 120 - 50 - (20 * 3), secontLineH + (i * 25), 115, 25), "", myStyle_comp);
            GUI.color = Color.white;

            //진행률 텍스트
            if (Mathf.Floor(todoDatas.progress[i]) <= 0 || todoDatas.progress[i] == null || float.IsNaN(todoDatas.progress[i])) progeressText = "0";
            else progeressText = Mathf.Floor(todoDatas.progress[i]).ToString();

            myStyle.alignment = TextAnchor.MiddleRight;
            GUI.Box(new Rect(Screen.width - 80-35 , secontLineH + (i * 25), 35, 25), progeressText + "% ", myStyle);
            myStyle.alignment = TextAnchor.MiddleCenter;

            //슬라이더 조그
            if (float.IsNaN(todoDatas.progress[i])) todoDatas.progress[i] = 0;
            todoDatas.progress[i] = GUI.HorizontalSlider(new Rect(Screen.width - 120 - 50 - (20 * 3), secontLineH + (i * 25), 115, 25), todoDatas.progress[i], 0.0f, 100.0f, fakemyhorizontalSlider, myhorizontalSliderThumb);


            //그룹 라인
            ContentsLine(Screen.width - 80, secontLineH + (i * 25), i);
            GUI.Box(new Rect(Screen.width -78, secontLineH + (i * 25), 78, 25), "", myStyle);

            if(mouseOverWindow) Repaint();
            //--[ 버튼  Complete ] --


            

            // 마우스 오버, 클릭 설정
            ProgressState(i);


            if (secontLineH + (i * 25) > 50)
            {
                if (stateInfo == state.stop) myStyle_buttons.hover.background = stateStart;
                else if (stateInfo == state.start) myStyle_buttons.hover.background = btn_complete;
                else myStyle_buttons.hover.background = btn_complete;


                if (GUI.Button(new Rect(Screen.width - 78, secontLineH + (i * 25), 26, 25), "", myStyle_buttons))
                {


                    if (stateInfo == state.stop)
                    {
                        todoDatas.progress[i] = 1;
                    }
                    else if (stateInfo == state.start)
                    {
                        todoDatas.progress[i] = 100;
                    }

                }


              



                //--[ 버튼  delete ] --
                myStyle_buttons.normal.background = btn_delete_alpha;
                myStyle_buttons.hover.background = btn_delete;
                if (GUI.Button(new Rect(Screen.width - 52, secontLineH + (i * 25), 26, 25), "", myStyle_buttons))
                {


                    try
                    {
                        for (int j = i; j < todoDatas.Contents.Count; j++)
                        {
                            todoDatas.popWindow[j + 1].cycleNum = j;
                            todoDatas.popWindow[j + 1].title = "List " + (j + 1);
                        }
                        
                    }
                    catch { }

                    try
                    {
                        todoDatas.popWindow[i].Close();
                    }
                    catch { }

                    todoDatas.popWindow.RemoveAt(i);
                    todoDatas.Contents.RemoveAt(i);
                    todoDatas.progress.RemoveAt(i);
                    todoDatas.startTime.RemoveAt(i);
                    todoDatas.detailContent.RemoveAt(i);
                    todoDatas.startTimeView.RemoveAt(i);
                    todoDatas.completeTime.RemoveAt(i);
                    todoDatas.completeTimeView.RemoveAt(i);
                    todoDatas.ElapsedTime.RemoveAt(i);
                    todoDatas.completeSwitch.RemoveAt(i);
                    todoDatas.startTimeSwitch.RemoveAt(i);


                   subdata = (ToDoSubCheckList)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/SCLData/SCLD_" + i.ToString() + ".asset", typeof(ToDoSubCheckList));
                    if(subdata != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(subdata));
                    subdata = null;
                    //var ObjectsName = new List<Object>();
                    for(int k = i+1; k < todoDatas.Contents.Count+1; k++)
                    {
                        subdata = (ToDoSubCheckList)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/SCLData/SCLD_" + k.ToString() + ".asset", typeof(ToDoSubCheckList));
                        if (subdata == null) continue;
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(subdata), subdata.name.Replace(k.ToString(),(k-1).ToString()));
                    }
                    subdata = null;
                }



                //--[ 버튼  info ] --
                myStyle_buttons.normal.background = btn_info_alpha;
                myStyle_buttons.hover.background = btn_info;
                if (GUI.Button(new Rect(Screen.width - 26, secontLineH + (i * 25), 26, 25), "", myStyle_buttons))
                {
                    if (secontLineH + (i * 25) < 58) { Debug.Log("ss"); return; }

                    try
                    {
                        todoDatas.popWindow[i].Close();
                    }
                    catch
                    {
                    }

                    todoDatas.popWindow[i] = new DataPopUpWindow(i);
                    todoDatas.popWindow[i] = (DataPopUpWindow)EditorWindow.GetWindow(typeof(DataPopUpWindow));
                    todoDatas.popWindow[i].minSize = new Vector2(212, 269);

                    //PopUpWindow = new DataPopUpWindow(i);
                    //PopUpWindow = (DataPopUpWindow)EditorWindow.GetWindow(typeof(DataPopUpWindow));
                    //PopUpWindow.minSize = new Vector2(326, 269);
                    todoDatas.popWindow[i].title = "List " + (i + 1);
                    todoDatas.popWindow[i].Show();
                 

                }

            }

            EditorGUILayout.EndHorizontal();
            ListEnd.normal.background = ListEndLine;
            GUI.Box(new Rect(0, secontLineH + (25* todoDatas.Contents.Count),Screen.width,1),"",ListEnd);
            ListEnd.normal.background = ListEndBg;
            GUI.Box(new Rect(0, secontLineH + 1 + (25 * todoDatas.Contents.Count), Screen.width, Screen.height), "", ListEnd);
            //secontLineH

            //GUILayout.BeginArea(new Rect(0, 45*i, Screen.width, 25), myStyle);
            //GUILayout.Label(FirstNum+i.ToString(),GUILayout.MinWidth(20));
            //GUILayout.EndArea();

            // 시작시간 체크
           

        }



        //일부값(현재 모든 진행률의 합) ÷ 현재 생성된 모든 진행율의 최고값(7개면 700%)x 100
        // currentProgress;
        // totalProgress;

        CurrentProgressCal();
        Footer();
        // GUILayout.BeginArea(new Rect(0, Screen.height - 57, Screen.width, 50));
        // EditorGUILayout.HelpBox("Current List : " + todoDatas.Contents.Count + " Complete List : " + "" + "Total Progress : " + Mathf.Floor(totalCurrentProgress).ToString() + "%", MessageType.Info);
        //GUILayout.EndArea();

        //GUI.Box(new Rect(0, Screen.height - 43, Screen.width, 20), "Current List : " + todoDatas.Contents.Count + " Complete List : " + "" + "Total Progress : " +Mathf.Floor(totalCurrentProgress).ToString()  + "%");

        //Repaint();
        ////https://www.youtube.com/watch?v=aropZRQberw 유니티 시간별로
        //GUI.skin.horizontalScrollbarThumb = GUI.skin.horizontalScrollbarThumb;
        //GUI.skin.horizontalScrollbar = GUI.skin.horizontalScrollbar;


        // -[ bue line ]-
        myStyle.alignment = TextAnchor.MiddleCenter;
        myStyle.normal.background = bg_01;
        myStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

        EditorGUILayout.BeginHorizontal();
        GUI.Box(new Rect(0, firstLineH, 26, 25), "#", myStyle);
        TiTleLine(26);
        GUI.Box(new Rect(28, firstLineH, Screen.width - 20 - 130 - 50 - (20 * 3), 25), "Description", myStyle);
        TiTleLine(Screen.width - 122 - 50 - (20 * 3));
        GUI.Box(new Rect(Screen.width - 120 - 50 - (20 * 3), firstLineH, 150, 25), "Progress", myStyle);
        TiTleLine(Screen.width - 20 - (20 * 3));
        GUI.Box(new Rect(Screen.width - 78, firstLineH, 78, 25), "Buttons", myStyle);
        EditorGUILayout.EndHorizontal();

        TopLine();
        Repaint();
    }

    //completeSwitch
    void ProgressState(int num)
    {
        if (todoDatas.progress[num] == 0) stateInfo = state.stop;
        else if (todoDatas.progress[num] >= 1 && todoDatas.progress[num] <= 99) stateInfo = state.start;
        else if (todoDatas.progress[num] > 99) stateInfo = state.complete;

        switch (stateInfo)
        {
            case state.stop:
                myStyle_buttons.normal.background = stateStop;
                todoDatas.completeSwitch[num] = false;
                break;
            case state.start:
                myStyle_buttons.normal.background = stateStart;
                todoDatas.completeSwitch[num] = false;
                break;
            case state.complete:
                myStyle_buttons.normal.background = btn_complete;
                break;
        }

       // InputCompleteTime(num);
    }

    void InputCompleteTime(int num)
    {
        if (todoDatas.completeSwitch[num] == false && stateInfo == state.complete)
        {
            todoDatas.completeTime[num] = System.DateTime.Now.ToString("u");
            todoDatas.completeTimeView[num] = System.DateTime.Now.ToString("g");
           // ElapsedCal(num);
            todoDatas.completeSwitch[num] = true;
        }
    }

    void ElapsedCal(int num)
    {
        System.DateTime startDate = System.Convert.ToDateTime(todoDatas.startTime[num]);
        System.DateTime completeDate = System.Convert.ToDateTime(System.DateTime.Now.ToString("u"));
        System.TimeSpan timeCal = completeDate - startDate;
        todoDatas.ElapsedTime[num] = timeCal.Days + "Days " + timeCal.Hours + ":" + timeCal.Minutes;
    }


    void StartTime(int num)
    {
        if (todoDatas.Contents.Count > 0 && todoDatas.progress[num] > 0 && todoDatas.startTimeSwitch[num] == false)
        {
            todoDatas.startTime[num] = System.DateTime.Now.ToString("u");
            todoDatas.startTimeView[num] = System.DateTime.Now.ToString("g");

            todoDatas.startTimeSwitch[num] = true;
        }
        else if (todoDatas.Contents.Count > 0 && todoDatas.progress[num] <= 0)
        {
            //todoDatas.startTime.Add("0000-00-00 00:00:00Z");
            //todoDatas.startTimeView.Add("0/0/0000 0:00 TM");
            todoDatas.startTime[num] = "0000-00-00 00:00:00Z";
            todoDatas.startTimeView[num] = "Standby";

            todoDatas.startTimeSwitch[num] = false;
        }
    }
    List<string> chickListSub;
    void TopLine()
    {
        EditorGUILayout.BeginHorizontal();

        addList.normal.background = addListIcon;
        addList.hover.background = addListIcon;
        GUI.Box(new Rect(0, 0, 28, 33), "", addList);
        addList.normal.background = addListBg;
        addList.hover.background = addListBg;
        GUI.Box(new Rect(28, 0, Screen.width - 108, 33), "", addList);

       
        GUILayout.BeginArea(new Rect(28, 8, Screen.width - 108, 33));
        addList.normal.background = null;
        addList.hover.background = null;
        contentsField = EditorGUILayout.TextField(contentsField, addList);
        //contentsField = GUI.TextField(new Rect(28, 0, Screen.width - 108, 33), contentsField, addList);
        GUILayout.EndArea();
        addList.normal.background = addListendLine;
        GUI.Box(new Rect(Screen.width - 83, 0, 3, 33), "", addList);


        //// Enter 입력시 데이터 add
        ////Event e = 
        //Event k = Event.current;
        //if (k.type == EventType.keyDown && k.keyCode == KeyCode.Return)
        //{
        //    if (contentsField != "")
        //    {
        //        todoDatas.completeSwitch.Add(false);
        //        todoDatas.Contents.Add(contentsField);
        //        todoDatas.progress.Add(0);
        //        todoDatas.startTime.Add(System.DateTime.Now.ToString("u"));
        //        todoDatas.startTimeView.Add(System.DateTime.Now.ToString("g"));
        //        todoDatas.detailContent.Add("Detail Content...");
        //        todoDatas.completeTimeView.Add("0");
        //        todoDatas.ElapsedTime.Add("0");
        //        todoDatas.completeTime.Add("0");

                
        //        EditorUtility.SetDirty(todoDatas); // 데이터 디스크 저장

        //        contentsField = "";
        //    }
        //}


        addList.normal.background = addListButton;
        addList.hover.background = addListButtonOver;
        // 신규 리스트 생성
        if (GUI.Button(new Rect(Screen.width - 80, 0, 36, 33), "", addList))
        {
           // Debug.Log(DBTextures.DBTexture[0].name);
            if (contentsField != "")
            {
                todoDatas.popWindow.Add(PopUpWindow);
                todoDatas.completeSwitch.Add(false);
                todoDatas.startTimeSwitch.Add(false);
                todoDatas.Contents.Add(contentsField);
                todoDatas.progress.Add(0);
                todoDatas.detailContent.Add("Detail Content...");
                todoDatas.completeTimeView.Add("0");
                todoDatas.ElapsedTime.Add("0");
                todoDatas.completeTime.Add("0");

                

                
                // START TIME
                todoDatas.startTime.Add("0000-00-00 00:00:00Z");
                todoDatas.startTimeView.Add("0/0/0000 0:00 TM");

                
                

                //todoDatas.startTime.Add(System.DateTime.Now.ToString("u"));
                //todoDatas.startTimeView.Add(System.DateTime.Now.ToString("g"));

                EditorUtility.SetDirty(todoDatas); // 데이터 디스크 저장

                contentsField = "";
            }
        }

        addList.normal.background = scrollDownButton;
        addList.hover.background = scrollDownButtonOver;
        if (GUI.Button(new Rect(Screen.width - 44, 0, 23, 33), "", addList))
        {
            
            if((25 * todoDatas.Contents.Count) - (secontLineH*-1) > 133) { 
                secontLineH -= 25;
            }
        }

        addList.normal.background = scrollUpButton;
        addList.hover.background = scrollUpButtonOver;
        if (GUI.Button(new Rect(Screen.width - 21, 0, 21, 33), "", addList))
        {

            if (secontLineH < 58)
            {
                secontLineH += 25;

            }
        }

        EditorGUILayout.EndHorizontal();
    }

    void Footer()
    {

        CompleteList = 0;

        for(int i = 0; i < todoDatas.Contents.Count; i++)
        {
            if(todoDatas.progress[i] > 99)
            {
                CompleteList++;
            }
        }

        //footerBg = TEXGET("footer_Bg.png");
        //footerBoundaryLine = TEXGET("footerBoundaryLine.png");
        //footerCurrentIcon = TEXGET("footer_current_icon.png");
        //footerCompleteIcon = TEXGET("footer_complete_icon.png");
        //footerTotalIcon = TEXGET("footer_total_icon.png");
        //footerLogo = TEXGET("nexti_logo.png");
        //resultFont = (Font)AssetData

        footerYpos = Screen.height - 52;

        footer.normal.background = footerCurrentIcon;
        GUI.Box(new Rect(0, footerYpos, 109, 30), "", footer);

        //current list
        footer.normal.background = footerBg;
        GUI.Box(new Rect(109, footerYpos, 31, 30), todoDatas.Contents.Count.ToString(), footer);

        footer.normal.background = footerBoundaryLine;
        GUI.Box(new Rect(140, footerYpos, 3, 30), "", footer);

        footer.normal.background = footerCompleteIcon;
        GUI.Box(new Rect(143, footerYpos, 84, 30), "", footer);

        //complete list
        footer.normal.background = footerBg;
        GUI.Box(new Rect(227, footerYpos, 24, 30), CompleteList.ToString() , footer);

        footer.normal.background = footerBoundaryLine;
        GUI.Box(new Rect(251, footerYpos, 3, 30), "", footer);

        footer.normal.background = footerTotalIcon;
        GUI.Box(new Rect(254, footerYpos, 121, 30), "", footer);


        //total progress
        footer.normal.background = footerBg;
        GUI.Box(new Rect(375, footerYpos, 41, 30), Mathf.Floor(totalCurrentProgress).ToString() + "%" , footer);

        //bg
        footer.normal.background = footerBg;
        GUI.Box(new Rect(416, footerYpos, Screen.width-64, 30), "", footer);

        // save Data 버튼 xml저장
        footer.normal.background = footerSaveData;
        if(GUI.Button(new Rect(375+41, footerYpos, 91, 30), "", footer))
        {
            saveDataWin saveDataWindow = new saveDataWin(todoDatas);
            saveDataWindow = (saveDataWin)EditorWindow.GetWindow(typeof(saveDataWin));
            saveDataWindow.title = "Save Data";
            saveDataWindow.minSize = new Vector2(219, 137);
            saveDataWindow.Show();

            ////string path = EditorUtility.SaveFilePanel("XML Save", "", "data", "xml");

            ////if (path.Length > 0)
            ////{

            ////    //직렬화
            ////    XmlSerializer ser = new XmlSerializer(typeof(Root)); //데이터를 xml로 파싱

            ////    //텍스트 파일 저장
            ////    TextWriter textWriter = new StreamWriter(path);

            ////    Root root = new Root();
            ////    List<xmlData> list = new List<xmlData>();

            ////    int num = 1;
            ////    for (int i = 0; i < todoDatas.Contents.Count; i++)
            ////    {
            ////        if (todoDatas.progress[i] < 99) continue;
            ////        xmlData xmldatas = new xmlData();
            ////        xmldatas.number = num.ToString();
            ////        xmldatas.description = todoDatas.Contents[i];
            ////        xmldatas.startTime = todoDatas.startTimeView[i];
            ////        xmldatas.completeTime = todoDatas.completeTimeView[i];
            ////        xmldatas.elapsedTime = todoDatas.ElapsedTime[i];
            ////        xmldatas.detailContents = todoDatas.detailContent[i];
            ////        list.Add(xmldatas);
            ////        num++;
            ////    }
            ////    root.RecordXmlData = new xmlData[list.Count];
            ////    root.RecordXmlData = list.ToArray();
            ////    ser.Serialize(textWriter, root);
            ////}

        }


       // logo
       // footer.normal.background = footerLogo;
       // GUI.Box(new Rect(Screen.width-64, footerYpos, 64, 30), "", footer);
    }


    // Total Progress 계산.
    void CurrentProgressCal()
    {
        startCurrentProgress = 0;
        for (int i = 0; i < todoDatas.progress.Count; i++)
        {
          startCurrentProgress += todoDatas.progress[i];
        }

        currentProgress = startCurrentProgress;
        totalProgress = todoDatas.Contents.Count * 100;


        if(todoDatas.Contents.Count > 0 )
        {
            totalCurrentProgress = currentProgress / totalProgress * 100;
        }
        else
        {
            totalCurrentProgress = 0;
        }

        // NaN 리턴 방지
        //if (float.IsNaN(totalCurrentProgress)) totalCurrentProgress = 0;
        //else
        //{
        //    totalCurrentProgress = currentProgress / totalProgress * 100;
        //}
    }

    void ContentsLine(float posx, float posy , int num)
    {
        if (num % 2 == 0) myStyle.normal.background = bg_line_02;
        else myStyle.normal.background = bg_line_03;

        GUI.Box(new Rect(posx, posy, 2, 25), "", myStyle);
        if (num % 2 == 0) myStyle.normal.background = bg_02;
        else myStyle.normal.background = bg_03;
    }


    void TiTleLine(float posx)
    {
        myStyle.normal.background = bg_line;
        GUI.Box(new Rect(posx, firstLineH, 2, 25), "", myStyle);
        myStyle.normal.background = bg_01;
    }

    void OnDestroy()
    {
        Icon_Menu.btnBools[10] = false;
    }

    //IEnumerator WeatheInfo()
    //{

    //    string url = "http://api.openweathermap.org/data/2.5/find?q=Seoul&type=accurate&mode=xml&lang=nl&units=metric&appid=ca1322e8dcbcb2d2b9f3558e87ca2208";
    //    WWW www = new WWW(url);
    //    yield return www;
    //    if (www.error == null)
    //    {


    //        Debug.Log("Loaded following XML " + www.data);
    //        XmlDocument xmlDoc = new XmlDocument();
    //        xmlDoc.LoadXml(www.data);
    //        Debug.Log(xmlDoc.Name); 
    //        Debug.Log("City: " + xmlDoc.SelectSingleNode("cities/list/item/city/@name").InnerText);
    //        Debug.Log("Temperature: " + xmlDoc.SelectSingleNode("cities/list/item/temperature/@value").InnerText);
    //        Debug.Log("humidity: " + xmlDoc.SelectSingleNode("cities/list/item/humidity /@value").InnerText);
    //        Debug.Log("Cloud : " + xmlDoc.SelectSingleNode("cities/list/item/clouds/@value").InnerText);
    //        Debug.Log("Title: " + xmlDoc.SelectSingleNode("cities /list/item/weather/@value").InnerText);
    //    }
    //    else
    //    {
    //        Debug.Log("ERROR: " + www.error);

    //    }

    //}





}


