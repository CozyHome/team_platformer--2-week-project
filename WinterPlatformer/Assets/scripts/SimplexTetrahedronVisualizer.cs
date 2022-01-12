using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

enum SimplexTetrahedronClosestFeature {
    ABC,
    ADB,
    ACD,
    AB,
    AC,
    AD,
    A,
    ENCLOSED
};

public class SimplexTetrahedronVisualizer : MonoBehaviour
{
    [SerializeField] Transform   point_d;
    [SerializeField] Transform   point_c;
    [SerializeField] Transform   point_b;
    [SerializeField] Transform   point_a;
    [SerializeField] Transform   point_o;
    [SerializeField] Color      normal_c;
    [SerializeField] Color      search_c;
    [SerializeField] Color       inner_c;
    [SerializeField] Color       outer_c;
    [SerializeField] Color        next_c;

    [SerializeField] private SimplexTetrahedronClosestFeature closest_feature;

    void OnDrawGizmos() {
        if(point_d == null || point_c == null || point_b == null || point_a == null || point_o == null)
           return;
        else {
            Gizmos.matrix = Matrix4x4.identity;
            Draw();
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

    void Draw() {
        Vector3 d = point_d.position;
        Vector3 c = point_c.position;
        Vector3 b = point_b.position;
        Vector3 a = point_a.position;
        Vector3 o = point_o.position;

        Vector3 ac = c - a;
        Vector3 ab = b - a;
        Vector3 ad = d - a;
        Vector3 ao = o - a;

        Gizmos.color = inner_c;
        // edges
        Gizmos.DrawLine(d, b);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(b, c);
        
        Gizmos.DrawLine(d, a);
        Gizmos.DrawLine(c, a);
        Gizmos.DrawLine(b, a);

        // centroids        
        Gizmos.color = outer_c;
        Vector3 abc = Vector3.Cross(ab, ac);
        Vector3 centroid_abc = (a + b + c) / 3;
        
        Gizmos.DrawLine(a, centroid_abc);
        Gizmos.DrawLine(b, centroid_abc);
        Gizmos.DrawLine(c, centroid_abc);
        
        Vector3 acd = Vector3.Cross(ac, ad);
        Vector3 centroid_acd = (a + c + d) / 3;
        
        Gizmos.DrawLine(a, centroid_acd);
        Gizmos.DrawLine(c, centroid_acd);
        Gizmos.DrawLine(d, centroid_acd);
        
        Vector3 adb = Vector3.Cross(ad, ab);
        Vector3 centroid_adb = (a + b + d) / 3;
        
        Gizmos.DrawLine(a, centroid_adb);
        Gizmos.DrawLine(d, centroid_adb);
        Gizmos.DrawLine(b, centroid_adb);

        Gizmos.color = normal_c;

        // normals
        abc.Normalize();
        acd.Normalize();
        adb.Normalize();
        Gizmos.DrawLine(centroid_abc, centroid_abc + abc);
        Gizmos.DrawLine(centroid_acd, centroid_acd + acd);
        Gizmos.DrawLine(centroid_adb, centroid_adb + adb);

        Gizmos.color = next_c;
        Gizmos.DrawLine(a, o);
        
        // actual algorithm:
        bool Same(Vector3 v1, Vector3 v2) {
            return VectorHeader.Dot(v1, v2) > 0;
        }
        Gizmos.color = search_c;
    
        // check plane abc first
        if(Same(abc, ao)) {
            // if second plane found as well
            if(Same(acd, ao)) {
                if(Same(adb, ao)) { // all three planes implies a
                    closest_feature = SimplexTetrahedronClosestFeature.A;
                    Gizmos.DrawLine(a, o);
                }
                else { // abc and acd but not adb implies edge 
                    closest_feature = SimplexTetrahedronClosestFeature.AC;
                    Gizmos.DrawLine((a + c) / 2, o);
                }
            }else {
                if(Same(adb, ao)) { // abc and adb but not acd
                    closest_feature = SimplexTetrahedronClosestFeature.AB;
                    Gizmos.DrawLine((a + b) / 2, o);
                }
                else { // abc is only plane
                    closest_feature = SimplexTetrahedronClosestFeature.ABC;
                    Gizmos.DrawLine(centroid_abc, o);
                }
            }
        }else {
            if(Same(acd, ao)) { // not abc but acd
                if(Same(adb, ao)) { // only acd and abd
                    closest_feature = SimplexTetrahedronClosestFeature.AD;
                    Gizmos.DrawLine((a + d) / 2, o);
                }
                else { // only acd
                    closest_feature = SimplexTetrahedronClosestFeature.ACD;
                    Gizmos.DrawLine(centroid_acd, o);
                }
            }else {
                if(Same(adb, ao)) { // only abd
                    closest_feature = SimplexTetrahedronClosestFeature.ADB;
                    Gizmos.DrawLine(centroid_adb, o);
                }else {
                    closest_feature = SimplexTetrahedronClosestFeature.ENCLOSED;
                    Gizmos.DrawLine(centroid_adb, o);
                    Gizmos.DrawLine(centroid_acd, o);
                    Gizmos.DrawLine(centroid_abc, o);
                }
            }
        }
    } 
}
