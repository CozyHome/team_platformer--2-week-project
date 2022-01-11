using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

[ExecuteAlways]
public class ConvexGizmoTransform : MonoBehaviour
{
    [SerializeField] private Vector2[] local_points;
    [SerializeField] private Color color;

    public Vector2 GetVertex(int i) => transform.localToWorldMatrix.MultiplyPoint(local_points[i]);

    public Vector2 Support(Vector2 d) {
        float max = Mathf.NegativeInfinity;
        Vector2 m_v = Vector2.zero;
        for(int i = 0;i < local_points.Length;i++) {
            float dot = VectorHeader.Dot(GetVertex(i), d);
            if(dot > max) {
                max = dot;
                m_v = GetVertex(i);
            }
        }

        return m_v;
    }

    void OnDrawGizmos() {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color  = color;
        for(int i = 0;i < local_points.Length;i++) {
            Vector2 p1 = transform.localToWorldMatrix.MultiplyPoint(local_points[i]);
            Vector2 p2 = transform.localToWorldMatrix.MultiplyPoint(local_points[(i + 1) % local_points.Length]);
            Gizmos.DrawLine(p1, p2);
        }
    
        Gizmos.matrix = Matrix4x4.identity;
    }
}
