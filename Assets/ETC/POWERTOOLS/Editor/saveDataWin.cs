using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;//  xml 추가



public class saveDataWin : EditorWindow
{

    Texture2D blueLineTex;
    Texture2D toggleLineTex;
    Texture2D bineLineTex;
    Texture2D bineLineTex2;
    Texture2D bgTex;
    Texture2D footerBgTex;

    GUIStyle guiStyle = new GUIStyle();
    GUIStyle guiLine = new GUIStyle();


    bool saveXml;
    bool saveJson;
    string path;

    ToDoData saveToDoDats;
    jsonData saveJsonDatas;

    public saveDataWin(ToDoData todo)
    {
        saveToDoDats = todo;
    }


    void OnEnable()
    {

        blueLineTex = TEXGET("blue_bg.png");
        toggleLineTex = TEXGET("timeLine.png");
        bineLineTex = TEXGET("boundaryLine.png");
        bineLineTex2 = TEXGET("boundaryLine_02.png");
        footerBgTex = TEXGET("footer_Bg.png");
        // 수정 해야 함!!!
        bgTex = MakeTex(1, 1, new Color32(39,40,40,255));
        guiStyle.alignment = TextAnchor.MiddleLeft;
        


    }


    void OnGUI()
    {
        guiStyle.normal.background = blueLineTex;
        guiStyle.normal.textColor = new Color32(30, 30, 40, 255);
        GUI.Box(new Rect(0,0,Screen.width,25), "   Select Data Type.", guiStyle);
        guiStyle.normal.background = toggleLineTex;
        guiStyle.normal.textColor = new Color32(120, 120, 150, 255);
        GUI.Box(new Rect(0, 25, Screen.width, 24), "        XML(beta)", guiStyle);
        guiLine.normal.background = bineLineTex;
        GUI.Box(new Rect(0, 49, Screen.width, 3), "", guiLine);
        GUI.Box(new Rect(0, 52, Screen.width, 24), "        JSON(beta)", guiStyle);
        guiLine.normal.background = bineLineTex2;
        GUI.Box(new Rect(0, 76, Screen.width, 3), "", guiLine);
        guiStyle.normal.background = bgTex;
        GUI.Box(new Rect(0, 79, Screen.width, Screen.height), "", guiStyle);
        GUI.color = new Color32(120, 120, 150, 255);
        saveXml = GUI.Toggle(new Rect(10, 30, 24, 24),saveXml ,"");
        saveJson = GUI.Toggle(new Rect(10, 57, 24, 24), saveJson, "");

        GUI.color = new Color32(170, 170, 170, 255);
        if(GUI.Button(new Rect(6, 86, Screen.width-12, 20),"SAVE DATA"))
        {
            // xml 저장
            if (saveXml)
            {
                path = EditorUtility.SaveFilePanel("XML Save", "", "XML_Data", "xml");
                if (path.Length > 0)
                {

                    XmlSerializer ser = new XmlSerializer(typeof(Root)); // 직렬화
                    TextWriter textWeiter = new StreamWriter(path);

                    Root root = new Root();
                    List<xmlData> list = new List<xmlData>();

                    int num = 1;
                    for (int i = 0; i < saveToDoDats.Contents.Count; i++)
                    {
                        if (saveToDoDats.progress[i] < 99) continue;
                        xmlData xmldatas = new xmlData();
                        xmldatas.number = num.ToString();
                        xmldatas.description = saveToDoDats.Contents[i];
                        xmldatas.startTime = saveToDoDats.startTimeView[i];
                        xmldatas.completeTime = saveToDoDats.completeTimeView[i];
                        xmldatas.elapsedTime = saveToDoDats.ElapsedTime[i];
                        xmldatas.detailContents = saveToDoDats.detailContent[i];
                        list.Add(xmldatas);
                        num++;
                    }
                    root.RecordXmlData = new xmlData[list.Count];
                    root.RecordXmlData = list.ToArray();
                    ser.Serialize(textWeiter, root);
                }
            }

            // json 저장
            if (saveJson)
            {
                path = EditorUtility.SaveFilePanel("JSON Save", "", "JSON_Data", "json");
                if (path.Length > 0)
                {
                    saveJsonDatas = new jsonData();

                    int numb = 1;
                    for (int i = 0; i < saveToDoDats.Contents.Count; i++)
                    {
                        if (saveToDoDats.progress[i] < 99) continue;
                        saveJsonDatas.number.Add(numb.ToString());
                        saveJsonDatas.description.Add(saveToDoDats.Contents[i]);
                        saveJsonDatas.startTime.Add(saveToDoDats.startTimeView[i]);
                        saveJsonDatas.completeTime.Add(saveToDoDats.completeTimeView[i]);
                        saveJsonDatas.elapsedTime.Add(saveToDoDats.ElapsedTime[i]);
                        saveJsonDatas.detailContents.Add(saveToDoDats.detailContent[i]);
                        numb++;
                    }

                    string jsonDataText;
                    jsonDataText = JsonUtility.ToJson(saveJsonDatas);
                    File.WriteAllText(path, jsonDataText);
                }
            }

        }

        GUI.color = Color.white;
        guiStyle.normal.background = footerBgTex;
        GUI.Box(new Rect(0,Screen.height-40,Screen.width,30), "", guiStyle);



        //GUI.Toggle(ne)


        //saveXml = EditorGUILayout.Toggle("Save XML ",saveXml);
        //saveJson = EditorGUILayout.Toggle("Save JSON ",saveJson);


        //if(GUILayout.Button("Save Data"))
        //{

        //    // xml 저장
        //    if (saveXml)
        //    {
        //        path = EditorUtility.SaveFilePanel("XML Save", "", "XML_Data", "xml");
        //        XmlSerializer ser = new XmlSerializer(typeof(Root)); // 직렬화
        //        TextWriter textWeiter = new StreamWriter(path);


        //        Root root = new Root();
        //        List<xmlData> list = new List<xmlData>();


        //        int num = 1;
        //        for (int i = 0; i < saveToDoDats.Contents.Count; i++)
        //        {
        //            if (saveToDoDats.progress[i] < 99) continue;
        //            xmlData xmldatas = new xmlData();
        //            xmldatas.number = num.ToString();
        //            xmldatas.description = saveToDoDats.Contents[i];
        //            xmldatas.startTime = saveToDoDats.startTimeView[i];
        //            xmldatas.completeTime = saveToDoDats.completeTimeView[i];
        //            xmldatas.elapsedTime = saveToDoDats.ElapsedTime[i];
        //            xmldatas.detailContents = saveToDoDats.detailContent[i];
        //            list.Add(xmldatas);
        //            num++;
        //        }
        //        root.RecordXmlData = new xmlData[list.Count];
        //        root.RecordXmlData = list.ToArray();
        //        ser.Serialize(textWeiter, root);
        //    }

        //    // json 저장

        //    if (saveJson)
        //    {
        //        path = EditorUtility.SaveFilePanel("JSON Save", "", "JSON_Data", "json");
        //        saveJsonDatas = new jsonData();

        //        int numb = 1;
        //        for (int i =0; i < saveToDoDats.Contents.Count; i++)
        //        {
        //            if (saveToDoDats.progress[i] < 99) continue;
        //            saveJsonDatas.number.Add(numb.ToString());
        //            saveJsonDatas.description.Add(saveToDoDats.Contents[i]);
        //            saveJsonDatas.startTime.Add(saveToDoDats.startTimeView[i]);
        //            saveJsonDatas.completeTime.Add(saveToDoDats.completeTimeView[i]);
        //            saveJsonDatas.elapsedTime.Add(saveToDoDats.ElapsedTime[i]);
        //            saveJsonDatas.detailContents.Add(saveToDoDats.detailContent[i]);
        //            numb++;
        //        }

        //        string jsonDataText;
        //        jsonDataText = JsonUtility.ToJson(saveJsonDatas);
        //        File.WriteAllText(path, jsonDataText);

        //    }

        //}

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


}
