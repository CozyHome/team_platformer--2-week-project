using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

public class BooleanGJK {

    public struct ConvexPolyhedron {
        private Vector3[] points;
        private Matrix4x4 m;

        public ConvexPolyhedron(Vector3[] points, Matrix4x4 t_m) {
            this.points = points;
            this.m = t_m;
        }

        private Vector3 GetVertex(int i) => m.MultiplyPoint(points[i]);

        public Vector3 Support(Vector3 dir) { // maximize dot between vert and dir
            var max   = float.NegativeInfinity;
            var max_d = Vector3.zero;
            
            for(int i = 0;i < points.Length;i++) {
                var v = GetVertex(i);
                var d = VectorHeader.Dot(v, dir);
                if(d > max) {
                    max   = d;
                    max_d = v;
                }
            }

            return max_d;
        }

        public Vector3 Origin => m.GetColumn(3);
    }

    public static bool Same(Vector3 v1, Vector3 v2) {
        return VectorHeader.Dot(v1, v2) > 0;
    }

    public static bool GJK(in ConvexPolyhedron a, in ConvexPolyhedron b) {
        count = 0;
        List<Vector3> splx = new List<Vector3>();
        var S = Support(a, b, a.Origin - b.Origin);
        var D = -S;

        var iterat = 1000;

        splx.Add(S);
        while(true && iterat-- < 1000) {
            var A = Support(a, b, D);
            if(!Same(A, D))
                return false;
            else {
                splx.Add(A);
                if(DoSimplex(ref D, splx))
                    return true;
            }
        }

        return false;
    }

    static int count = 0;
    public static int stopat = 4;

    public static bool DoSimplex(
        ref Vector3 D,
        List<Vector3> splx) {
        if(splx.Count == 2) { 
            // potential 0D -> 1D simplex
            // Gizmos.DrawLine(splx[0], splx[1]);
            return Simplex1D(ref D, splx);
        }
        else if(splx.Count == 3) { 
            // potential 1D -> 2D simplex generation
            
            return Simplex2D(ref D, splx);
        }
        else if(splx.Count == 4) {
            // potential 2D -> 3D simplex generation
            if(count == stopat) {
                Gizmos.DrawLine(splx[0], splx[1]);
                Gizmos.DrawLine(splx[0], splx[2]);
                Gizmos.DrawLine(splx[0], splx[3]);
                
                Gizmos.DrawLine(splx[1], splx[2]);
                Gizmos.DrawLine(splx[2], splx[3]);
                Gizmos.DrawLine(splx[3], splx[1]);

                Gizmos.color = Color.red; // d
                Gizmos.DrawWireSphere(splx[3], .5F);
                Gizmos.color = Color.blue; // c 
                Gizmos.DrawWireSphere(splx[2], .5F);
                Gizmos.color = Color.green; // b
                Gizmos.DrawWireSphere(splx[1], .5F);
                Gizmos.color = Color.yellow; // a
                Gizmos.DrawWireSphere(splx[0], .5F);
                Gizmos.color = Color.white;

                Vector3 abc = Vector3.Cross(splx[2] - splx[0], splx[1] - splx[0]);
                Vector3 centroid_abc = (splx[0] + splx[1] + splx[2]) / 3;
                
                Vector3 acd = Vector3.Cross(splx[3] - splx[0], splx[2] - splx[0]);
                Vector3 centroid_acd = (splx[0] + splx[2] + splx[3]) / 3;
                
                Vector3 adb = Vector3.Cross(splx[1] - splx[0], splx[3] - splx[0]);
                Vector3 centroid_adb = (splx[0] + splx[1] + splx[3]) / 3;
                
                Vector3 centroid_dcb = (splx[3] + splx[2] + splx[1]) / 3;
                Vector3 dcb = Vector3.Cross(splx[1] - splx[3], splx[2] - splx[3]);

                // normals
                abc.Normalize();
                acd.Normalize();
                adb.Normalize();
                dcb.Normalize();
                Gizmos.DrawLine(centroid_abc, centroid_abc + abc * 3.0F);
                Gizmos.DrawLine(centroid_acd, centroid_acd + acd * 3.0F);
                Gizmos.DrawLine(centroid_adb, centroid_adb + adb * 3.0F);
                //Gizmos.DrawLine(centroid_dcb, centroid_dcb + dcb * 3.0F);
            }
            bool b = Simplex3D(ref D, splx);
            // if(count == 248) {
            //     Gizmos.color = Color.magenta;
            //     Gizmos.DrawLine(Vector3.zero, D);
            //     Gizmos.color = Color.white;
            //     Debug.Log("new simplex: " + splx.Count);
            // }
            if(count == stopat) {
                //Gizmos.DrawLine(Vector3.zero, D);
            }

            count++;
            return b;
        }

        return false; // never should reach
    }

    public static bool Simplex1D(
        ref Vector3 D,
        List<Vector3> splx) {
        Vector3 a = splx[0];
        Vector3 b = splx[1];

        Vector3 ab = b - a;
        Vector3 ao =   - a;

        if(Same(ab, ao)) {
            // splx is not modified
            D = Vector3.Cross(Vector3.Cross(ab, ao), ab);
            D.Normalize();
            return false;
        }
        else {
            // splx is now just A, search is now in ao
            splx.Clear();
            splx.Add(a);
            D = ao;
            return false;
        }
    }

    public static bool Simplex2D(
        ref Vector3 D,
        List<Vector3> splx) {
            
            Vector3 a = splx[0];
            Vector3 b = splx[1];
            Vector3 c = splx[2];

            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ao =   - a;

            Vector3 abc = Vector3.Cross(ab, ac);

            if(Same(Vector3.Cross(abc, ac), ao)) {
                if(Same(ac, ao)) {
                    // AC
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(c);
                    D = Vector3.Cross(Vector3.Cross(ac, ao), ac);
                    D.Normalize();
                }
                else {
                    if(Same(ab, ao)) {
                        // AB
                        splx.Clear();
                        splx.Add(a);
                        splx.Add(b);
                        D = Vector3.Cross(Vector3.Cross(ab, ao), ab);
                        D.Normalize();
                    }else {
                        // A
                        splx.Clear();
                        splx.Add(a);
                        D = ao;
                    }
                }
            }else {
                if(Same(Vector3.Cross(ab, abc), ao)) {
                    if(Same(ab, ao)) {
                        // AB
                        splx.Clear();
                        splx.Add(a);
                        splx.Add(b);
                        D = Vector3.Cross(Vector3.Cross(ab, ao), ab);
                        D.Normalize();
                    }else {
                        // A
                        splx.Clear();
                        splx.Add(a);
                        D = ao;
                    }
                }else {
                    if(Same(abc, ao)) {
                        // ABC
                        D = abc;
                    }
                    else {
                        // NEGABC
                        splx.Clear();
                        splx.Add(c);
                        splx.Add(b);
                        splx.Add(a);
                        D = -abc;
                    }
                }
            }
            return false; // not done building the simplex!
    }

    public static bool Simplex3D(
        ref Vector3 D,
        List<Vector3> splx) {
            Vector3 a = splx[0];
            Vector3 b = splx[1];
            Vector3 c = splx[2];
            Vector3 d = splx[3];

            Vector3 ao = - a;

            Vector3 abc = Vector3.Cross(c - a, b - a);
            Vector3 acd = Vector3.Cross(d - a, c - a);
            Vector3 adb = Vector3.Cross(b - a, d - a);
            Vector3 dcb = Vector3.Cross(b - c, c - d);

            if(Same(abc, ao)) {
                if(Same(acd, ao)) {
                    if(Same(adb, ao)) {
                        // A
                        splx.Clear();
                        splx.Add(a);
                        D = ao;
                        return false;
                    }
                    else {
                        //AC
                        splx.Clear();
                        splx.Add(a);
                        splx.Add(c);
                        D = Vector3.Cross(Vector3.Cross(c - a, ao), c - a);
                        D.Normalize();
                        return false;
                    }
                }else {
                    if(Same(adb, ao)) {
                        // AB
                        splx.Clear();
                        splx.Add(a);
                        splx.Add(b);
                        D = Vector3.Cross(Vector3.Cross(b - a, ao), b - a);
                        D.Normalize();
                        return false;
                    }else {
                        // ABC
                        splx.Clear();
                        splx.Add(c);
                        splx.Add(b);
                        splx.Add(a);
                        D = abc;
                        return false;
                    }
                }
            }else {
                if(Same(acd, ao)) {
                    if(Same(adb, ao)) {
                        // AD
                        splx.Clear();
                        splx.Add(a);
                        splx.Add(d);
                        D = Vector3.Cross(Vector3.Cross(d - a, ao), d - a);
                        D.Normalize();
                        return false;
                    } else {
                        // ACD
                        splx.Clear();
                        splx.Add(d);
                        splx.Add(c);
                        splx.Add(a);
                        D = acd;
                        return false;
                    }
                } else {
                    if(Same(adb, ao)) {
                        // ADB
                        splx.Clear();
                        splx.Add(b);
                        splx.Add(d);
                        splx.Add(a);
                        D = adb;
                        return false;
                    }else if(!Same(dcb, ao)) {
                        // ENCLOSED
                        Debug.Log(count);
                        // Gizmos.color = Color.cyan;
                        // Gizmos.DrawLine(Vector3.zero, D);
                        return true;
                    }else {
                        splx.Clear();
                        splx.Add(b);
                        splx.Add(c);
                        splx.Add(d);
                        D = dcb;
                        return false;
                    }
                }
            }
    }
    
    public static Vector3 Support(
        in ConvexPolyhedron a,
        in ConvexPolyhedron b,
        Vector3 D) => a.Support(D) - b.Support(-D);
}