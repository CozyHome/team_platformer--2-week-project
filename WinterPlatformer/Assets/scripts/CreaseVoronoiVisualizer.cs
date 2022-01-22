using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

public class CreaseVoronoiVisualizer : MonoBehaviour {
    [SerializeField] private Transform a_t;
    [SerializeField] private Transform b_t;
    [SerializeField] private Transform c_t;
    [SerializeField] private Transform o_t;

    void OnDrawGizmos() {
        if(a_t == null || b_t == null || c_t == null || o_t == null)
            return;
        
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;
        
        Draw();
    }

    void Draw() {
        (Vector3 a, Vector3 b, Vector3 c) tri = (a_t.position, b_t.position, c_t.position);
        Vector3 o = o_t.position;

        Vector3 ao = o - tri.a;
        Vector3 bo = o - tri.b;
        Vector3 co = o - tri.c;

        Gizmos.color = Color.black;
        Gizmos.DrawLine(tri.a, tri.b);
        Gizmos.DrawLine(tri.b, tri.c);

        Vector3 ab = tri.b - tri.a;
        Vector3 bc = tri.c - tri.b;


        Vector3 abc = Vector3.Cross(ab, bc);
        float area = abc.magnitude;
        abc /= area;

        Vector3 ab_n = Vector3.Cross(ab, abc);
        Vector3 bc_n = Vector3.Cross(bc, abc);
        ab_n.Normalize();
        bc_n.Normalize();

        bool Same(Vector3 v1, Vector3 v2) {
            return VectorHeader.Dot(v1, v2) > 0;
        };
        
        // Gizmos.DrawRay(tri.a, ab_n);
        // Gizmos.DrawRay(tri.b, bc_n);

        if(Same(ab_n, bo) && Same(bc_n, bo)) {
            if(Same(bo, ab) && !Same(bo, bc)) { // region b
                Gizmos.DrawLine(tri.b, o);
                return;
            }
            else if(!Same(bo, ab)) { // Voronoi regions of AB
                if(!Same(ao, ab)) { // region A
                    Gizmos.DrawLine(tri.a, o);
                    return;
                }
                else { // region AB
                    Gizmos.DrawLine(o ,o - VectorHeader.ProjectVector(o - tri.a, ab_n));
                    return;
                }
            }
            else if(Same(bo, bc)) { // Voronoi regions of BC
                if(Same(co, bc)) {
                    Gizmos.DrawLine(tri.c, o);
                    return;
                }
                else {
                    Gizmos.DrawLine(o, o - VectorHeader.ProjectVector(o - tri.c, bc_n));
                    return;
                }
            }
        }

        if(Same(ab_n, bo)) { // Voronoi regions of AB
            Gizmos.DrawLine(VectorHeader.ClosestPointEdge((tri.b, tri.a), o).b, o);
            return;
        }
        else if(Same(bc_n, bo)) { // Voronoi regions of BC
            Gizmos.DrawLine(VectorHeader.ClosestPointEdge((tri.b, tri.c), o).b, o);
            return;
        }
    }
}
