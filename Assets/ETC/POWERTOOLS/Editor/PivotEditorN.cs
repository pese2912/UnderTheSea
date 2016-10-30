using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class PivotEditorN : EditorWindow
{


    Mesh PIVOTMESH;
    Bounds BOUNDS;
    Collider COL;
    Renderer REND; //월드 좌표에서 vertives의 중심점
    GameObject SELECTGAMEOBJECT;// 선택된 게임 오브젝트
    GameObject PIVOTOBJECT; // 피봇 오브젝트

    Vector3 CONTROLPOS; // 변화된 이동값
    Vector3 STARTPOS; // 초기 위치값

    Vector3[] MESHVERTICES; // vertices value
    Vector3[] STARTMESHVERTICES; // vertices value 초기값


    bool OptionSceneOnEnable;
    float pivotPosX, pivotPosY, pivotPosZ;
    string pivotName = "PivotObj";


    void OnEnable()
    {
        // scene view 렌더링
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }
    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    public void OnSceneGUI(SceneView sceneview)
    {


    }


    void OnGUI()
    {



    }







    void CenterPivot()
    {

    }

    void GetInfo()
    {


    }



}
