using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

enum ClosestTriFeature {
    NULL,
    A,
    B,
    C,
    AB,
    BC,
    CA,
    ENCLOSED
};

public class ClosestPointTriangleVisualizer : MonoBehaviour
{
    [SerializeField] Transform point_a;
    [SerializeField] Transform point_b;
    [SerializeField] Transform point_c;
    [SerializeField] Transform point_o;
    [SerializeField] ClosestTriFeature closest_feature;

    void OnDrawGizmos() {
        if(point_a == null || point_b == null || point_c == null)
            return;
        
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;
        Vector3 a = point_a.position;
        Vector3 b = point_b.position;
        Vector3 c = point_c.position;
        Vector3 o = point_o.position;        

        (Vector3 a, Vector3 b) query = VectorHeader.ClosestPointTriangle(
            (a, b, c),
            o
        );
        
        Vector3 bary = VectorHeader.Barycentric2DClamped((a, b, c), o);
        // returns bitstring detailing which region we are in
        int s = VectorHeader.Barycentric2DVoronoi((a, b, c), o);
        Debug.Log(s);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, a);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(query.a, query.b);

        // Draw();
    }   
    
    void Draw() {
        
        Vector3 a = point_a.position;
        Vector3 b = point_b.position;
        Vector3 c = point_c.position;
        Vector3 o = point_o.position;

        Vector3 ab = b - a;
        Vector3 bc = c - b;
        Vector3 ca = a - c;

        Vector3 ao = o - a;
        Vector3 bo = o - b;
        Vector3 co = o - c;

        Vector3 abc = Vector3.Cross(ab, bc);
        Vector3 ab_n = Vector3.Cross(ab, abc);
        Vector3 bc_n = Vector3.Cross(bc, abc);
        Vector3 ca_n = Vector3.Cross(ca, abc);

        Gizmos.color = Color.red;
        // segments
        Gizmos.DrawLine(a,b);
        Gizmos.DrawLine(b,c);
        Gizmos.DrawLine(c,a);

        // norms
        Gizmos.color = Color.white;
        Gizmos.DrawRay((a + b) / 2, ab_n);
        Gizmos.DrawRay((b + c) / 2, bc_n);
        Gizmos.DrawRay((c + a) / 2, ca_n);

        bool Same(Vector3 v1, Vector3 v2) {
            return VectorHeader.Dot(v1, v2) > 0;
        };

        // branching approach:
        if(Same(ao, ab_n)) {
            if(!Same(ao, ab)) {
                closest_feature = ClosestTriFeature.A;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(o, a);
                return;
            }
            else {
                if(Same(bo, ab)) {
                    closest_feature = ClosestTriFeature.B;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(o, b);
                    return;
                }
                else {
                    closest_feature = ClosestTriFeature.AB;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(o, a + VectorHeader.ProjectVector(ao, ab.normalized));
                    return;
                }
            }
        }
        else if(Same(bo, bc_n)) {
            if(!Same(bo, bc)) {
                closest_feature = ClosestTriFeature.B;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(o, b);
                return; 
            } 
            else {
                if(!Same(co, bc)) {
                    closest_feature = ClosestTriFeature.AB;
                    Gizmos.color = Color.green;
                    // Gizmos.DrawLine(o, VectorHeader.ClosestPointOnPlane(o, b, bc_n.normalized));
                    Gizmos.DrawLine(o, b + VectorHeader.ProjectVector(bo, bc.normalized));
                    return;
                }
                else {
                    closest_feature = ClosestTriFeature.C;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(o, c);
                    return;    
                }
            }
        }
        else if(Same(co, ca_n)) {
            if(!Same(co, ca)) {
                closest_feature = ClosestTriFeature.C;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(o, c);
                return;
            }
            else {
                if(Same(ao, ca)) {
                    closest_feature = ClosestTriFeature.A;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(o, a);
                }
                else {
                    closest_feature = ClosestTriFeature.CA;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(o, c + VectorHeader.ProjectVector(co, ca.normalized));
                    // Gizmos.DrawLine(o, VectorHeader.ClosestPointOnPlane(o, c, ca_n.normalized));
                }
            }
        }
        else {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(o, a);
            Gizmos.DrawLine(o, b);
            Gizmos.DrawLine(o, c);
            return;
        }

        // Direct, case by case approach:

        // vertex regions
        // if(Same(ao, ca) && !Same(ao, ab)) {
        //     closest_feature = ClosestTriFeature.A;
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawLine(o, a);
        //     return;
        // }
        // else if(Same(bo, ab) && !Same(bo, bc)) {
        //     closest_feature = ClosestTriFeature.B;
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawLine(o, b);
        //     return;
        // }
        // else if(Same(co, bc) && !Same(co, ca)) {
        //     closest_feature = ClosestTriFeature.C;
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawLine(o, c);
        //     return;
        // }
        
        // // edge regions
        // if(Same(ao, ab) && Same(ao, ab_n)) {
        //     closest_feature = ClosestTriFeature.AB;
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawLine(o, a + VectorHeader.ProjectVector(ao, ab.normalized));
        //     return;
        // }
        // else if(Same(bo, bc) && Same(bo, bc_n)) {
        //     closest_feature = ClosestTriFeature.BC;
        //     Gizmos.color = Color.green;
        //     // Gizmos.DrawLine(o, VectorHeader.ClosestPointOnPlane(o, b, bc_n.normalized));
        //     Gizmos.DrawLine(o, b + VectorHeader.ProjectVector(bo, bc.normalized));
        //     return;
        // }
        // else if(Same(co, ca) && Same(co, ca_n)) {
        //     closest_feature = ClosestTriFeature.CA;
        //     Gizmos.color = Color.green;
        //     // Gizmos.DrawLine(o, VectorHeader.ClosestPointOnPlane(o, c, ca_n.normalized));
        //     Gizmos.DrawLine(o, c + VectorHeader.ProjectVector(co, ca.normalized));
        //     return;
        // }
        // else {
        //     closest_feature = ClosestTriFeature.ENCLOSED;
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawLine(o, a);
        //     Gizmos.DrawLine(o, b);
        //     Gizmos.DrawLine(o, c);
        //     return;
        // }
    }
}