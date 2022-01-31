using System;
using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

public class MPR {
    public static bool Same(Vector3 v1, Vector3 v2) {
        return VectorHeader.Dot(v1, v2) > 0;
    }
    
    public static int stopat = 0;
    public static int stopat1 = 0;
    public static bool BooleanMPR(in ConvexPolyhedron a, in ConvexPolyhedron b)
    {
        GJKSimplex splx = new GJKSimplex();

        // MPR pseudo-code

        // Phase 1: Portal Discovery
        // Subalgorithm #1: Find Origin Ray & Find Candidate Portal 
        var v = a.Origin - b.Origin;
        do {
            var d = v;
            var A = a.Support(d);
            var B = b.Support(-d);
            var V = new MinkowskiVertex(A, B, A - B);
            if(!Same(d, V.v))
                return false; // we aren't intersecting
            else {
                splx.Add(V);
                MutateCandidateSimplex(ref splx, ref v);
            }
        } while(splx.Count < 4);

        // Subalgorithm #2: Choose New Candidate
        // while(origin ray does not intersect candidate)
            // choose_new_candidate()
        RefineCandidateSimplex(in a, in b, ref splx);

        // Phase 2: Portal Refinement
        // while(true)
            // if (origin inside portal) return hit
            // find_support_in_direction_of_portal()
            // if(origin outside support plane) return miss
            // if(support plane close to portal) return miss
            // choose_new_portal()
        return RefinePortalSimplex(in a, in b, ref splx);
    }

    private static bool RefinePortalSimplex(in ConvexPolyhedron a, in ConvexPolyhedron b, ref GJKSimplex splx) {
        const bool HIT = true;
        const bool MISS = false;

        int iterat = 100;
        do {
            Vector3 v123 = Vector3.Cross(splx[3].v - splx[1].v, splx[2].v - splx[1].v);
            var V4 = Support(in a, in b, v123);

            Vector3 v014 = Vector3.Cross(V4.v - splx[0].v, splx[1].v - splx[0].v);
            Vector3 v024 = Vector3.Cross(V4.v - splx[0].v, splx[2].v - splx[0].v);
            Vector3 v034 = Vector3.Cross(splx[3].v - splx[0].v, V4.v - splx[0].v);
            
            if(iterat == stopat1) {
                DrawSimplex(splx);
                // Debug.Log("OK");
                Gizmos.color = Color.white;
                // Gizmos.DrawLine(splx[3].v, V4.v);
                // Gizmos.DrawLine(splx[2].v, V4.v);
                // Gizmos.DrawLine(splx[1].v, V4.v);

                // Gizmos.color = Color.magenta;
                // Gizmos.DrawRay(splx[1].v, v123);
                // Gizmos.DrawRay(splx[2].v, v123);
                // Gizmos.DrawRay(splx[3].v, v123);

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay( // V0, V1, V4
                    (splx[0].v + splx[1].v + V4.v) / 3,
                    v014
                );
                Gizmos.DrawLine(splx[0].v, splx[1].v);
                Gizmos.DrawLine(splx[1].v, V4.v);
                Gizmos.DrawLine(V4.v, splx[0].v);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(splx[0].v, splx[2].v);
                Gizmos.DrawLine(splx[2].v, V4.v);
                Gizmos.DrawLine(V4.v, splx[0].v);
                Gizmos.DrawRay( // V0, V2, V4
                    (V4.v + splx[2].v + splx[0].v) / 3,
                    v024
                );

                Gizmos.color = Color.red;
                Gizmos.DrawRay( // V0, V3, V4
                    (splx[0].v + splx[3].v + V4.v) / 3,
                    v034
                );
                Gizmos.DrawLine(splx[0].v, splx[3].v);
                Gizmos.DrawLine(splx[3].v, V4.v);
                Gizmos.DrawLine(V4.v, splx[0].v);

                Gizmos.color = Color.white;
            }


            // if origin inside portal
            if(Same(splx[1].v, v123)) {
                for(int i = 0;i < splx.Count;i++) {
                    Gizmos.DrawWireSphere(splx[i].a, 0.125F);
                    Gizmos.DrawWireSphere(splx[i].b, 0.125F);
                }

                return HIT;
            }
            else if(VectorHeader.Dot(splx[1].v, v123) >= -1e-3)
                return MISS;

            // find_support_in_direction_of_portal()


            // if origin outside support plane
            // CONTEXT:
            // we've maximized as much as possible along v123, The origin is actually now behind V4.v.
            // In this circumstance, its safe to assume we aren't colliding as we've traveled as far as we
            // can in via the portal's normal.
            // if(VectorHeader.Dot(V4.v - splx[1].v, v123) <= 1e-3) {
            //     // stopat1 = iterat;
            //     Debug.Log("RCP " + iterat);

            //     return HIT;
            // }
            
            if(Same(splx[0].v, v014)) {
                if(Same(splx[0].v, v024)) {
                    // we must remove v1
                    splx[1] = V4;
                }
                else {
                    // we must remove v3
                    splx[3] = V4;
                }
            }
            else {
                if(Same(splx[0].v, v034)) {
                    // elim v2
                    splx[2] = V4;
                }
                else {
                    // elim v1
                    splx[1] = V4;
                }
            }

            // choose_new_portal()
            // here we split the tetrahedron into three plane checks:

            // if support plane close to portal: return miss

            // break;

        } while(true && iterat-- > 0);

        // stopat1 = iterat;
        return MISS;
    }

    private static void RefineCandidateSimplex(in ConvexPolyhedron a, in ConvexPolyhedron b, ref GJKSimplex splx) {
        int iterat = 100;
        do {

            // if(iterat == MPR.stopat) {
            //     // DrawSimplexPlanes(in splx);
            // }

            // V0, V1, V2
            Vector3 v012 = Vector3.Cross(splx[1].v - splx[0].v, splx[2].v - splx[0].v);
            if (Same(-splx[0].v, v012)) {
                
                var V3 = Support(in a, in b, v012);
                splx[3] = V3;

                // make sure normal of affine space is facing origin, we know that 
                // it isn't via this check above ^
                // a.k.a, triple product of v012 and v0 is negative
                // swap verts
                var v1 = splx[1];
                splx[1] = splx[2];
                splx[2] = v1;
                continue;
            }

            Vector3 v013 = Vector3.Cross(splx[3].v - splx[0].v, splx[1].v - splx[0].v);

            // V0, V1, V3
            if(Same(-splx[0].v, v013)) {
                var V2 = Support(in a, in b, v013);
                splx[2] = V2;

                var v1  = splx[1];
                splx[1] = splx[2];
                splx[2] = v1;
                continue;
            }

            // V0, V2, V3
            Vector3 v023 = Vector3.Cross(splx[2].v - splx[0].v, splx[3].v - splx[0].v);
            if(Same(-splx[0].v, v023)) {
                var V2  = Support(in a, in b, v023);
                splx[2] = V2;

                var v1  = splx[1];
                splx[1] = splx[2];
                splx[2] = v1;
                continue;
            }

            break;
        } while(iterat-- > 0);

        // DrawSimplex(in splx);
        Debug.Log("RCS " + iterat);
        
        MPR.stopat = iterat;
    }

    private static void DrawSimplexPlanes(in GJKSimplex splx)
    {
        Gizmos.color = Color.cyan; // V0, V1, V2
        Gizmos.DrawRay(splx[0].v, Vector3.Cross(splx[1].v - splx[0].v, splx[2].v - splx[0].v));
        Gizmos.DrawRay(splx[1].v, Vector3.Cross(splx[1].v - splx[0].v, splx[2].v - splx[0].v));
        Gizmos.DrawRay(splx[2].v, Vector3.Cross(splx[1].v - splx[0].v, splx[2].v - splx[0].v));
        Gizmos.color = Color.red; // V0, V1, V3
        Gizmos.DrawRay(splx[0].v, Vector3.Cross(splx[3].v - splx[0].v, splx[1].v - splx[0].v));
        Gizmos.DrawRay(splx[1].v, Vector3.Cross(splx[3].v - splx[0].v, splx[1].v - splx[0].v));
        Gizmos.DrawRay(splx[3].v, Vector3.Cross(splx[3].v - splx[0].v, splx[1].v - splx[0].v));
        Gizmos.color = Color.yellow; // V0, V2, V3
        Gizmos.DrawRay(splx[0].v, Vector3.Cross(splx[2].v - splx[0].v, splx[3].v - splx[0].v));
        Gizmos.DrawRay(splx[2].v, Vector3.Cross(splx[2].v - splx[0].v, splx[3].v - splx[0].v));
        Gizmos.DrawRay(splx[3].v, Vector3.Cross(splx[2].v - splx[0].v, splx[3].v - splx[0].v));
        Gizmos.color = Color.white;
    }

    private static void MutateCandidateSimplex(ref GJKSimplex splx, ref Vector3 v)
    {
        switch(splx.Count) {
            case 1: // 0D affine hull
                v = -splx[0].v;
            break;
            case 2: // 1D affine hull
                v = Vector3.Cross(splx[1].v, splx[0].v);
            break;
            case 3: // 2D affine hull
                v = Vector3.Cross(splx[2].v - splx[0].v, splx[1].v - splx[0].v);

                // if triple product is positive, normal is not facing origin
                if(Same(splx[0].v, v)) {
                    // swap vertices and negate v
                    var v1  = splx[1];
                    splx[1] = splx[2];
                    splx[2] = v1;
                    v       = -v;
                }
            break;
        }
    }

    private static void DrawSimplex(in GJKSimplex splx)
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Vector3.zero, 0.125F);

        Gizmos.color = Color.cyan; // V0
        Gizmos.DrawWireSphere(splx[0].v, .125F);
        Gizmos.color = Color.red; // V1
        Gizmos.DrawWireSphere(splx[1].v, .125F);
        Gizmos.color = Color.green; // V2 
        Gizmos.DrawWireSphere(splx[2].v, .125F);
        Gizmos.color = Color.yellow; // V3
        Gizmos.DrawWireSphere(splx[3].v, .125F);
        
        Gizmos.color = Color.white;

        Gizmos.DrawLine(splx[3].v, splx[0].v);
        Gizmos.DrawLine(splx[3].v, splx[1].v);
        Gizmos.DrawLine(splx[3].v, splx[2].v);
        
        Gizmos.DrawLine(splx[0].v, splx[1].v);
        Gizmos.DrawLine(splx[1].v, splx[2].v);
        Gizmos.DrawLine(splx[2].v, splx[0].v);

        Gizmos.DrawLine(splx[0].v, Vector3.zero);
    }

    public static MinkowskiVertex Support(
            in ConvexPolyhedron a,
            in ConvexPolyhedron b,
            Vector3 D) {
                Vector3 _point_a = a.Support(D);
                Vector3 _point_b = b.Support(-D);
            return new MinkowskiVertex(_point_a, _point_b, _point_a - _point_b); 

    }
}