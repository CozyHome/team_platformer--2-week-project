using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

[ExecuteAlways]
public class TetrahedronVoronoi : MonoBehaviour
{
    [SerializeField] private Transform a_t;
    [SerializeField] private Transform b_t;
    [SerializeField] private Transform c_t;
    [SerializeField] private Transform d_t;
    [SerializeField] private Transform o_t;

    void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;
        Draw();
    }

    void Draw() {
        if(a_t == null || b_t == null || c_t == null || d_t == null || o_t == null)
            return;

        Vector3 a = a_t.position;
        Vector3 b = b_t.position;
        Vector3 c = c_t.position;
        Vector3 d = d_t.position;
        Vector3 o = o_t.position;

        Vector3 acb = Vector3.Cross(b - a, c - a);
        Vector3 abd = Vector3.Cross(d - a, b - a);
        Vector3 acd = Vector3.Cross(c - a, d - a);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, b);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(a, c);
        Gizmos.DrawLine(a, d);

        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(a, .125F);
        Gizmos.DrawWireSphere(b, .125F);
        Gizmos.DrawWireSphere(c, .125F);
        Gizmos.DrawWireSphere(d, .125F);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(o, .125F);

        bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
        };

        if(Same(acb, o - a)) {
            Vector3 _c = VectorHeader.Barycentric2DClamped((a, b, c), o);
            Vector3 v = a * _c[0] + b * _c[1] + c * _c[2];
            Gizmos.DrawLine(v, o);

            var state = VectorHeader.Barycentric2DVoronoi((a, b, c), o);
            Debug.Log("acb: " + state);
        }
        else if(Same(abd, o - a)) {
            Vector3 _c = VectorHeader.Barycentric2DClamped((a, b, d), o);
            Vector3 v = a * _c[0] + b * _c[1] + d * _c[2];
            Gizmos.DrawLine(v, o);

            var state = VectorHeader.Barycentric2DVoronoi((a, b, d), o);
            Debug.Log("abd: " + state);
        }
        else if(Same(acd, o - a)) {
            Vector3 _c = VectorHeader.Barycentric2DClamped((a, c, d), o);
            Vector3 v = a * _c[0] + c * _c[1] + d * _c[2];
            Gizmos.DrawLine(v, o);

            var state = VectorHeader.Barycentric2DVoronoi((a, c, d), o);
            Debug.Log("acd: " + state);
        }
    }
}
