using System;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

[ExecuteAlways]
public class PointPlaneConvergence : MonoBehaviour
{
    [SerializeField] float amt;
    [SerializeField] private Transform point_t0;
    [SerializeField] private Transform point_t1;

    [SerializeField] private Transform normal_plane;

    void OnDrawGizmos() {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(point_t0.position, point_t1.position);

        Gizmos.DrawRay(normal_plane.position, normal_plane.forward);
        Gizmos.color = Color.green;
        Matrix4x4 m = Matrix4x4.Translate(normal_plane.position);
        m *= Matrix4x4.Rotate(normal_plane.rotation);
        Gizmos.matrix = m;
        Gizmos.DrawWireCube(Vector3.zero, normal_plane.localScale);
        
        Gizmos.matrix = Matrix4x4.identity;
        FOR_staticplane_Converge(
            (normal_plane.rotation * Vector3.forward, VectorHeader.Dot(normal_plane.position, normal_plane.rotation * Vector3.forward)),
            (point_t0.position, point_t1.position - point_t0.position));
    }

    void FOR_staticplane_Converge((Vector3 n, float w) pln, (Vector3 p, Vector3 r) pnt) {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pnt.p, pnt.p + pnt.r);

        Vector3 p2 = pnt.p + amt * pnt.r; 
        Gizmos.DrawLine(pnt.p, p2);

        Gizmos.DrawLine(p2, p2 - pln.n * (VectorHeader.Dot(p2, pln.n) - pln.w));
    }
}
