using System;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

public class VoronoiRegionTetrahedron : MonoBehaviour
{
    [SerializeField] private Transform a_t;
    [SerializeField] private Transform b_t;
    [SerializeField] private Transform c_t;
    [SerializeField] private Transform d_t;
    [SerializeField] private Transform o_t;

    void OnDrawGizmos() {
        if(a_t == null || b_t == null || c_t == null || d_t == null || o_t == null)
            return;
    
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color  = Color.white;

        // Draw();
        Vector4 bary = VectorHeader.Barycentric3DClamped(
            (a_t.position, b_t.position, c_t.position, d_t.position),
            o_t.position
        );
        
        int region = VectorHeader.Barycentric3DVoronoi(
            (a_t.position, b_t.position, c_t.position, d_t.position),
            o_t.position
        );

        // Debug.Log(region);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(a_t.position, b_t.position);
        Gizmos.DrawLine(a_t.position, c_t.position);
        Gizmos.DrawLine(a_t.position, d_t.position);
        Gizmos.DrawLine(b_t.position, c_t.position);
        Gizmos.DrawLine(c_t.position, d_t.position);
        Gizmos.DrawLine(d_t.position, b_t.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            o_t.position,
            a_t.position * bary[0] +
            b_t.position * bary[1] +
            c_t.position * bary[2] +
            d_t.position * bary[3]
        );
    }
}
