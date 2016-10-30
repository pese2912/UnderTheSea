using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class test : EditorWindow
{
    List<GameObject> point = new List<GameObject>();
    Vector3 centerPoint;

    List<Vector3> pointStartValue = new List<Vector3>();
    //Vector3[] pointStartValue;

    void OnGUI()
    {

        if (GUILayout.Button("move") && Selection.gameObjects != null)
        {
            centerPoint = GameObject.Find("center").transform.position; //  선택된 오브젝트를 담습니다
            point = Selection.gameObjects.ToList();


            for (int i = 0; i < point.Count; i++)
            {
                pointStartValue.Add(point[i].transform.position);

            }

            for (int i = 0; i < point.Count; i++)
            {
                Undo.RecordObject(point[i].transform, point[i].name);
                point[i].transform.position = pointStartValue[i] + centerPoint;
            }
            


        }

    }

}
