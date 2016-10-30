using UnityEngine;
using System.Collections;

public class DrawGizmo : MonoBehaviour {

    public static Vector3 scale;
    public static Color32 gizmoColor;

    void OnDrawGizmos()
    {


        // scale = this.transform.localScale / 4;
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, scale);
        // Gizmos.DrawIcon(transform.position, "ICON_RED_1.png", true);
    }
    
}
