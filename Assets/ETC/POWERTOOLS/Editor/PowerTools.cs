using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PowerTools : EditorWindow
{


  

    Vector2 SCROLLVIEW;
    Dictionary<string, GameObject> DICSELECTOBJECT = new Dictionary<string, GameObject>();

    [MenuItem("NEXTI/PowerTools")]
    static void ToolBarWindow()
    {
        PowerTools window = (PowerTools)EditorWindow.GetWindow(typeof(PowerTools));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Multi Apply Prefabs"))
        {
            MultiPrefabs multiPrefabs = (MultiPrefabs)EditorWindow.GetWindow(typeof(MultiPrefabs));
            multiPrefabs.Show();
        }
        if (GUILayout.Button("Prefab Generator"))
        {
            PrefabPlacer multiPrefabsWindow = (PrefabPlacer)EditorWindow.GetWindow(typeof(PrefabPlacer));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("Grouping"))
        {
            Grouping multiPrefabsWindow = (Grouping)EditorWindow.GetWindow(typeof(Grouping));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("Renaming"))
        {
            Renaming multiPrefabsWindow = (Renaming)EditorWindow.GetWindow(typeof(Renaming));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("Transfer"))
        {
            Transfer transfer = (Transfer)EditorWindow.GetWindow(typeof(Transfer));
            transfer.Show();
        }
        if (GUILayout.Button("Solo"))
        {
            Solo multiPrefabsWindow = (Solo)EditorWindow.GetWindow(typeof(Solo));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("Mesh Combine"))
        {
            Combiner multiPrefabsWindow = (Combiner)EditorWindow.GetWindow(typeof(Combiner));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("Pivot Editor"))
        {
            PivotEditor multiPrefabsWindow = (PivotEditor)EditorWindow.GetWindow(typeof(PivotEditor));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("Mesh Editor"))
        {
            MeshEditor multiPrefabsWindow = (MeshEditor)EditorWindow.GetWindow(typeof(MeshEditor));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("Object Align"))
        {
            AlignObject multiPrefabsWindow = (AlignObject)EditorWindow.GetWindow(typeof(AlignObject));
            multiPrefabsWindow.Show();
        }
        if (GUILayout.Button("To Do LIST"))
        {
            ToDoList todoList = (ToDoList)EditorWindow.GetWindow(typeof(ToDoList));
            todoList.minSize = new Vector2(515f, 184f);
            todoList.Show();
        }
        if (GUILayout.Button("Rrnamer"))
        {
            Renamer renamer = (Renamer)EditorWindow.GetWindow(typeof(Renamer));
            renamer.Show();
        }
        if (GUILayout.Button("Data Maker"))
        {
            DataMaker dataMaker = (DataMaker)EditorWindow.GetWindow(typeof(DataMaker));
            dataMaker.Show();
        }
        if (GUILayout.Button("protoyype"))
        {
            prototypeObjects prototypeObjects = (prototypeObjects)EditorWindow.GetWindow(typeof(prototypeObjects));
            prototypeObjects.Show();
        }
        //Arkanoid
        if (GUILayout.Button("Arkanoid"))
        {
            Arkanoid Arkanoid = (Arkanoid)EditorWindow.GetWindow(typeof(Arkanoid));
            Arkanoid.Show();
        }

        //tesstt
        if (GUILayout.Button("test"))
        {
            tesstt TESTTT = (tesstt)EditorWindow.GetWindow(typeof(tesstt));
            TESTTT.Show();
        }
    }
}

