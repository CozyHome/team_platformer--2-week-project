using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;

public class BooleanGJK {

    public struct ConvexPolyhedron {
        private Vector3[] points;
        private Matrix4x4 m;

        public ConvexPolyhedron(Vector3[] points, Transform t_m) {
            this.points = points;
            this.m = t_m.localToWorldMatrix;
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
        List<Vector3> splx = new List<Vector3>();
        var S = Support(a, b, a.Origin - b.Origin);
        var D = -S;

        splx.Add(S);
        while(true) {
            var A = Support(a, b, D);
            if(!Same(A, D))
                return false;
            else {
                splx.Add(A);
                if(DoSimplex(ref D, splx))
                    return true;
            }
        }
    }

    public static bool DoSimplex(
        ref Vector3 D,
        List<Vector3> splx) {
        if(splx.Count == 2) { 
            // potential 0D -> 1D simplex
            return Simplex1D(ref D, splx);
        }
        else if(splx.Count == 3) { 
            // potential 1D -> 2D simplex generation
            return Simplex2D(ref D, splx);
        }
        else if(splx.Count == 4) {
            // potential 2D -> 3D simplex generation
            return Simplex3D(ref D, splx);
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

            Vector3 abc = Vector3.Cross(b - a, c - a);
            Vector3 acd = Vector3.Cross(c - a, d - a);
            Vector3 adb = Vector3.Cross(d - a, b - a);

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
                        splx.Add(a);
                        splx.Add(b);
                        splx.Add(c);
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
                        splx.Add(a);
                        splx.Add(c);
                        splx.Add(d);
                        D = acd;
                        return false;
                    }
                } else {
                    if(Same(adb, ao)) {
                        // ADB
                        splx.Clear();
                        splx.Add(a);
                        splx.Add(d);
                        splx.Add(b);
                        D = adb;
                        return false;
                    }else {
                        // ENCLOSED
                        return true;
                    }
                }
            }
    }
    
    public static Vector3 Support(
        in ConvexPolyhedron a,
        in ConvexPolyhedron b,
        Vector3 D) => a.Support(D) - b.Support(-D);
}