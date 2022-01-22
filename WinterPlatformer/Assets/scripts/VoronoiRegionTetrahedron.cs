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

        Draw();
    }

    void Draw() {
        
        bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
        };

        Vector3 a = a_t.position;
        Vector3 b = b_t.position;
        Vector3 c = c_t.position;
        Vector3 d = d_t.position;
        Vector3 o = o_t.position;

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(a, c);
        Gizmos.DrawLine(a, d);

        Gizmos.DrawLine(d, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);

        // Gizmos.color = Color.cyan;
        // Gizmos.DrawRay(a, (a - b).normalized);
        // Gizmos.DrawRay(a, (a - c).normalized);
        // Gizmos.DrawRay(a, (a - d).normalized);
        
        Gizmos.color = Color.red;
        
        Vector3 adb = Vector3.Cross(d - a, b - d);
        Vector3 acd = Vector3.Cross(c - a, d - c);
        Vector3 abc = Vector3.Cross(b - a, c - b);
        Vector3 cbd = Vector3.Cross(b - c, d - b);
        adb.Normalize();
        acd.Normalize();
        abc.Normalize();
        cbd.Normalize();

        // Gizmos.DrawRay(a, adb);
        // Gizmos.DrawRay(a, acd);
        // Gizmos.DrawLine(o,
        // o - VectorHeader.ProjectVector(o - a, adb));
        // Gizmos.DrawLine(o,
        // o - VectorHeader.ProjectVector(o - a, acd));
        // Gizmos.DrawLine(o,
        // o - VectorHeader.ProjectVector(o - a, abc));
        // Gizmos.DrawLine(o,
        // o - VectorHeader.ProjectVector(o - a, cbd));

        // Gizmos.DrawRay((c + b + a) / 3, abc);
        // Gizmos.DrawRay((d + c + a) / 3, acd);
        // Gizmos.DrawRay((b + d + a) / 3, adb);
        // Gizmos.DrawRay((c + b + d) / 3, cbd);

        void TripleEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, Vector3 p) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(tet.x, (tet.x - tet.y).normalized);
            Gizmos.DrawRay(tet.x, (tet.x - tet.z).normalized);
            Gizmos.DrawRay(tet.x, (tet.x - tet.w).normalized);

            bool check1 = Same(o - tet.x, tet.x - tet.y);
            bool check2 = Same(o - tet.x, tet.x - tet.z);
            bool check3 = Same(o - tet.x, tet.x - tet.w);
            Debug.Log(check1 + " " + check2 + " " + check3);
        }

        void SingleEdges((Vector3 x, Vector3 y, Vector3 z) tet, Vector3 p) {
            Gizmos.color = Color.yellow;
            Vector3 xyz = Vector3.Cross(tet.y - tet.x, tet.z - tet.y);
            xyz.Normalize();
            xyz *= .5F;
            Gizmos.DrawRay(tet.x, xyz);
            Gizmos.DrawRay(tet.y, xyz);
            Gizmos.DrawRay(tet.z, xyz);
            Gizmos.DrawLine(
                o,
                VectorHeader.ClosestPointTriangle((tet.x, tet.y, tet.z), o).b
            );
        }

        void DualEdges((Vector3 x, Vector3 y, Vector3 z, Vector3 w) tet, Vector3 p) {
            Gizmos.color = Color.cyan;
            Vector3 zxy = Vector3.Cross(tet.x - tet.z, tet.y - tet.x);
            Vector3 xwy = Vector3.Cross(tet.w - tet.x, tet.y - tet.w);
            zxy.Normalize();
            xwy.Normalize();
            Gizmos.DrawRay(tet.y, xwy);
            Gizmos.DrawRay(tet.x, xwy);
            Gizmos.DrawRay(tet.y, zxy);
            Gizmos.DrawRay(tet.x, zxy);
            Gizmos.color = Color.red;
            Vector3 xwy_n = Vector3.Cross(Vector3.Cross(zxy, xwy), xwy);
            Vector3 zxy_n = Vector3.Cross(zxy, Vector3.Cross(zxy, xwy));
            
            Gizmos.DrawRay(tet.x, xwy_n);
            Gizmos.DrawRay(tet.x, zxy_n);

            Vector3 xo = o - tet.x; 
            // see which side of crease we exist in
            if(!Same(xo, xwy_n) && !Same(xo, zxy_n)) {
                Gizmos.DrawLine(o, VectorHeader.ClosestPointEdge((tet.x, tet.y), o).b);
            }
            else if(Same(xo, zxy_n)) {
                Gizmos.DrawLine(o, VectorHeader.ClosestPointTriangle((tet.z, tet.x, tet.y), o).b);
            }
            else if(Same(xo, xwy_n)) {
                Gizmos.DrawLine(o, VectorHeader.ClosestPointTriangle((tet.x, tet.w, tet.y), o).b);
            }

            // Gizmos.DrawRay(tet.x, tet.x - tet.w);
            // Gizmos.DrawRay(tet.y, tet.y - tet.w);
            // Gizmos.DrawRay(tet.x, tet.x - tet.z);
            // Gizmos.DrawRay(tet.y, tet.y - tet.z);
        }

        int ComputeSignBits(Vector3 p) {
            int nflags = 0;
            nflags |= VectorHeader.Dot(o - b, adb) > 0 ? (1 << 0) : 0;
            nflags |= VectorHeader.Dot(o - d, acd) > 0 ? (1 << 1) : 0;
            nflags |= VectorHeader.Dot(o - c, abc) > 0 ? (1 << 2) : 0;
            nflags |= VectorHeader.Dot(o - d, cbd) > 0 ? (1 << 3) : 0;
            return nflags;
        }

        int v = ComputeSignBits(o);
        Debug.Log(v);
        switch (v) {
            case 0: // INNER
            break;
            case 1:  // ADB             (SINGULAR)
                SingleEdges((a, d, b), o);
            break;
            case 2:  // ACD             (SINGULAR)
                SingleEdges((a, c, d), o);
            break;
            case 3:  // ADB & ACD       (DUAL)
                DualEdges((a, d, b, c), o);
            break;
            case 4:  // ABC             (SINGULAR)
                SingleEdges((a, b, c), o);
            break;
            case 5:  // ABC & ADB (     DUAL)
                DualEdges((a, b, c, d), o);
            break;
            case 6:  // ACD & ABC       (DUAL)
                DualEdges((a, c, d, b), o);
            break;
            case 7:  // ADB & ACD & ABC (TRIPLE)
                TripleEdges((a, b, c, d), o);
            break;
            case 8:  // CBD             (SINGULAR)
                SingleEdges((c, b, d), o);
            break;
            case 9:  // ADB & CBD       (DUAL)
                DualEdges((b, d, c, a), o);
            break;
            case 10: // ACD & CBD       (DUAL)
                DualEdges((c, d, a, b), o);
            break;
            case 11: // ADB & ACD & CBD (TRIPLE)
                TripleEdges((d, b, c, a), o);
            break;
            case 12: // ABC & CBD       (DUAL)
                DualEdges((b, c, a, d), o);
            break;
            case 13: // ADB & ABC & CBD (TRIPLE)
                TripleEdges((b, a, c, d), o);
            break;
            case 14: // ACD & ABC & CBD (TRIPLE)
                TripleEdges((c, b, a, d), o);
            break;
            default:
            break;
        }

        // Debug.Log(Convert.ToString(ComputeSignBits(o), 2));

        // Gizmos.DrawLine(
        //     a,
        //     a + VectorHeader.ProjectVector(o - a, (a - b).normalized)
        // );
        // Gizmos.DrawLine(
        //     a,
        //     a + VectorHeader.ProjectVector(o - a, (a - c).normalized)
        // );
        // Gizmos.DrawLine(
        //     a,
        //     a + VectorHeader.ProjectVector(o - a, (a - d).normalized)
        // );

        // bool check1 = VectorHeader.Dot(o - a, (a - b).normalized) > 0;
        // bool check2 = VectorHeader.Dot(o - a, (a - c).normalized) > 0;
        // bool check3 = VectorHeader.Dot(o - a, (a - d).normalized) > 0;
        // if(check1 && check2 && check3)
        //     Gizmos.DrawLine(a, o);
        // Debug.Log(check1 + " " + check2 + " " + check3);
    }
}
