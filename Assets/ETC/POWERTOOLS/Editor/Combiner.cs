using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Enum = System.Enum;

public class Combiner : EditorWindow
{


    PTData DB;

    GUIStyle BgStyle = new GUIStyle();
    GUIStyle LineStyle = new GUIStyle();
    GUIStyle fontNomalStyle = new GUIStyle();
    GUIStyle boundarylineStyle = new GUIStyle();
    GUIStyle nullStylle = new GUIStyle();
    //GUIStyle fontStyle = new GUIStyle();
    GUIStyle optionStyle = new GUIStyle();
    GUIStyle buttonStyle = new GUIStyle();
    //GUIStyle footerStyle = new GUIStyle();
    //GUIStyle sceneStyle = new GUIStyle();
    //GUIStyle sceneIcon = new GUIStyle();
    GUIStyle fontFildStyle = new GUIStyle();


    bool optionNew, optionDelNew, optionMulti;



    MeshFilter[] meshFilters;
    CombineInstance[] combines;
    Material[] combineMaterial;

    string dicName;
    string ObjMaterialName;

    float togglePosY, optionTextPosY;
    float labelPosX, labelPosX2;

    Mesh newMesh;

    List<MeshFilter> meshFilterList = new List<MeshFilter>(); //여기여기
    List<CombineInstance> combineList = new List<CombineInstance>();
    List<GameObject> multipleCombine = new List<GameObject>();
    List<GameObject> multiPleAllObj = new List<GameObject>();
    string NewCombineName = "Combine New Name";

    List<GameObject> SELECTMeshObject = new List<GameObject>();
    

    Dictionary<string, List<GameObject>> DICFILTER = new Dictionary<string, List<GameObject>>();

    
    void OnEnable()
    {
        LoadData();  //데이터 로드

       
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
        GUI.Label(new Rect(9, 0, 80, 25), "COMBINER", fontNomalStyle);
    }
    //--[ OPTIONS ]--
    //labelPosX, labelPosX2;
    void Options()
    {
        togglePosY = 48;
        optionTextPosY = 46;
        labelPosX = 36;
        labelPosX2 = 11;

        EditorGUILayout.BeginHorizontal();

        optionStyle.normal.background = DB.DBTexture[18];
        GUILayout.Box("", optionStyle, GUILayout.Width(27), GUILayout.Height(21));
        optionStyle.normal.background = DB.DBTexture[17];
        FontSetting(optionStyle, 1, 11, TextAnchor.MiddleLeft, 55, 54, 54, 255); // 폰트 스타일

        GUILayout.Box("SELECT OPTION", optionStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(21));
        EditorGUILayout.EndHorizontal();

        LineSetting(optionStyle, 19, Screen.width, 21); // 한줄 라인



        //Label
        optionStyle.normal.background = null;
        GUI.Label(new Rect(27, optionTextPosY, 3, 21), "NEW", optionStyle);
        GUI.Label(new Rect(122 - labelPosX, optionTextPosY, 3, 21), "DELETE & NEW", optionStyle);
        GUI.Label(new Rect(218 - labelPosX2, optionTextPosY, 3, 21), "MULTIPLE", optionStyle);


        //경계선
        boundarylineStyle.normal.background = DB.DBTexture[21];
        GUI.Box(new Rect(92 - labelPosX, 46, 2, 21), "", boundarylineStyle);
        GUI.Box(new Rect(188 - labelPosX2, 46, 2, 21), "", boundarylineStyle);



        //toggle
        GUI.color = Corr(170, 170, 170, 255);
        optionNew = GUI.Toggle(new Rect(7, togglePosY, 40, 25), optionNew, ""); // SELECT POSITION
        if (optionNew)
        {
            optionDelNew = false; optionMulti = false;
        }
        optionDelNew = GUI.Toggle(new Rect(102 - labelPosX, togglePosY, 40, 25), optionDelNew, ""); // SELECT TAG
        if (optionDelNew)
        {
            optionNew = false; optionMulti = false;
        }
        optionMulti = GUI.Toggle(new Rect(198 - labelPosX2, togglePosY, 40, 25), optionMulti, ""); // selection
        if (optionMulti)
        {
            optionDelNew = false; optionNew = false;
        }
        GUI.color = Color.white;
    }

    void OnGUI()
    {

        TopLine();
        LineSetting(boundarylineStyle, 29, Screen.width, 1); // 밝은 경계선
        Options();
        LineSetting(boundarylineStyle, 22, Screen.width, 1); // 밝은 경계선



        // 시작 메세지
        if (!optionNew && !optionMulti && !optionDelNew)
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

       

        if (optionNew || optionMulti || optionDelNew)
        {
            fontFildStyle = new GUIStyle(GUI.skin.textField);
            FontSetting(fontFildStyle, 1, 11, TextAnchor.MiddleLeft, 50, 50, 50, 255);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Box("", nullStylle, GUILayout.Width(3));
            GUI.color = Corr(180, 180, 180, 255);
            NewCombineName = GUILayout.TextField(NewCombineName, fontFildStyle); //텍스트 입력
            GUI.color = Color.white;
            GUILayout.Box("", nullStylle, GUILayout.Width(3));

            EditorGUILayout.EndHorizontal();
        }

        //컨덴츠 여백
        if (optionNew || optionMulti || optionDelNew)
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


        GUI.color = Corr(150, 150, 150, 255);
        if (GUILayout.Button("COMBINE", buttonStyle, GUILayout.MinHeight(20)) && (optionNew || optionMulti || optionDelNew) && Selection.gameObjects.Length > 0)
        {
            Filter();
        }

        GUILayout.Box("", nullStylle, GUILayout.Width(3));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        //if (GUILayout.Button("test2"))
        //{
        //    string k = Selection.gameObjects[0].GetComponent<Renderer>().sharedMaterial.GetInstanceID().ToString();
        //    Debug.Log(k);
        //    //string kk =  Selection.activeGameObject.renderer.materials[1 % Selection.activeGameObject.renderer.materials.length].mainTexture.name;

        //}


        //if (GUILayout.Button("OBJECT COMBINE"))
        //{
        //    //새로운 오브젝트 생성
        //    GameObject go = new GameObject();
        //    go.AddComponent<MeshFilter>();
        //    go.AddComponent<MeshRenderer>();


        //    if (SELECTMeshObject != null) SELECTMeshObject.Clear();
        //    foreach (var t in Selection.gameObjects)
        //    {
        //        SELECTMeshObject.Add(t);
        //    }

        //    meshFilters = new MeshFilter[SELECTMeshObject.Count];

        //    for (int i = 0; i < SELECTMeshObject.Count; i++)
        //    {
        //        meshFilters[i] = SELECTMeshObject[i].transform.GetComponent<MeshFilter>();
        //    }
        //    // CombineInstance : 메쉬가 Mesh.CombineMeshes를 사용하여 결합하는 방법을 설명하는데 사용하는 구조입니다..
        //    combines = new CombineInstance[meshFilters.Length];


        //    for (int i = 0; i < meshFilters.Length; i++)
        //    {
        //        combines[i].mesh = meshFilters[i].sharedMesh;
        //        combines[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //        meshFilters[i].gameObject.active = false; // 기존에 선택된 오브젝트 비활성화
        //    }

        //    combineMaterial = new Material[meshFilters.Length];
        //    for (int i = 0; i < meshFilters.Length; i++)
        //    {
        //        combineMaterial[i] = SELECTMeshObject[i].transform.GetComponent<MeshRenderer>().sharedMaterial;
        //    }


        //    go.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        //    go.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combines, false); //만들어진 콤바인 인스턴트를 결합 subMesh 남겨두지 않음.
        //    go.transform.GetComponent<MeshRenderer>().sharedMaterials = combineMaterial; // 매터리얼 적용.
        //    //go.transform.GetComponent<MeshRenderer>().sharedMaterial = combineMaterial[0]; //sharedMaterials = combineMaterial;

        //    go.transform.gameObject.active = true;// 신규 생성된 오브젝트 활성화.


        //}

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


    //string dicName;
    //string ObjMaterialName;

    // 선택된 오브젝트중 같은 매터리얼을 가진것끼리 딕셔너리 사용하여 list로 묶는다. DIC < STRING , LIST >
    void Filter()
    {
        DICFILTER.Clear();
        if (optionMulti)
        {
            //multiPleAllObj.Clear();
            multiPleAllObj = Selection.gameObjects.ToList();
        }

        foreach (var t in Selection.gameObjects)
        {
            if (t.GetComponent<MeshRenderer>() != null)
            {
                dicName = t.GetComponent<MeshRenderer>().sharedMaterial.name;

                while (dicName.Contains(" (Instance)"))
                {
                    dicName = dicName.Replace(" (Instance)", "");
                }
                DICFILTER[dicName] = new List<GameObject>();
            }
        }
        foreach (var t in Selection.gameObjects)
        {
            if (t.GetComponent<MeshRenderer>() != null)
            {
                dicName = t.GetComponent<MeshRenderer>().sharedMaterial.name;

                while (dicName.Contains(" (Instance)"))
                {
                    dicName = dicName.Replace(" (Instance)", "");
                }
                DICFILTER[dicName].Add(t);
            }
        }

        //keyName = DICFILTER.Keys.ToList();

        //for(int i = 0; i < keyName.Count; i++)
        //{
        //    MeshCombine(DICFILTER[keyName[i]], keyName[i]);
        //}

        foreach (var t in DICFILTER)
        {
            MeshCombine(t.Value, t.Key,"not");
        }

        if (optionMulti)
        {
            MeshCombine(multipleCombine, "not", "multi");
        }

    }


    //string NameChange(string name)
    //{
    //    while (name.Contains(" (Instance)"))
    //    {
    //        name = name.Replace(" (Instance)", "");
    //    }
    //    return name;
    //}


    void MeshCombine(List<GameObject> list,string name,string multiValue)
    {

       // Debug.Log(list[0]);
       // Debug.Log(list.Count);
        GameObject go = new GameObject();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.name = NewCombineName;

        meshFilterList.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            meshFilterList.Add(list[i].GetComponent<MeshFilter>());
        }
        combines = new CombineInstance[meshFilterList.Count];

        for (int i = 0; i < meshFilterList.Count; i++)
        {
            combines[i].mesh = meshFilterList[i].sharedMesh;
            combines[i].transform = meshFilterList[i].transform.localToWorldMatrix;
            //meshFilterList[i].gameObject.active = false; // 기존에 선택된 오브젝트 비활성화
        }

        


        combineMaterial = new Material[meshFilterList.Count];
        for (int i = 0; i < meshFilterList.Count; i++)
        {
            combineMaterial[i] = list[i].transform.GetComponent<MeshRenderer>().sharedMaterial;
        }
        go.transform.GetComponent<MeshFilter>().mesh = new Mesh();

        if (multiValue == "multi")
        {
            go.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combines, false); //만들어진 콤바인 인스턴트를 결합   TRUE 시 subMesh 남겨두지 않음.
            go.transform.GetComponent<MeshRenderer>().sharedMaterials = combineMaterial; // 매터리얼  여러개 적용. 

            NewGroup(multiPleAllObj);
            DelNew(multipleCombine);
            multipleCombine.Clear();
            multiPleAllObj.Clear();
        }
        else
        {
            go.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combines, true); //만들어진 콤바인 인스턴트를 결합   TRUE 시 subMesh 남겨두지 않음.
            go.transform.GetComponent<MeshRenderer>().sharedMaterial = combineMaterial[0]; //매터리얼  한개 적용. sharedMaterials = combineMaterial;

            //매쉬 파일로 저장
            Mesh myNewMesh = new Mesh();
            myNewMesh = go.GetComponent<MeshFilter>().sharedMesh;
            AssetDatabase.CreateAsset(myNewMesh, "Assets/" + name + "_Mesh" + ".asset");
        }
        go.transform.gameObject.active = true;// 신규 생성된 오브젝트 활성화.
        if(optionMulti && multiValue != "multi")
        {
            multipleCombine.Add(go);
        }
        
        if (optionNew) NewGroup(list);  // 기존 오브젝트 그룹화
        if (optionDelNew ) DelNew(list); // 기존 오브젝트 삭제
    }


    //new 옵션 ( 새로운 오브젝트 생성후 기전 오브젝트 몰아놓음)
    void NewGroup(List<GameObject> list)
    {
        GameObject newObj = new GameObject();
        newObj.name = "Old_Objects";

        foreach(var t in list)
        {
            t.transform.SetParent(newObj.transform);
        }
        newObj.SetActive(false);
    }

    //delete & new ( 기존오브젝트 지움)
   void DelNew(List<GameObject> list)
    {
        foreach(var t in list)
        {
            DestroyImmediate(t);
        }
    }


    // 에디터 윈도우가 닫힐때
    void OnDestroy()
    {
        Icon_Menu.btnBools[2] = false;
    }

}


