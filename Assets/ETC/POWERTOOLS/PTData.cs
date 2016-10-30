using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class PTData : ScriptableObject
{

    public Dictionary<string, Texture2D> DataTexture2D = new Dictionary<string, Texture2D>();
    public List<Texture2D> DBTexture = new List<Texture2D>();
    public List<string> DBdescription = new List<string>();
    public List<Font> DBFonts = new List<Font>();

}
