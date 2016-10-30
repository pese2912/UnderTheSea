using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class Grouping : EditorWindow
{

    PTData DB;

    Object[] selectObject;
    Object selectGroup;

    Object firstSelectedObj;

    GameObject create;
    GameObject changeObject;
    GameObject SelectObject;

    Vector3 groupPos;

    List<GameObject> SelectedGO = new List<GameObject>();

    List<GameObject> seleceted = new List<GameObject>();
    GameObject activeGameObject;

    string selectGroupName;
    string groupName = "NEW GROUP";
    string emptyName = "EMPTY OBJECT";
    string path;
    string newPath;
    string file;
    string newFile;
    string extension;
    string createFolderPath;
    string firstLetter;
    string titleText;

    string pervFileName;

    float copyButtonYpos;
    float posxGap = 3;

    float toplinePosX;

    int SelectObjectCount;
    int posxGap2 = 40;
    int indexNumber;
    bool optionCreateRoot;
    bool optionFirestSelectPos;
    bool optionReplace;
    bool optionAdd;
    bool optionCreate;

    bool optionRoot;
    bool optionName;
    bool optionPosition;
    bool optionRemove;

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

    void OnEnable()
    {
        LoadData();  //데이터 로드
        nullStylle.normal.background = null;
        this.minSize = new Vector2(200,185);
    }
    
    //저장된 데이터 로드
    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
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
    //----[ CLOSE ITEM ZONE ]----



    //--[ 타이틀 라인 ]--
    void TopLine()
    {
        LineStyle.normal.background = DB.DBTexture[0];
        GUILayout.Box("", LineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));
        fontNomalStyle.normal.textColor = Corr(204, 204, 204, 255);
        fontNomalStyle.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(9, 0, 80, 25), "GROUP MAKER", fontNomalStyle);
    }

    //--[  NEW | ADD ]-- 토글 옵션들 
    void OptionLine()
    {
        toplinePosX = 67;

        bgStyle.normal.background = DB.DBTexture[16];
        GUILayout.Box("", bgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(25));

        
        fontStyle.font = DB.DBFonts[1];
        fontStyle.normal.textColor = Corr(193, 193, 193, 255);
        fontStyle.alignment = TextAnchor.MiddleLeft;
        fontStyle.fontSize = 11;

        // label
        GUI.Label(new Rect(25 + posxGap, 24, 40, 25), "NEW", fontStyle);
        GUI.Label(new Rect(93+3, 24, 40, 25), "ADD", fontStyle);
        GUI.Label(new Rect(94 + toplinePosX, 24, 40, 25), "REMOVE PARENT", fontStyle);

        // 경계선
        boundarylineStyle.normal.background = DB.DBTexture[20];
        GUI.Box(new Rect(61, 24, 3, 25), "", boundarylineStyle);
        GUI.Box(new Rect(61+ toplinePosX, 24, 3, 25), "", boundarylineStyle);

        // toggle
        GUI.color = Corr(120, 120, 120, 255);
        optionCreate = GUI.Toggle(new Rect(7, 28, 50, 25), optionCreate, "");
        if (optionCreate)
        {
            if (optionAdd) ResetOption();
            optionAdd = false;
            optionRemove = false;
        }
        optionAdd = GUI.Toggle(new Rect(72+3, 28, 50, 25), optionAdd, "");
        if (optionAdd)
        {
            if (optionCreate) ResetOption();
            optionCreate = false;
            optionRemove = false;
        }

        optionRemove = GUI.Toggle(new Rect(73 + toplinePosX, 28, 50, 25), optionRemove, "");
        if (optionRemove)
        {
            if (optionAdd || optionCreate) ResetOption();
            optionAdd = false;
            optionCreate = false;
        }

        GUI.color = Color.white;
    }

   


    //--[ OPTION NEW SELECTED ]--
    void OptionNew()
    {

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("OPTIONS", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(27, 71, 3, 21), "ROOT", optionStyle);
        GUI.Label(new Rect(91, 71, 3, 21), "GROUP NAME", optionStyle);
        GUI.Label(new Rect(150 + posxGap2, 71, 3, 21), "POSITION", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(62, 71, 2, 21), "", boundarylineStyle);
        GUI.Box(new Rect(122+posxGap2, 71, 2, 21), "", boundarylineStyle);


        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        optionRoot = GUI.Toggle(new Rect(7, 73, 40, 25), optionRoot, ""); // SELECT NAME
        if (optionRoot)
        {
            //optionName = false;
            //optionPosition = false;
        }
        optionName = GUI.Toggle(new Rect(72, 73, 40, 25), optionName, ""); // SELECT TAG
        if (optionName)
        {
            //optionRoot = false;
            //optionPosition = false;
        }
        optionPosition = GUI.Toggle(new Rect(132+posxGap2, 73, 40, 25), optionPosition, ""); // selection
        if (optionPosition)
        {
            //optionRoot = false;
            //optionName = false;
        }

        GUI.color = Color.white;
    }
    //--[ OPTION ADD SELECTED ]--
    void OptionAdd()
    {

        if (selectGroup == null) selectGroupName = "SELECT GROUP";
        else selectGroupName = "SELECTED GROUP";// : " + selectGroup.name;

        //selectGroupName

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));

        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box(selectGroupName, optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인


        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(10, 72, 3, 21), "GROUP", optionStyle);
        


        //toggle
        GUI.color = Corr(170, 170, 170, 255);

        selectGroup = EditorGUI.ObjectField(new Rect(55, 74, Screen.width-62, 16),selectGroup,typeof(Object));
        //optionName = GUI.Toggle(new Rect(72, 73, 40, 25), optionName, ""); // SELECT TAG
        if (optionName)
        {
            //optionRoot = false;
            //optionPosition = false;
        }
        

        GUI.color = Color.white;
    }


    //--[ NEW OPTION ITEM ]--
    void OptionNewItem()
    {
        
        fontFildStyle = new GUIStyle(GUI.skin.textField);
        FontSetting(fontFildStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);
        
        // 네임선택

        if(optionPosition && !optionName) copyButtonYpos = 110;
        if(optionName && optionPosition) copyButtonYpos = 150;

        if (optionName)
        {

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Box("", nullStylle, GUILayout.Width(3));
            GUI.color = Corr(180, 180, 180, 255);
            groupName = GUILayout.TextField(groupName, fontFildStyle); //텍스트 입력
            GUI.color = Color.white;
            GUILayout.Box("", nullStylle, GUILayout.Width(3));

            EditorGUILayout.EndHorizontal();
        }

        if(optionName && optionPosition)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(15));
            ContentsBLine();
            GUILayout.Box("", nullStylle, GUILayout.Height(2));
            
        }
        //copyButtonYpos
        if (optionPosition)
        {
           
            

            // EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", nullStylle, GUILayout.Width(3));
            GUI.color = Corr(180, 180, 180, 255);
            groupPos = EditorGUILayout.Vector3Field("group position",groupPos,GUILayout.Width(Screen.width-64),GUILayout.Height(12));
            GUI.color = Color.white;
            GUILayout.Box("", nullStylle, GUILayout.Width(3));
            EditorGUILayout.EndHorizontal();


            // Button style
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.font = DB.DBFonts[1];
            buttonStyle.fontSize = 11;
            
            
            if (GUI.Button(new Rect(Screen.width - 50, copyButtonYpos, 43, 18), "COPY", buttonStyle) && Selection.gameObjects.Length > 0)
            {
                groupPos = Selection.gameObjects[0].transform.position;
            }
            
            // 아래여백
            GUILayout.Box("",nullStylle,GUILayout.Height(20));


        }


    }


    //--[ 옵션 리셋 ]--
    void ResetOption()
    {
        //selectGroup = null;
        groupName = "NEW GROUP";
        groupPos = Vector3.zero;
        optionRoot = false;
        optionName = false;
        optionPosition = false;
    }

    void NewOptionItemReset()
    {
        create.name = "NEW GROUP";
        create.transform.position = Vector3.zero;
    }
    
    //처음 선택한 오브젝트 반환
    void FirstSelectedObject()
    {
        if (Selection.objects.Length == 1 && AssetDatabase.Contains(Selection.activeObject))
        {
            firstSelectedObj = Selection.activeObject;
        }
    }
    void OnGUI()
    {
        FirstSelectedObject();  //처음 선택한 오브젝트 반환

        TopLine();
        OptionLine();
        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선

        if (optionCreate)
        {
            OptionNew();
            LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선
            OptionNewItem();
        }

        if (optionAdd)
        {
            OptionAdd();
        }

        // 시작 메세지
        if(!optionCreate && !optionAdd && !optionRemove)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(7));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", nullStylle, GUILayout.Width(5));

            GUI.color = Corr(153, 153, 153, 255);
            EditorGUILayout.HelpBox("Please Checked Option And Apply Grouping", MessageType.Info);
            GUI.color = Color.white;

            GUILayout.Box("", nullStylle, GUILayout.Width(5));
            EditorGUILayout.EndHorizontal();

            GUILayout.Box("", nullStylle, GUILayout.Height(17));
        }

        // 컨덴츠 여백
        if (optionCreate || optionAdd)
        {
            GUILayout.Box("", nullStylle, GUILayout.Height(10));
        }




        ////root 생성 여부
        //optionCreateRoot = EditorGUILayout.Toggle("Create Root Folder", optionCreateRoot);
        ////
        //optionFirestSelectPos = EditorGUILayout.Toggle("First Select Position", optionFirestSelectPos);
        ////그룹이름
        //groupName = EditorGUILayout.TextField(groupName);
        ////그룹 포지션 설정
        //groupPos = EditorGUILayout.Vector3Field("Group Position", groupPos);





        //경계선
        BgStyle.normal.background = DB.DBTexture[23];
        GUILayout.Box("", BgStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(3));

        // 마지막 렉트 위치 가지고 오기
        BgStyle.normal.background = DB.DBTexture[27];
        GUI.Box(new Rect(0, GUILayoutUtility.GetLastRect().position.y + 2, Screen.width, Screen.height),"",BgStyle);

        //사이 띄우기
        GUILayout.Box("", nullStylle, GUILayout.Height(4));



        BgStyle.normal.background = DB.DBTexture[27];
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));

        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.font = DB.DBFonts[1];
        buttonStyle.fontSize = 11;

        //  GUI.Box(new Rect(0,145,Screen.width,Screen.height),"", BgStyle);

      
        GUI.color = Corr(120, 120, 150, 255);
        if(optionRemove) GUI.color = Corr(50, 50, 50, 70);
        if (GUILayout.Button("GROUPING",buttonStyle, GUILayout.MinHeight(20)) && (optionCreate || optionAdd))
        {
            selectObject = Selection.objects;

            if (selectObject.Length == 0)
            {
                create = new GameObject();

                if (optionName) create.name = groupName;
                else create.name = "Empty Object";
                create.transform.position = groupPos;
            }

            //Project 상의 오브젝트 일때
            if (selectObject.Length > 0 && AssetDatabase.Contains(selectObject[0]))
            {

                //if (AssetDatabase.IsValidFolder("Assets" + "/" + groupName)) { Debug.Log("2"); } // 같은 이름의 폴더가 있는지 여부 확인

                System.Array.Clear(selectObject, 0, selectObject.Length);//초기화

                // 디렉토리가 아닌 파일만 배열에 담아준다.
                selectObject = Selection.objects.Where(g => File.GetAttributes(AssetDatabase.GetAssetPath(g)) != FileAttributes.Directory).ToArray();

                if (optionCreate)
                {
                    path = AssetDatabase.GetAssetPath(firstSelectedObj); // 첫번째 선택된 오브젝트의 경로 

                    if (optionRoot)
                    {
                        newPath = "Assets/" + groupName + "/"; // 새로 만든 폴더 경로 
                        AssetDatabase.CreateFolder("Assets", groupName);  // 폴더가 없다면 폴더 생성
                    }
                    else
                    {
                        newPath = Path.GetDirectoryName(path) + "/" + groupName + "/"; // 새로 만든 폴더 경로 
                        AssetDatabase.CreateFolder(Path.GetDirectoryName(path), groupName); // 폴더 생성
                    }

                    // 생성된 폴더 선택
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath(newPath.Substring(0, newPath.Length - 1), typeof(Object));

                }
                if (optionAdd )
                {
                    if(selectGroup == null)
                    {
                        Debug.Log("Select Group Folder");
                    }
                    else if (!AssetDatabase.Contains(selectGroup))
                    {
                        Debug.Log("Select Project Window Folder");
                    }
                    else if (File.GetAttributes(AssetDatabase.GetAssetPath(selectGroup)) != FileAttributes.Directory) // 디렉터리 여부 판별
                    {
                        Debug.Log("Select Folder");
                    }
                    else
                    {
                        //path = AssetDatabase.GetAssetPath(selectGroup);
                        //Debug.Log(path);
                        newPath = AssetDatabase.GetAssetPath(selectGroup) + "/";
                    }
                  
                }


                // 이동
                for (int i = 0; i < selectObject.Length; i++)
                {
                    newFile = Path.GetFileName(AssetDatabase.GetAssetPath(selectObject[i])); // 파일 이름
                    file = AssetDatabase.GetAssetPath(selectObject[i]); // 현재 패스 위치
                    AssetDatabase.MoveAsset(file, newPath + newFile);
                    LoadingBarWindow(selectObject.Length, (float)i);
                }
                EditorUtility.ClearProgressBar();
            }

            // Hierarchy 상의 오브젝트 일때
            else if(selectObject.Length > 0 && !AssetDatabase.Contains(selectObject[0]))
            {
                SelectedGO = Selection.gameObjects.ToList();

                //--[ NEW OPTION ]--
                if (optionCreate)
                {
                    if (!optionRoot)
                    {
                        if(!optionName) groupName = "NEW GROUP";
                        if(!optionPosition) groupPos = Vector3.zero;

                        create = new GameObject();
                        create.name = groupName;
                        create.transform.position = groupPos;
                        
                        //Debug.Log(Selection.activeObject.name); // 처음 선택한 오브젝트 반환
                        if (Selection.activeGameObject.transform.parent != null)
                        {
                            create.transform.SetParent(Selection.activeGameObject.transform.parent.transform);
                        }

                        foreach (var t in SelectedGO)
                        {
                            t.transform.SetParent(create.transform);
                        }

                    }
                    else
                    {
                        if (!optionName) groupName = "NEW GROUP";
                        if (!optionPosition) groupPos = Vector3.zero;

                        create = new GameObject();
                        create.name = groupName;
                        create.transform.position = groupPos;

                        foreach (var t in selectObject)
                        {
                            changeObject = (GameObject)t as GameObject;
                            changeObject.transform.SetParent(create.transform);
                        }
                    }

                    Selection.activeGameObject = create;
                    create = null;

                }

                //--[ ADD OPTION ] --
                if (optionAdd)
                {   
                    if(selectGroup == null)
                    {
                        Debug.LogWarning("Select Group Object");
                    }
                    else
                    {
                        changeObject = (GameObject)selectGroup as GameObject;
                        foreach (var t in SelectedGO)
                        {
                            t.transform.SetParent(changeObject.transform);
                        }
                    }
                }

            }
        }

        GUI.color = Corr(150, 120, 120, 255);
        if (GUILayout.Button("UN GROUP", buttonStyle, GUILayout.MinHeight(20)) && Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.transform.childCount > 0)
            {
                activeGameObject = Selection.activeGameObject;//.transform.GetChild(1);
                indexNumber = activeGameObject.transform.GetSiblingIndex() + 1;
                for (int i = 0; i < activeGameObject.transform.childCount; i++)
                {
                    seleceted.Add(activeGameObject.transform.GetChild(i).gameObject);
                }

                for (int i = 0; i < seleceted.Count; i++)
                {
                    seleceted[i].transform.parent = null;
                    seleceted[i].transform.SetSiblingIndex(indexNumber);
                    indexNumber++;
                }
            }
            seleceted.Clear();
            if (optionRemove) DestroyImmediate(activeGameObject);
            else activeGameObject = null;
        }


        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();
        
        //사이 띄우기
        GUILayout.Box("", nullStylle, GUILayout.Height(5));

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("", nullStylle, GUILayout.Width(3));

        GUI.color = Corr(150, 150, 150, 255);
        if (GUILayout.Button("EXTENSION AUTO GROUPING", buttonStyle, GUILayout.MinHeight(20)) && Selection.objects.Length >0)
        {
            

            // 프로젝트 창에 있고 폴더가 아닌것만 담음
            selectObject = Selection.objects.Where(g => (File.GetAttributes(AssetDatabase.GetAssetPath(g)) != FileAttributes.Directory) && (AssetDatabase.Contains(g))).ToArray();

            
            createFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(firstSelectedObj));

            for (int i = 0; i < selectObject.Length; i++)
            {
                path = AssetDatabase.GetAssetPath(selectObject[i]);
                extension = Path.GetExtension(path).Replace(".","");

                extension = ExtensFolderNameFac(extension);
                //firstLetter = extension.Substring(0, 1).ToUpper();
                //extension = extension.Substring(1, extension.Length - 1);
                //extension = firstLetter + extension;


                newPath = createFolderPath + "/" + extension + "/";
                
                //확장자별 폴더생성 ( 있는지 없는지 판단한다 )
                if (AssetDatabase.IsValidFolder(createFolderPath + "/" + extension)) { }
                else AssetDatabase.CreateFolder(createFolderPath, extension);


                //에셋 이동
                newFile = Path.GetFileName(path);
                AssetDatabase.MoveAsset(path, newPath + newFile);

                LoadingBarWindow(selectObject.Length, (float)i);
            }
            EditorUtility.ClearProgressBar();

        }

        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
        

      
        Footer();  // 하단 상태바

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

        if (Selection.objects.Length > 0) SelectObjectCount = Selection.objects.Length;
        else SelectObjectCount = 0;

        GUI.Box(new Rect(125, Screen.height - 54, 32, 32), SelectObjectCount.ToString(), footerStyle);

    }
  

    string ExtensFolderNameFac(string name)
    {
        switch (name)
        {
            case "cs":
                name = "script";
                break;

            case "js":
                name = "script";
                break;

            case "mat":
                name = "material";
                break;

            case "unity":
                name = "scene";
                break;
            default:
                name = name;
                break;
        }

        firstLetter = name.Substring(0, 1).ToUpper();
        name = name.Substring(1, name.Length - 1);
        name = firstLetter + name;

        return name;
    }


    //Loading bar
    void LoadingBarWindow(int Count, float loadingBarCount)
    {
        EditorUtility.DisplayProgressBar("Auto Grouping...", "Complete : " + Count + " / " + loadingBarCount.ToString(), loadingBarCount / (float)Count);
        
    }


    /*
    string FirstLetter;
    string FirstLetterUpper(string text)
    {
        FirstLetter = text.Substring(0, 1).ToUpper();
        text = text.Substring(1, text.Length - 1);
        text = firstLetter + text;
        return text;
    }
    */


    //컬러 간단
    Color32 Corr(byte r, byte g, byte b, byte a)
    {
        return new Color32(r, g, b, a);
    }

    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[7] = false;
    }

}
