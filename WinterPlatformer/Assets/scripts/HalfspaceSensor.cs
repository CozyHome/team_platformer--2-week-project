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

        for(int i = 0; i < count;i++) {
            Transform t = colliders[i].GetComponent<Transform>();

            if(Physics.ComputePenetration(
                colliders[i],
                t.position,
                t.rotation,
                archetype.Collider(),
                transform.position,
                transform.rotation,
                out Vector3 normal,
                out float distance
            )) {
                
                Vector3 clos = archetype.ClosestPoint(transform.position - normal * 100);
                Debug.DrawRay(clos + normal * distance, normal, Color.red);

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