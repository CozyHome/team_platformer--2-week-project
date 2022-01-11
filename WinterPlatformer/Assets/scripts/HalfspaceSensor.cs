using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using com.cozyhome.Actors;
using com.cozyhome.Archetype;
using com.cozyhome.Vectors;

[RequireComponent(typeof(SphereCollider))]
public class HalfspaceSensor : MonoBehaviour {

    private LayerMask filter;
    private Collider[] colliders;
    private ArchetypeHeader.SphereArchetype archetype;

    private List<(Vector3 p, Vector3 n)> overlaps;

    void Start() {
        overlaps = new List<(Vector3 p, Vector3 n)>();

        filter |= 1;
        colliders = new Collider[20];
        archetype = new ArchetypeHeader.SphereArchetype(
            GetComponent<SphereCollider>()
        );
    }

    void FixedUpdate() {

        var r = GetComponent<SphereCollider>().radius;

        overlaps.Clear();

        archetype.Overlap(
            transform.position,
            transform.rotation,
            filter,
            0F,
            QueryTriggerInteraction.Ignore,
            colliders,
            out int count
        );

        ArchetypeHeader.OverlapFilters.FilterSelf(
            ref count,
            archetype.Collider(),
            colliders
        );

        var hits = new RaycastHit[10];

        for(int i = 0; i < count;i++) {
            Transform t = colliders[i].GetComponent<Transform>();

            if(Physics.ComputePenetration(
                archetype.Collider(),
                transform.position,
                transform.rotation,
                colliders[i],
                t.position,
                t.rotation,
                out Vector3 normal,
                out float distance
            )) {
                
                Vector3 clos = archetype.ClosestPoint(transform.position - normal * 100);
                float tlen = (clos - transform.position).magnitude;
                //Debug.DrawRay(transform.position, clos - transform.position, Color.green);

                int cast = ArchetypeHeader.TraceRay(
                    transform.position,
                    -normal,
                    r,
                    hits,
                    filter
                );

                ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(
                    ref cast,
                    out int i0,
                    0F,
                    archetype.Collider(),
                    hits
                );

                // if(i0 >= 0)
                //     Debug.Log(hits[i0].distance + " " + (0.5F - distance));

                if(i0 >= 0 && r - hits[i0].distance < r - distance)
                    Debug.DrawRay(hits[i0].point, hits[i0].normal, Color.red);


                // Debug.DrawRay(
                //     colliders[i].ClosestPoint(transform.position),
                //     transform.position - colliders[i].ClosestPoint(transform.position),
                //     Color.red
                // );
                // Debug.DrawRay(transform.position -, normal);
            }
        }
    }

}