using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class ToDoData : ScriptableObject
{

    public List<DataPopUpWindow> popWindow = new List<DataPopUpWindow>();

    public List<bool> completeSwitch = new List<bool>();
    public List<bool> startTimeSwitch = new List<bool>();

    public List<string> Contents = new List<string>();
    public List<float> progress = new List<float>();

    public List<string> startTime = new List<string>(); // 시작시간 계산 데이터
    public List<string> startTimeView = new List<string>(); // 시작시간 VIEW 데이터.

    public List<string> completeTime = new List<string>(); // 완료 시간
    public List<string> completeTimeView = new List<string>(); // 완료 시간 VIEW 데이터.

    public List<string> ElapsedTime = new List<string>(); // 경과된 View 데이터

    public List<string> currentTime = new List<string>(); // 경과된 시간

    public List<string> detailContent = new List<string>(); // 상세내용

    //public List<> checkList = new List<>();//checkList 

}
