using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

enum SimplexTriangleClosestFeature {
    NEG_ABC,
    ABC,
    AC,
    AB,
    A
};

public class SimplexTriangleVisualizer : MonoBehaviour
{
    [SerializeField] Transform   point_c;
    [SerializeField] Transform   point_b;
    [SerializeField] Transform   point_a;
    [SerializeField] Transform   point_o;
    [SerializeField] Color      normal_c;
    [SerializeField] Color      search_c;
    [SerializeField] Color       inner_c;
    [SerializeField] Color       outer_c;
    [SerializeField] Color        next_c;

    [SerializeField] private SimplexTriangleClosestFeature closest_feature;

    void OnDrawGizmos() {
        if(point_c == null || point_b == null || point_a == null || point_o == null)
           return;
        else {
            Gizmos.matrix = Matrix4x4.identity;
            Draw();
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

    void Draw() {
        Vector3 c = point_c.position;
        Vector3 b = point_b.position;
        Vector3 a = point_a.position;
        Vector3 o = point_o.position;

        Vector3 ac = c - a;
        Vector3 bc = c - b;
        Vector3 ab = b - a;
        Vector3 ao = o - a;

        Vector3 abc = Vector3.Cross(ab, bc);
        Vector3 abc_ab = Vector3.Cross(ab, abc);
        Vector3 abc_ac = Vector3.Cross(abc, ac);
        abc_ac.Normalize();
        abc_ab.Normalize();
        abc.Normalize();
        Gizmos.color = normal_c;
        Gizmos.DrawRay((a + b + c) / 3, abc);
        Gizmos.DrawRay((a + b) / 2, abc_ab);
        Gizmos.DrawRay((a + c) / 2, abc_ac);

        Gizmos.color = inner_c;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(c, a);
        Gizmos.DrawWireSphere(a, .5F);
        Gizmos.DrawWireSphere(b, .5F);
        Gizmos.DrawWireSphere(c, .5F);
        Gizmos.color = outer_c;
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(a, a - ac * 10F);
        Gizmos.DrawLine(a, a - ab * 10F);
        Gizmos.DrawLine(b, a + ab * 10F);
        Gizmos.DrawLine(a, a + Vector3.Cross(abc, ab) * 10F);
        Gizmos.DrawLine(a, a - Vector3.Cross(abc, ab) * 10F);
        Gizmos.DrawLine(a, a + Vector3.Cross(abc, ac) * 10F);
        Gizmos.DrawLine(a, a - Vector3.Cross(abc, ac) * 10F);

        Gizmos.color = next_c;
        Gizmos.DrawLine(a, o);
        Gizmos.DrawWireSphere(o, .5F);

        // actual algorithm:
        bool Same(Vector3 v1, Vector3 v2) {
            return VectorHeader.Dot(v1, v2) > 0;
        }

        Gizmos.color = search_c;

        if(Same(abc_ac, ao)) {
            if(Same(ac, ao)) {
                closest_feature = SimplexTriangleClosestFeature.AC;
                Gizmos.DrawLine((a + c) / 2, (a + c) / 2 + Vector3.Cross(Vector3.Cross(ac, ao), ac));
            }
            else {
                if(Same(ab, ao)) {
                    closest_feature = SimplexTriangleClosestFeature.AB;
                    Gizmos.DrawLine((a + b) / 2, (a + b) / 2 + Vector3.Cross(Vector3.Cross(ab, ao), ab));
                }
                else {
                    closest_feature = SimplexTriangleClosestFeature.A;
                    Gizmos.DrawLine(a, a + ao * 10F);
                }
            }
        } else {
            if(Same(abc_ab, ao)) {
                if(Same(ab, ao)) {
                    closest_feature = SimplexTriangleClosestFeature.AB;
                    Gizmos.DrawLine((a + b) / 2, (a + b) / 2 + Vector3.Cross(Vector3.Cross(ab, ao), ab));
                }
                else {
                    closest_feature = SimplexTriangleClosestFeature.A;
                    Gizmos.DrawLine(a, a + ao * 10F);
                }
            } else {
                if(Same(abc, ao)) {
                    closest_feature = SimplexTriangleClosestFeature.ABC;
                    Gizmos.DrawLine((a + b + c) / 3, (a + b + c) / 3 + abc * 10F);
                }
                else {
                    closest_feature = SimplexTriangleClosestFeature.NEG_ABC;
                    Gizmos.DrawLine((a + b + c) / 3, (a + b + c) / 3 - abc * 10F);
                }
            }
        }
    } 
}
