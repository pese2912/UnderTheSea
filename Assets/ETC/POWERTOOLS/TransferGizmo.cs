using UnityEngine;
using System.Collections;

public class TransferGizmo : MonoBehaviour {

    PTData DB;

    void OnDrawGizmos()
    {

        Gizmos.DrawIcon(transform.position, "8Direction_Color", true);
    }
}
