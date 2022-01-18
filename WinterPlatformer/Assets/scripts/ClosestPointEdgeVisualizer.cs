using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

[ExecuteAlways]
public class ClosestPointEdgeVisualizer : MonoBehaviour
{
    [SerializeField] Transform p_o;
    [SerializeField] Transform p_a;
    [SerializeField] Transform p_b;

    void OnDrawGizmos() {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;

        Draw();
    }

    void Draw() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(p_a.position, p_b.position);
        
        (Vector3 a, Vector3 b) query = VectorHeader.ClosestPointEdge(
            (p_a.position, p_b.position),
            p_o.position
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(query.a, query.b);
    }
}
