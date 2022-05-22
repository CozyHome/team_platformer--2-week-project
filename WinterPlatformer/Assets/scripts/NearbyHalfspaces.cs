using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.cozyhome.Archetype;

public class NearbyHalfspaces : MonoBehaviour
{
    private ArchetypeHeader.SphereArchetype sphere;
    private MeshCollider convex_mesh;
    
    private Collider[] colliders;

    void Start() {
        colliders = new Collider[10];

        sphere = new ArchetypeHeader.SphereArchetype(GetComponent<SphereCollider>());
        convex_mesh = GetComponent<MeshCollider>();
    }

    void OnDrawGizmos() {

        if(sphere == null || convex_mesh == null || colliders == null)
            return;

        sphere.Overlap(
            transform.position,
            transform.rotation,
            (1 << 0),
            0F,
            QueryTriggerInteraction.Ignore,
            colliders,
            out int i0
        );

        ArchetypeHeader.OverlapFilters.FilterSelf(
            ref i0,
            sphere.Collider(),
            colliders
        );

        for(int i =0;i<i0;i++) {
            if(colliders[i] is MeshCollider) {
                Debug.Log("OK");
                Mesh wallmesh = ((MeshCollider)colliders[i]).sharedMesh;
                Mesh playermesh = convex_mesh.GetComponent<MeshCollider>().sharedMesh;
                DistanceGJK.GJK(
                    new ConvexPolyhedron(playermesh, transform.localToWorldMatrix),
                    new ConvexPolyhedron(wallmesh, colliders[i].transform.localToWorldMatrix)
                );

                Gizmos.color = Color.red;

                Gizmos.DrawWireMesh(
                    playermesh,
                    0,
                    transform.position,
                    transform.rotation,
                    transform.localScale
                );

                Gizmos.color = Color.cyan;

                Gizmos.DrawWireMesh(
                    wallmesh,
                    0,
                    colliders[i].transform.position,
                    colliders[i].transform.rotation,
                    colliders[i].transform.localScale
                );
            }
        }
    }
}
