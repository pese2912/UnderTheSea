using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class DataMaker : EditorWindow
{
   public static PTData ptDatas;

    Texture2D texData;
    Texture2D preview;
    Font fonts;
    string description;
    Vector2 scrollview;

    GUIStyle previewStyle = new GUIStyle();

    void OnEnable()
    {
        LoadData();
    }

    
    void OnGUI()
    {

        //texName = EditorGUILayout.TextField(texName);
        fonts = (Font)EditorGUILayout.ObjectField("폰트추가", fonts, typeof(Font));
        if(GUILayout.Button("ADD Font")&& fonts !=null)
        {
            ptDatas.DBFonts.Add(fonts);
        }

        description = EditorGUILayout.TextField(description);
        texData = (Texture2D)EditorGUILayout.ObjectField("texture",texData, typeof(Texture2D));

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("BG Null", GUILayout.MaxWidth(70))) 
        {
            previewStyle.normal.background = null;
        }
        if (GUILayout.Button("add"))
        {
            ptDatas.DBTexture.Add(texData);
            ptDatas.DBdescription.Add(description);
            EditorUtility.SetDirty(ptDatas);
            texData = null;
            description = "";
        }
        GUILayout.EndHorizontal();

        // 프리뷰
        GUI.Box(new Rect(5, 145, Screen.width-10, 32),"", previewStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal(GUI.skin.box);
        scrollview = GUILayout.BeginScrollView(scrollview);

        if (ptDatas.DBTexture.Count > 0)
        {

            for(int i = 0; i <ptDatas.DBTexture.Count; i ++)
            {
                GUILayout.BeginHorizontal();
                if(GUILayout.Button(ptDatas.DBTexture[i], GUILayout.MaxWidth(40), GUILayout.MaxHeight(40)))
                {
                    previewStyle.normal.background = ptDatas.DBTexture[i];
                }
                EditorGUILayout.HelpBox("ID : "+i+ " / "+ ptDatas.DBdescription[i]+ "\n" + "Name : "+ptDatas.DBTexture[i].name + "\nSIZE  : " + ptDatas.DBTexture[i].width+" X "+ ptDatas.DBTexture[i].height, MessageType.None);
                if (GUILayout.Button("-", GUILayout.MaxWidth(20),GUILayout.MinHeight(40)))
                {
                   // ptDatas.DBTexture.RemoveAt(i);
                   // ptDatas.DBdescription.RemoveAt(i);
                   // EditorUtility.SetDirty(ptDatas);
                }
                GUILayout.EndHorizontal();
            }
            

        }
           


        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        

    


    }

    //저장된 데이터 로드
    void LoadData()
    {
        ptDatas = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
        if (ptDatas == null)
        {
            CreateData();
        }
    }

    //데이터 파일 없을시 생성
    void CreateData()
    {
        ptDatas = ScriptableObject.CreateInstance<PTData>();
        AssetDatabase.CreateAsset(ptDatas, "Assets/POWERTOOLS/Data/ptDatas.asset");
    }


    void SetTexture(Texture2D tex)
    {




    }


}
