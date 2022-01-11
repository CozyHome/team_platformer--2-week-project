using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

enum SimplexLineClosestFeature {
    AB,
    A
};

public class SimplexLineVisualizer : MonoBehaviour
{
    [SerializeField] Transform   point_b;
    [SerializeField] Transform   point_a;
    [SerializeField] Transform   point_o;
    [SerializeField] Color      normal_c;
    [SerializeField] Color      search_c;
    [SerializeField] Color       inner_c;
    [SerializeField] Color       outer_c;
    [SerializeField] Color        next_c;

    [SerializeField] private SimplexLineClosestFeature closest_feature;

    void OnDrawGizmos() {
        if(point_b == null || point_a == null || point_o == null)
           return;
        else {
            Gizmos.matrix = Matrix4x4.identity;
            Draw();
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

    void Draw() {
        Vector3 b = point_b.position;
        Vector3 a = point_a.position;
        Vector3 o = point_o.position;

        Vector3 ab = b - a;
        Vector3 ao = o - a;

        Gizmos.color = inner_c;
        Gizmos.DrawLine(b, a);
        Gizmos.DrawWireSphere(b, 1F);
        Gizmos.DrawWireSphere(a, 1F);
        
        Gizmos.color = outer_c;
        Gizmos.DrawLine(a, a - ab);
        Gizmos.DrawLine(b, b + ab);

        Gizmos.color = next_c;
        Gizmos.DrawLine(o, a);
        Gizmos.DrawWireSphere(o, 1F);

        Gizmos.color = search_c;
        if(VectorHeader.Dot(ao, ab) > 0) {
            closest_feature = SimplexLineClosestFeature.AB;
        
            Vector3 next = Vector3.Cross(Vector3.Cross(ab, ao), ab);
            next.Normalize();
            Gizmos.DrawLine((a + b) / 2, (a + b) / 2 + next * 10);
        }
        else {
            closest_feature = SimplexLineClosestFeature.A;
            Gizmos.DrawLine(a, a + ao * 10);
        }
    } 
}
