using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToDoSubCheckList : ScriptableObject
{

    public List<string> subCheckList = new List<string>();
    public List<bool> checkComplete = new List<bool>();
    public List<bool> listEditMode = new List<bool>();
	
}
