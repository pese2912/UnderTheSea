using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Enum = System.Enum;
//using System.Reflection;

public class Renaming : EditorWindow
{
    /*
    [MenuItem("NEXTI/PowerTools/Renaming")]
    static void ToolBarWindow()
    {
        Renaming window = (Renaming)EditorWindow.GetWindow(typeof(Renaming));
        window.Show();
    }
    */

    // testapplicationbot000@gmail.com  쀼빠님 이메일 (CODE REVIEW)
    static List<Object> LIVESELECTOBJECT = new List<Object>();
    List<Object> SELECTOBJECT = new List<Object>();
    List<GameObject> SORTOBJECT = new List<GameObject>();
    Dictionary<string, bool> OPTIONS = new Dictionary<string, bool>();

    Vector2 SCROLLVIEW;
    Vector2 STATESCROLLVIEW;
    Vector2 MAINCROLLVIEW;

    float loadingBarCount; // 로딩바 카운트
    int pNum = 2, nNum = 1, number = 1;
    int numLetter;
    string selectChr = "Selcet Charater";
    string changeName = "Change Name";
    string addLetter = "Add Letter";
    string firstLetter;
    static string liveFirstLetter;

    enum OPTIONSTATE
    {
        NONE = 0,
        FIRSTUPPER,
        FIRSTLOWER,
        ALLUPPER,
        ALLLOWER,
        NUMBERING,
        REPLACE
    }
    OPTIONSTATE optionState = OPTIONSTATE.NONE;

    void OnEnable()
    {
        foreach (var t in Enum.GetValues(typeof(OPTIONSTATE)))
        {
            OPTIONS[t.ToString()] = t.ToString() is bool;
        }
    }

    //-- [ GUI ZONE ] -----------------------------------------//
    void OnGUI()
    {
        MAINCROLLVIEW = GUILayout.BeginScrollView(MAINCROLLVIEW);

        OPTIONS["FIRSTUPPER"] = EditorGUILayout.Toggle("First Letter ToUpper", OPTIONS["FIRSTUPPER"]);
        OPTIONS["FIRSTLOWER"] = EditorGUILayout.Toggle("First Letter ToLower", OPTIONS["FIRSTLOWER"]);
        OPTIONS["ALLUPPER"] = EditorGUILayout.Toggle("All ToUpper", OPTIONS["ALLUPPER"]);
        OPTIONS["ALLLOWER"] = EditorGUILayout.Toggle("All ToLower", OPTIONS["ALLLOWER"]);
        OPTIONS["NUMBERING"] = EditorGUILayout.Toggle("Add Numbering", OPTIONS["NUMBERING"]);
        OPTIONS["REPLACE"] = EditorGUILayout.Toggle("Replace Letter", OPTIONS["REPLACE"]);

        if (OPTIONS["FIRSTUPPER"]) OptionCheck("FIRSTUPPER");
        if (OPTIONS["FIRSTLOWER"]) OptionCheck("FIRSTLOWER");
        if (OPTIONS["ALLUPPER"]) OptionCheck("ALLUPPER");
        if (OPTIONS["ALLLOWER"]) OptionCheck("ALLLOWER");
        if (OPTIONS["NUMBERING"]) { OptionCheck("NUMBERING"); addLetter = EditorGUILayout.TextField(addLetter); }
        if (OPTIONS["REPLACE"]) { OptionCheck("REPLACE"); selectChr = EditorGUILayout.TextField(selectChr); }

        // Load Object List
        if (GUILayout.Button("LOAD SELECTED OBJECTS"))
        {
            if (SELECTOBJECT != null) SELECTOBJECT.Clear();
            SELECTOBJECT = Selection.objects.Where(g => AssetDatabase.Contains(g)).ToList();
            SORTOBJECT = Selection.gameObjects.Where(g => !AssetDatabase.Contains(g)).OrderBy(g => g.transform.GetSiblingIndex()).ToList();

            for (int i = 0; i < SORTOBJECT.Count; i++)
            {
                SELECTOBJECT.Add(SORTOBJECT[i]);
            }
        }

        GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.MinHeight(150.0f));
        SCROLLVIEW = GUILayout.BeginScrollView(SCROLLVIEW);
        if (SELECTOBJECT != null || SORTOBJECT != null)
        {
            foreach (var t in SELECTOBJECT.ToList())
            {
                if (t == null)
                {
                    SELECTOBJECT.Clear(); // 사용자가 임의로 지웠을때 고려
                    
                    return;
                }
                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(t.name, MessageType.None);
                if (GUILayout.Button(" - ", GUILayout.MaxWidth(20.0f))) SELECTOBJECT.Remove(t); // 해당 오브젝트만 삭제
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        changeName = EditorGUILayout.TextField(changeName);

        if (GUILayout.Button("TEST"))
        {
            SORTOBJECT = Selection.gameObjects.ToList();
            
            SORTOBJECT[0].name = "";

        }
            if (GUILayout.Button("Delete Unity Numbering"))
        {
            foreach (var t in SELECTOBJECT.ToList())
            {
                if (t == null)
                {
                    SELECTOBJECT.Clear();
                    return;
                }
                // int.TryParse ans
                if ((t.name.Substring(t.name.Length - 1) == ")") && (int.TryParse(t.name.Substring(t.name.Length - 2,1) , out numLetter) ) )
                {
                    Debug.Log(t.name.Substring(t.name.Length - 2, 1));
                   
                    for (int i = 0; i < t.name.Length; i++)
                    {
                        if(int.TryParse(t.name.Substring(t.name.Length - pNum, nNum), out numLetter))
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

        //ReNaming
        if (GUILayout.Button("Renaming"))
        {
            if (SELECTOBJECT == null) return;
            number = 1;

            foreach (var t in SELECTOBJECT)
            {

                if (AssetDatabase.Contains(t)) { }
                else Undo.RecordObject(t, t.name);

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

                    case OPTIONSTATE.NUMBERING:
                        DataBaseGetTypeToAll(t, number + addLetter + t.name);
                        number++;
                        break;

                    case OPTIONSTATE.REPLACE:
                        DataBaseGetTypeToAll(t, t.name.Replace(selectChr, changeName));
                        break;

                    default:
                        DataBaseGetTypeToAll(t, changeName);
                        break;
                }
            }
            AssetDatabase.Refresh();
        }

        //Remove ALL
        if (GUILayout.Button("Remove All"))
        {
            if (SELECTOBJECT != null)
            {
                loadingBarCount = 0;
                SELECTOBJECT.Clear();
            }
        }
        GUILayout.EndScrollView();
    }

    //-- [ MESTHOD ZONE ] -----------------------------------------//
    // 사용자가 선택한 Object 가 Hireachy 혹은, Project 상인지 판단에 따른 명령 실행
    void DataBaseGetTypeToAll(Object obj, string name)
    {
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
    }//
    

    //Mouse Light Click Menu Mesthod
    static string LiveFirstLetterChange(string text, string option)
    {
        liveFirstLetter = option == "upper" ? liveFirstLetter = text.Substring(0, 1).ToUpper() : liveFirstLetter = text.Substring(0, 1).ToLower();
        text = text.Substring(1, text.Length - 1);
        text = liveFirstLetter + text;
        return text;
    }

    void OptionCheck(string optionName)
    {
        if (optionName == optionState.ToString()) return;
        optionState = (OPTIONSTATE)Enum.Parse(typeof(OPTIONSTATE), optionName);
        foreach (var t in OPTIONS.Keys.ToList())
        {
            if (t == optionName) continue;
            else OPTIONS[t] = false;
        }
    }

    //--[ Hierarchy Mouse Light Click Menu ]--
    [MenuItem("GameObject/NEXTI/First Letter ToUpper", false, 0)]
    static void FirestLetterUpper() { LiveFirestLetter("upper"); }
    [MenuItem("GameObject/NEXTI/First Letter ToLower", false, 1)]
    static void FirestLetterLower() { LiveFirestLetter("lower"); }

    //--[ Project Mouse Light Click Menu ]--
    [MenuItem("Assets/NEXTI/First Letter ToUpper", false, 0)]
    static void FirestLetterUpperAsset() { LiveFirestLetter("upper"); }
    [MenuItem("Assets/NEXTI/First Letter ToLower", false, 1)]
    static void FirestLetterLowerAsset() { LiveFirestLetter("lower"); }

    static void LiveFirestLetter(string type)
    {
        if (Selection.objects[0] == null) return;
        foreach (var t in Selection.objects)
        {
            Undo.RecordObject(t, t.name);
            if (AssetDatabase.Contains(t)) AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(t), LiveFirstLetterChange(t.name, type));
            else t.name = LiveFirstLetterChange(t.name, type);
        }
    }
}
