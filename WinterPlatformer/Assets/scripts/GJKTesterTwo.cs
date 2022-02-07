using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJKTesterTwo : MonoBehaviour
{
    [SerializeField] private GameObject PolyhedronA;
    [SerializeField] private GameObject PolyhedronB;

    [SerializeField] private bool isColliding = false;

    [SerializeField] [Range(0,1000)]private int stopat = 0;

    void Start() {

    }

    void OnDrawGizmos() {
        if(PolyhedronA == null || PolyhedronB == null)
            return;
        else {
            DistanceGJK.stopat = stopat;

            // Run GJK
            float answer = DistanceGJK.GJK(
                new ConvexPolyhedron(
                    PolyhedronA.GetComponent<MeshCollider>().sharedMesh,
                    PolyhedronA.transform.localToWorldMatrix
                ),
                new ConvexPolyhedron(
                    PolyhedronB.GetComponent<MeshCollider>().sharedMesh,
                    PolyhedronB.transform.localToWorldMatrix
                )
            );
            // Debug.Log("Distance: " + answer);

            isColliding = answer < Vector3.kEpsilon;
            Gizmos.color = isColliding ? Color.red : Color.green;
            Gizmos.matrix = PolyhedronA.transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(
                PolyhedronA.GetComponent<MeshFilter>().sharedMesh,
                0,
                Vector3.zero,
                Quaternion.identity,
                Vector3.one
            );

            Gizmos.color = isColliding ? Color.red : Color.blue;
            Gizmos.matrix = PolyhedronB.transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(
                PolyhedronB.GetComponent<MeshFilter>().sharedMesh,
                0,
                Vector3.zero,
                Quaternion.identity,
                Vector3.one
            );
        }

    }
}
