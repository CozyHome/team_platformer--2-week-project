using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

[ExecuteAlways]
public class CreaseInterpolant : MonoBehaviour
{
    [SerializeField] public Transform p1_transform; 
    [SerializeField] public Transform p2_transform; 


    void OnDrawGizmos()
    {

        Vector3 s1 = p1_transform.localScale;
        Vector3 p1 = p1_transform.position;
        Quaternion r1 = p1_transform.rotation;
        DrawPlane(s1, p1, r1, Color.red);
    
        Vector3 s2 = p2_transform.localScale;
        Vector3 p2 = p2_transform.position;
        Quaternion r2 = p2_transform.rotation;
        DrawPlane(s2, p2, r2, Color.green);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color  = Color.white;

        Vector3 cross = Vector3.Cross(r1 * Vector3.forward, r2 * Vector3.forward);
        Vector3 v = p2 - p1;

        Vector3 tan = Vector3.Cross(cross, r1 * Vector3.forward).normalized;
        float toi = VectorHeader.LinePlaneIntersection((p1, tan), (p2, r2 * Vector3.forward));
        Vector3 proj = tan * toi;
        Gizmos.DrawRay(p1, proj);
        Gizmos.DrawRay(p1 + proj, cross * 5F);
    }

    private static void DrawPlane(Vector3 s1, Vector3 p1, Quaternion r1, Color c)
    {
        Gizmos.color = c;

        Matrix4x4 m = Matrix4x4.Translate(p1);
        m *= Matrix4x4.Rotate(r1);
        Gizmos.matrix = m;

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(s1[0], s1[1], 0.0F));
    }
}