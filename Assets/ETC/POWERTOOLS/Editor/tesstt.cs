using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class tesstt : EditorWindow
{

    List<GameObject> seleceted = new List<GameObject>();
    int row;
    int col;
    float x;
    float z;
    Vector3 newPos;
    Vector3 sPos;
    string startTime;
    string currentTime;
    string calTime;
    bool timecalcu;
    bool enable;

    GameObject select;




    void OnGUI()
    {
        if (GUILayout.Button("INPUT"))
        {

            Input();
        }

        if (GUILayout.Button("CHECK"))
        {

            Check();


        }

      

    }

    int[] a;
    int b = 5;    

    void Input()
    {
        a = new int[19];
        for (int i=0; i < 20; i++)
        {
            a[0] = i;
        }
    }

    void Check()
    {
       //foreach(var v in t)
       // {
            
       // }
       //     // Debug.Log("B와 같은 수가 있습니다.");
       // else Debug.Log("B와 같은 수가 없습니다.");
    }


    void TimeCal()
    {
        if (!timecalcu) return;
        currentTime = System.DateTime.Now.ToString("h:mm:ss.ff");
        System.DateTime start = System.Convert.ToDateTime(startTime);
        System.DateTime current = System.Convert.ToDateTime(currentTime);
        System.TimeSpan timeCal = current - start;
        calTime = timeCal.ToString();
        GUILayout.Label(calTime.Substring(0, calTime.Length - 5));
    }

    void Memo()
    {
        
    }
}
