using UnityEngine;

public class MPRTester : MonoBehaviour {
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
            MPR.stopat1 = stopat;

            // Run GJK
            bool answer = MPR.BooleanMPR(
                new ConvexPolyhedron(
                    PolyhedronA.GetComponent<MeshCollider>().sharedMesh.vertices,
                    PolyhedronA.transform.localToWorldMatrix
                ),
                new ConvexPolyhedron(
                    PolyhedronB.GetComponent<MeshCollider>().sharedMesh.vertices,
                    PolyhedronB.transform.localToWorldMatrix
                )
            );
            // Debug.Log("Distance: " + answer);

            isColliding = answer;
            Gizmos.color = isColliding ? Color.red : Color.green;
            Gizmos.matrix = PolyhedronA.transform.localToWorldMatrix;
            Gizmos.DrawWireMesh(
                PolyhedronA.GetComponent<MeshCollider>().sharedMesh,
                0,
                Vector3.zero,
                Quaternion.identity,
                Vector3.one
            );

            Gizmos.color = isColliding ? Color.red : Color.blue;
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