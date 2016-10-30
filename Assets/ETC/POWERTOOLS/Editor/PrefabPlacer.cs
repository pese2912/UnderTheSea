using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public class PrefabPlacer : EditorWindow
{


    Object FOLDER;
    Vector2 MAINSCROLLVIEW;
    Vector2 SCROLLVIEWPREVIEW;
    Vector3 ADDPOS;
    Vector3 POS;

    FileAttributes CHECKDIR;
    Texture2D[] PREFABTEXTURE;

    GameObject[] SELECTObject;
    

    GameObject create;

    DirectoryInfo DIRINFO;
    FileInfo[] PREFABINFO;

    List<GameObject> PREFABS = new List<GameObject>();
    List<GameObject> GAMEOBJECTNameCheck = new List<GameObject>();
    

    string folderPath;
    string[] DirpathName;
    string returnText;

    string tagName;
    string GameObjectName ="GameObject Name";


    bool OptionReplaceOn;
    bool OptionScaleOn;
    bool OptionRotationOn;
    bool OptionParentOn;
    bool OptionTagOn;
    bool OptionNameOn;
    bool OptionAddOn;

    enum OPTIONSTATE
    {
        NONE = 0,
        REPLACEON,
        PARENTON
    }
    OPTIONSTATE optionState = OPTIONSTATE.NONE;


    void OnGUI()
    {
        if (GUILayout.Button("TYPE test"))
        {
            // MESH 필터가 있는 오브젝트만 배열에 담음
            SELECTObject = Selection.gameObjects.Where(g => g.GetComponent<MeshFilter>() != null).ToArray();
            Debug.Log(SELECTObject.Length);
        }

        //OptionSelectlimit(); // 옵션 제한
        // 오브젝트 필드
        FOLDER = (Object)EditorGUILayout.ObjectField("ObjectField", FOLDER, typeof(Object), false); //booltype : scene의 오브젝트를 드래그 해서 넣을 수 있는가?

        OptionReplaceOn = EditorGUILayout.Toggle("REPLACE", OptionReplaceOn);
        if (OptionReplaceOn)
        {
            OptionParentOn = false;

            // 태그 선택.
            OptionTagOn = EditorGUILayout.Toggle("Tag select", OptionTagOn);
            if (OptionTagOn)
            {
                tagName = EditorGUILayout.TagField("select tag", tagName);
            }
            OptionNameOn = EditorGUILayout.Toggle("Name Select", OptionNameOn);
            if (OptionNameOn)
            {
                GameObjectName = EditorGUILayout.TextField(GameObjectName);
            }
        }

        // -- [ 토글모음 ] -- //

        OptionRotationOn = EditorGUILayout.Toggle("REF ROTATION", OptionRotationOn);
        OptionScaleOn = EditorGUILayout.Toggle("REF SCALE", OptionScaleOn);
        OptionAddOn = EditorGUILayout.Toggle("ADD PREFAB", OptionAddOn);
        if (OptionAddOn)
        {
            OptionReplaceOn = false;
            ADDPOS = EditorGUILayout.Vector3Field("ADD POSTION", ADDPOS);
        }

        OptionParentOn = EditorGUILayout.Toggle("PARENT", OptionParentOn);
        if (OptionParentOn) OptionReplaceOn = false;

        

        POS = EditorGUILayout.Vector3Field("POSTION", POS);


        // 프리팹 로드
        if (GUILayout.Button("LOAD PREFABS"))
        {
            if (FOLDER == null)
            {
                string title1 = "Error Meassage";
                string msg1 = "폴더를 선택해 주세요.\n Must Imported Folder!";
                EditorUtility.DisplayDialog(title1, msg1, "OK");
                return;
            }
            // 폴더인지 아닌지 판별
            folderPath = AssetDatabase.GetAssetPath(FOLDER);
            CHECKDIR = File.GetAttributes(folderPath);
            if (CHECKDIR == FileAttributes.Directory)
            {
                // 디렉터리 정보 로드
                DIRINFO = new DirectoryInfo(folderPath);
                // 디렉터리에 담긴 모든 오브젝트를 PREFABINFO[] 안에 넣어준다.
                PREFABINFO = DIRINFO.GetFiles();

                for (int i = 0; i < PREFABINFO.Length; i++)
                {
                    if (PREFABINFO[i].Name.Contains(".meta") && PREFABINFO[i].Name.Contains(".prefab"))
                    {
                        continue;
                    }
                    else if (PREFABINFO[i].Name.Contains(".prefab"))
                    {
                        PREFABS.Add((GameObject)AssetDatabase.LoadAssetAtPath(DirPathNameFactory(PREFABINFO[i].ToString()), typeof(GameObject)));
                    }
                }

                //texture2d 적용
                PREFABTEXTURE = new Texture2D[PREFABS.Count];
                for (int i = 0; i < PREFABS.Count; i++)
                {
                    PREFABTEXTURE[i] = AssetPreview.GetAssetPreview(PREFABS[i]);
                }
            }
            else
            {
                string title1 = "Error Meassage";
                string msg1 = "폴더가 아닙니다. 폴더를 선택해 주세요.\n Must Imported Folder!";
                EditorUtility.DisplayDialog(title1, msg1, "OK");
                return;
            }
            FOLDER = null;
        }
        // 제거 버튼
        if (GUILayout.Button("REMOVE ALL"))
        {
            PREFABS.Clear();
            FOLDER = null;
        }

        int columns = Mathf.FloorToInt(Screen.width / 55);
        GUI.color = new Color(0.3f, 0.3f, 0.3f);
        GUILayout.BeginHorizontal(GUI.skin.box);
        SCROLLVIEWPREVIEW = GUILayout.BeginScrollView(SCROLLVIEWPREVIEW);
        GUILayout.BeginHorizontal();
        
        //--[ BUTTONS ]--//

        // Preview 버튼 들어갈 자리.
        for (int i = 0; i < PREFABS.Count; i++)
        {
            if (i % columns == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }


            GUI.color = Color.white;
            if (GUILayout.Button(PREFABTEXTURE[i], GUILayout.Width(50), GUILayout.Height(50)))
            {
                try
                {
                    if (OptionTagOn) // 태그 선택 상태 일때
                    {
                        if (tagName == null)
                        {
                            Debug.Log("태그를 선택 해주세요.");
                            return;
                        }
                        SELECTObject = new GameObject[GameObject.FindGameObjectsWithTag(tagName).Length];
                        SELECTObject = GameObject.FindGameObjectsWithTag(tagName);
                    }
                    else if (OptionNameOn)
                    {
                        if (GameObjectName == null)
                        {
                            Debug.Log("이름을 입력 해주세요.");
                            return;
                        }
                        // where 조건
                        SELECTObject = GameObject.FindObjectsOfType<GameObject>().Where(g => g.name == GameObjectName).ToArray();
                    }
                    else
                    {
                        //SELECTObject = new GameObject[Selection.gameObjects.Length];
                        SELECTObject = Selection.gameObjects.Where(g => g.GetComponent<MeshFilter>() != null).ToArray();
                    }
                }
                catch
                {
                    SELECTObject[0] = null;
                }
                //Debug.Log(SELECTObject.Length);


                // Rplace 지우고 그 자리에 생성
                if (OptionReplaceOn)
                {
                    for (int j = 0; j < SELECTObject.Length; j++)
                    {
                        create = PrefabUtility.InstantiatePrefab(PREFABS[i]) as GameObject;
                        create.transform.position = SELECTObject[j].transform.position;
                        if (OptionRotationOn) create.transform.rotation = SELECTObject[j].transform.rotation;
                        if (OptionScaleOn) create.transform.localScale = SELECTObject[j].transform.localScale;
                        DestroyImmediate(SELECTObject[j]);
                    }
                }

                // 살려두고 포지션만 다르게 함
                if (OptionAddOn && OptionParentOn == false)
                {
                    for (int j = 0; j < SELECTObject.Length; j++)
                    {
                        create = PrefabUtility.InstantiatePrefab(PREFABS[i]) as GameObject;
                        create.transform.position = SELECTObject[j].transform.position + ADDPOS;
                        if (OptionRotationOn) create.transform.rotation = SELECTObject[j].transform.rotation;
                        if (OptionScaleOn) create.transform.localScale = SELECTObject[j].transform.localScale;
                    }
                }

                //선택한 오브젝트중 첫번째 오브젝트의 자식으로 생성 -> 수정필요 선택한 모든 모브젝트의 차일드 되도록함.
                if (OptionParentOn)
                {
                    if (SELECTObject.Length == 0)
                    {
                        string title1 = "Error Meassage";
                        string msg1 = "선택된 오브젝트가 없습니다 오브젝트를 선택 해주세요.\n Must Selected Object!";
                        EditorUtility.DisplayDialog(title1, msg1, "OK");
                        return;
                    }
                    for (int j = 0; j < SELECTObject.Length; j++)
                    {
                        create = PrefabUtility.InstantiatePrefab(PREFABS[i]) as GameObject;
                        if (OptionAddOn) create.transform.position = SELECTObject[j].transform.position + ADDPOS;
                        else create.transform.position = SELECTObject[j].transform.position;
                        create.transform.SetParent(SELECTObject[j].transform);
                        //Debug.Log(i);
                    }
                }
                // 기본생성
                if (OptionReplaceOn == false && OptionParentOn == false && OptionAddOn == false)
                {
                    create = PrefabUtility.InstantiatePrefab(PREFABS[i]) as GameObject;

                    if (POS != Vector3.zero) create.transform.position = POS;
                    else create.transform.position = Vector3.zero;

                    SceneView.RepaintAll();
                }


            }

        }

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();


    }

    //경로 수정
    string DirPathNameFactory(string pathName)
    {
        DirpathName = pathName.Split(new string[] { "Assets" }, System.StringSplitOptions.None);
        returnText = "Assets" + DirpathName[1];
        return returnText;
    }


}

