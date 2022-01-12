using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GJKTester : MonoBehaviour
{
    [SerializeField] private GameObject PolyhedronA;
    [SerializeField] private GameObject PolyhedronB;

    [SerializeField] private bool isColliding = false;

    [SerializeField] [Range(0,10)]private int stopat = 0;

    void Start() {

    }

    void OnDrawGizmos() {
        if(PolyhedronA == null || PolyhedronB == null)
            return;
        else {
            BooleanGJK.stopat = stopat;

            // Run GJK
            bool answer = BooleanGJK.GJK(
                new BooleanGJK.ConvexPolyhedron(
                    PolyhedronA.GetComponent<MeshCollider>().sharedMesh.vertices,
                    PolyhedronA.transform.localToWorldMatrix
                ),
                new BooleanGJK.ConvexPolyhedron(
                    PolyhedronB.GetComponent<MeshCollider>().sharedMesh.vertices,
                    PolyhedronB.transform.localToWorldMatrix
                )
            );

            isColliding = answer;
            Gizmos.color = answer ? Color.red : Color.green;
            Gizmos.matrix = PolyhedronA.transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(
                PolyhedronA.GetComponent<MeshCollider>().sharedMesh,
                0,
                Vector3.zero,
                Quaternion.identity,
                Vector3.one
            );

            Gizmos.color = answer ? Color.red : Color.blue;
            Gizmos.matrix = PolyhedronB.transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(
                PolyhedronB.GetComponent<MeshCollider>().sharedMesh,
                0,
                Vector3.zero,
                Quaternion.identity,
                Vector3.one
            );
        }

    }
}
