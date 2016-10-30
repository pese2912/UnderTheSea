using UnityEngine;
using System.Collections;

public class CharControl : MonoBehaviour 
{
    public static CharControl m_This = null;

    public void Start()
    {
        m_This = this;
    }
}
