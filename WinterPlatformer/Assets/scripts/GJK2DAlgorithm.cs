using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

[ExecuteAlways]
public class GJK2DAlgorithm : MonoBehaviour
{
    [SerializeField] Transform init_v;
    [SerializeField] ConvexGizmoTransform c0;
    [SerializeField] ConvexGizmoTransform c1;

    void OnDrawGizmos() {
        //Debug.Log(GJK(c0, c1));
    }
}
