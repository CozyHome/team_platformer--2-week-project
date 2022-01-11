using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.cozyhome.Actors;
using com.cozyhome.Archetype;
using com.cozyhome.Vectors;

public class WallRunner : MonoBehaviour, ActorHeader.IActorReceiver
{
    
    CapsuleActor actor;
    Vector3 aux_dir = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        actor = GetComponent<CapsuleActor>();
        aux_dir = actor.orientation * Vector3.forward;
        actor.velocity = aux_dir;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        actor.SetPosition(transform.position);
        actor.SetOrientation(transform.rotation);
        //FSM.Current.Tick(Time.fixedDeltaTime);
        Move(Time.fixedDeltaTime);
        ActorHeader.Move(this, actor, Time.fixedDeltaTime);

        transform.SetPositionAndRotation(actor.position, actor.orientation);
    }

    void Move(float fdt) {
        Quaternion quat = actor.orientation;
        var arc = actor.GetArchetype();
        arc.Trace(
            actor.position,
            aux_dir,
            .1F,
            quat,
            actor.Mask,
            0F,
            QueryTriggerInteraction.Ignore,
            actor.Hits,
            out int count
        );

        ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(
            ref count, 
            out int i0,
            0F,
            arc.Collider(),
            actor.Hits
        );

        if(i0 >= 0) {
            aux_dir = -Vector3.Cross(Vector3.Cross(aux_dir, actor.Hits[i0].normal), actor.Hits[i0].normal);
            aux_dir.Normalize();

            //actor.orientation = Quaternion.LookRotation(aux_dir, Vector3.up);
            actor.velocity = aux_dir;
            Debug.Log(actor.Hits[i0].collider.name);

            Debug.Log(i0);
        }
        else {
            arc.Trace(
            actor.position,
            Vector3.Cross(aux_dir, Vector3.up).normalized,
            1F,
            quat,
            actor.Mask,
            0F,
            QueryTriggerInteraction.Ignore,
            actor.Hits,
            out count
        );
        
        if(count > 0) {
            ArchetypeHeader.TraceFilters.FindClosestFilterInvalids(
                ref count,
                out int i1,
                0F,
                arc.Collider(),
                actor.Hits
            );

            if(i1 >= 0) {
                aux_dir = -Vector3.Cross(Vector3.Cross(aux_dir, actor.Hits[i1].normal), actor.Hits[i1].normal);
                aux_dir.Normalize();
                //actor.orientation = Quaternion.LookRotation(aux_dir, Vector3.up);
                actor.velocity = aux_dir;
            }
        }
        }
    }

    public void OnGroundHit(ActorHeader.GroundHit ground, ActorHeader.GroundHit lastground, LayerMask layermask)
    { }

    public void OnTraceHit(ActorHeader.TraceHitType tracetype, RaycastHit trace, Vector3 position, Vector3 velocity)
    {
        // if(tracetype == ActorHeader.TraceHitType.Trace) {
        //     aux_dir = -trace.normal;
        // }
    }

    public void OnTriggerHit(ActorHeader.TriggerHitType triggertype, Collider trigger)
    { }
}
