﻿using UnityEngine;
using System.Collections;

public class button : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Jump"))
        {
            Application.LoadLevel("2_gameview_1");
        }
    }

   
}