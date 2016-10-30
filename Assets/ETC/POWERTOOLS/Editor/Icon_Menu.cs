using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Icon_Menu : EditorWindow
{
    //윈도우 포커스 상태가 아닐때 이름 표시 안함

    PTData DB;

    


    //--[STYLE]--
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    GUIStyle bgStyle = new GUIStyle();
    GUIStyle bgGuiStyle = new GUIStyle();
    GUIStyle fontStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    GUIStyle toolTipStyle = new GUIStyle();


    Texture2D backGround;
    Texture2D nullTex;

    float mousePosX;

    string[] buttonNames = { "","PREFAB PLACER", "PREFAB MAKER","COMBINER", "PIVOT EDITOR", "MESH EDITOR", "TRANSFER", "SOLO", "GROUPER", "RENAMER", "ALIGNER", "TO-DO LIST","PROTOTYPE OBJ",""};

    public static bool IMprefabMaker;
    public static bool IMprefabPlacer;
    public static bool IMcombiner;
    public static bool IMpivotEditor;
    public static bool IMmeshEditor;
    public static bool IMtransfer;
    public static bool IMsolo;
    public static bool IMgrouper;
    public static bool IMrenamer;
    public static bool IMaligner;
    public static bool IMtodoList;

    public static bool IMprotoObj;

    public static bool endList;
    

    bool toolTip;
    bool topLineHide;
    bool showName;

    bool btnEnable;

    float nameBoxPosX;

    float topOptionPosY;
    float sizeX;
    List<Texture2D> iconNormal = new List<Texture2D>();
    List<Texture2D> iconOver = new List<Texture2D>();
    public static List<bool> btnBools = new List<bool>();

    [MenuItem("Tools/Unity Easy Tools")]
    static void ToolBarWindow()
    {
        Icon_Menu window = (Icon_Menu)EditorWindow.GetWindow(typeof(Icon_Menu));
        window.title = "";
        window.Show();
    }



    void OnEnable()
    {
        LoadData();
        InputBoolean();
        nullStylle.normal.background = null;
        backGround = DB.DBTexture[71];//MakeTex(1, 1, new Color32(83,83,83,255));
        bgGuiStyle.normal.background = backGround;

        //--[texture input]--
        TextureInput();

        nullTex = null;
        iconNormal.Add(nullTex);
        iconOver.Add(nullTex);
        // Debug.Log(iconNormal.Count); = 12

        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(150, 150, 150, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        topOptionPosY = 52;
    }

    void InputBoolean()
    {
        btnBools.Add(IMprefabMaker);
        btnBools.Add(IMprefabPlacer);
        btnBools.Add(IMcombiner);
        btnBools.Add(IMpivotEditor);
        btnBools.Add(IMmeshEditor);
        btnBools.Add(IMtransfer);
        btnBools.Add(IMsolo);
        btnBools.Add(IMgrouper);
        btnBools.Add(IMrenamer);
        btnBools.Add(IMaligner);
        btnBools.Add(IMtodoList);
        btnBools.Add(IMprotoObj);
        btnBools.Add(endList);
    }

    void TextureInput()
    {
        iconNormal.Clear(); iconOver.Clear();

        for (int i = 46; i < 68; i++)
        {
            if (i % 2 == 0) iconNormal.Add(DB.DBTexture[i]);
            else iconOver.Add(DB.DBTexture[i]);
        }
        //ADD texture
        iconNormal.Add(DB.DBTexture[81]);
        iconOver.Add(DB.DBTexture[82]);
    }

    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }

    //mouseOverWindow : 마우스가 윈도우를 선택하고 그 위에 있을때만.

    void OnGUI()
    {
        

        GUI.depth = 0;
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", bgGuiStyle);//BACK GROUND
        TopLine();
        if(!topLineHide) TopLineOption();
        else
        {
            bgStyle.normal.background = DB.DBTexture[70];
            GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(1));
        }
        GUILayout.Box("", nullStylle, GUILayout.Height(1));


        IconButtons();

        

        if (!Application.isPlaying && showName)//&& mouseOverWindow
        {
            Repaint();
        }
        

    }


    void IconButtons()
    {
        

        int columns = Mathf.FloorToInt(Screen.width / 42);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < iconNormal.Count; i++)
        {

            if(i%columns == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.Box("", nullStylle, GUILayout.Height(5));
                GUILayout.BeginHorizontal();
            }
            GUILayout.Box("", nullStylle, GUILayout.Width(5));

            if (!btnBools[i]) buttonStyle.normal.background = iconNormal[i];
            else buttonStyle.normal.background = iconOver[i];

            // 실행 버튼
            if (GUILayout.Button("", buttonStyle, GUILayout.Width(42), GUILayout.Height(43)))
            {
                btnBools[i] = !btnBools[i];
                ShowWindows(i, btnBools[i]);
            }


            //&& mouseOverWindow = 마우스가 윈도우를 선택하고 활성화 되었을때
            // 버튼 이름
            if (showName )
            {
                mousePosX = Event.current.mousePosition.x;
                sizeX = (float)buttonNames[i].Length * 7f;
                if (i == 1) sizeX = sizeX - 7;
                if (i == 7) sizeX = sizeX + 5;

                if(mousePosX > 0 && mousePosX < 50) nameBoxPosX = mousePosX - 15;
                else if (mousePosX > Screen.width - 50) nameBoxPosX = mousePosX - (sizeX - 20);
                else nameBoxPosX = mousePosX - (sizeX / 2);

                if (toolTip)
                {
                    toolTipStyle = new GUIStyle(GUI.skin.box);
                    toolTipStyle.font = DB.DBFonts[1];
                    toolTipStyle.normal.textColor = Corr(0, 0, 0, 255);
                    toolTipStyle.alignment = TextAnchor.MiddleCenter;
                    toolTipStyle.fontSize = 10;

                    GUI.color = Corr(255, 150, 150, 230);
                    GUI.Box(new Rect(nameBoxPosX, Event.current.mousePosition.y - 20, sizeX, 17), buttonNames[i], toolTipStyle);
                    GUI.color = Color.white;
                }

                if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) toolTip = true;
                else toolTip = false;
            }

        }
        
        GUILayout.EndHorizontal();
    }

    ////--[WINDOWS]--

    
    
    
    
    
    
    
    
   

    void ShowWindows(int type,bool open)
    {
        switch (type)
        {
            case 0:
                PrefabButton1 PREFABPLACER = (PrefabButton1)EditorWindow.GetWindow(typeof(PrefabButton1));
                if (open) PREFABPLACER.Show();
                else PREFABPLACER.Close();
                break;
            case 1:
                MultiPrefabs MULTIPREFABS = (MultiPrefabs)EditorWindow.GetWindow(typeof(MultiPrefabs));
                if (open) MULTIPREFABS.Show();
                else MULTIPREFABS.Close();
                break;
            case 2:
                Combiner COMBINER = (Combiner)EditorWindow.GetWindow(typeof(Combiner));
                if (open) COMBINER.Show();
                else COMBINER.Close();
                break;
            case 3:
                PivotEditor PIVOTEDITOR = (PivotEditor)EditorWindow.GetWindow(typeof(PivotEditor));
                if (open) PIVOTEDITOR.Show();
                else PIVOTEDITOR.Close();
                break;
            case 4:
                MeshEditor MESHEDITOR = (MeshEditor)EditorWindow.GetWindow(typeof(MeshEditor));
                if (open) MESHEDITOR.Show();
                else MESHEDITOR.Close();
                break;
            case 5:
                Transfer TRANSFER = (Transfer)EditorWindow.GetWindow(typeof(Transfer));
                if (open) TRANSFER.Show();
                else TRANSFER.Close();
                break;
            case 6:
                Solo SOLO = (Solo)EditorWindow.GetWindow(typeof(Solo));
                if (open) SOLO.Show();
                else SOLO.Close();
                break;
            case 7:
                Grouping GROUPER = (Grouping)EditorWindow.GetWindow(typeof(Grouping));
                if (open) GROUPER.Show();
                else GROUPER.Close();
                break;
            case 8:
                Renamer RENAMER = (Renamer)EditorWindow.GetWindow(typeof(Renamer));
                if (open) RENAMER.Show();
                else RENAMER.Close();
                break;
            case 9:
                AlignObject ALIGNER = (AlignObject)EditorWindow.GetWindow(typeof(AlignObject));
                if (open) ALIGNER.Show();
                else ALIGNER.Close();
                break;
            case 10:
                ToDoList TODOLIST = (ToDoList)EditorWindow.GetWindow(typeof(ToDoList));
                if (open) TODOLIST.Show();
                else TODOLIST.Close();
                break;
            case 11:
                prototypeObjects prototypeObjects = (prototypeObjects)EditorWindow.GetWindow(typeof(prototypeObjects));
                if (open) prototypeObjects.Show();
                else prototypeObjects.Close();
                break;
            case 12:
                if (topLineHide) topLineHide = false;
                break;
        }
    }



    void TopLineOption()
    {
        bgStyle.normal.background = DB.DBTexture[16];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(20));
        bgStyle.normal.background = DB.DBTexture[11];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));
        bgStyle.normal.background = DB.DBTexture[12];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(18));
        bgStyle.normal.background = DB.DBTexture[68];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));


        // label
        GUI.Label(new Rect(28, 24, 40, 20), "HIDE OPT", fontStyle);
        GUI.Label(new Rect(28, 46, 40, 20), "SHOW NAME", fontStyle);

        // TOGGLE
        GUI.color = Corr(120, 120, 120, 255);
        topLineHide = GUI.Toggle(new Rect(7, 27, 50, 22), topLineHide, "");
        if (topLineHide)
        {
            iconNormal[12] = DB.DBTexture[72];
            iconOver[12] = DB.DBTexture[72];
        }
        else
        {
            iconNormal[12] = nullTex;
            iconOver[12] = nullTex;
        }
        showName = GUI.Toggle(new Rect(7, 48, 50, 22), showName, "");

        GUI.color = Color.white;
    }

    //--[ 타이틀 라인 ]--
    void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "EASY TOOLS", fontNomalStyle);
    }




    //----[ OPEN ITEM ZONE ]----
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
    //----[ CLOSE ITEM ZONE ]----


}
