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

    public struct MinkowskiVertex {
        public Vector3 a;
        public Vector3 b;
        public Vector3 v;

        public MinkowskiVertex(Vector3 a, Vector3 b, Vector3 ba) {
            this.a = a; this.b = b; this.v = ba;
        }

        public MinkowskiVertex Copy() {
            return new MinkowskiVertex(this.a, this.b, this.v);
        }
    }

    public struct GJKSimplex {
        public MinkowskiVertex a;
        public MinkowskiVertex b;
        public MinkowskiVertex c;
        public MinkowskiVertex d;
        private int count;

        public void Add(MinkowskiVertex v) {
            switch(count) {
                case 0:
                    a = v;
                    break;
                case 1:
                    b = v;
                    break;
                case 2:
                    c = v;
                    break;
                case 3:
                    d = v;
                    break;
                default:
                    count--;
                break;
            }
            count++;
        }

        public void Push(MinkowskiVertex v) {
            d = c;
            c = b;
            b = a;
            a = v;
            count++;
            count = (count > 4) ? 4 : count;
        }

        public void Clear() {
            count = 0;
        }

        public MinkowskiVertex this[int i] {
            get { 
                switch(i) {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    case 2:
                        return c;
                    case 3:
                        return d;
                    default:
                        return new MinkowskiVertex(Vector3.zero, Vector3.zero, Vector3.zero); // should never happen
                }
            }
            set {
                switch(i) {
                    case 0:
                        a = value;
                        break;
                    case 1:
                        b = value;
                        break;
                    case 2:
                        c = value;
                        break;
                    case 3:
                        d = value;
                        break;
                    default:
                        break;
                }
            }
        }

        public int Count => count;
    }

    public enum GJKCASE {
        INTERSECTING = 0,
        NONINTERSECTING = 1,
        DEGENERATE = 2
    };

    public static GJKCASE GJK(in ConvexPolyhedron a, in ConvexPolyhedron b) {
        
        GJKSimplex splx = new GJKSimplex();
        MinkowskiVertex LastVertex = Support(a, b, a.Origin - b.Origin);
        Vector3 D = -LastVertex.v;

        count = 0;
        int iterat = 100;
        splx.Add(LastVertex);

        bool isSame(Vector3 v1) {
            for(int i = 0;i<splx.Count;i++) {
                if((v1 - splx[i].v).magnitude < Vector3.kEpsilon)
                    return true;
                else 
                    continue;
            }

            return false;
        }

        while(iterat-- > 0 && true) {
            count++;
            MinkowskiVertex Vertex = Support(a, b, D);
            float dd = Vector3.Dot(Vertex.v, Vertex.v); 
            if(isSame(Vertex.v) ||
               splx.Count > 2 && dd - Vector3.Dot(Vertex.v, LastVertex.v) <= dd * Vector3.kEpsilon) {
                // splx.Push(Vertex);
                switch(splx.Count) {
                    case 1:
                        Gizmos.DrawLine(splx[0].a, splx[1].b);
                    break;
                    case 2:
                        // edge containing new vertex
                        float _l = VectorHeader.Barycentric1DClamped(
                            (splx[0].v, splx[1].v),
                            Vector3.zero
                        );

                        Vector3 _p3 = splx[0].a * (_l) + splx[1].a * (1 -  _l);
                        Vector3 _p4 = splx[0].b * (_l) + splx[1].b * (1 -  _l);
                        Gizmos.DrawLine(_p3, _p4);
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawWireSphere(splx[0].a, .125F);
                        Gizmos.DrawWireSphere(splx[1].a, .125F);
                        Gizmos.DrawWireSphere(Vertex.a, .125F);

                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(splx[0].b, .125F);
                        Gizmos.DrawWireSphere(splx[1].b, .125F);
                        Gizmos.DrawWireSphere(Vertex.b, .125F);
                        Gizmos.DrawLine(_p3, _p4);
                    break;
                    case 3:
                        // triangle containing new vertex
                        Vector3 _c = VectorHeader.Barycentric2DClamped(
                            (splx[0].v, splx[1].v, splx[2].v),
                            Vector3.zero
                        );
                        // Debug.Log(_c);

                        Vector3 _p1 = splx[0].a * _c[0] + splx[1].a * _c[1] + splx[2].a * _c[2];
                        Vector3 _p2 = splx[0].b * _c[0] + splx[1].b * _c[1] + splx[2].b * _c[2];
                        Gizmos.color = Color.red;
                        // Debug.Log(splx[1].a + " " + splx[1].b);
                        Gizmos.DrawWireSphere(splx[0].a, .125F);
                        Gizmos.DrawWireSphere(splx[1].a, .125F);
                        Gizmos.DrawWireSphere(splx[2].a, .125F);
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(splx[0].b, .125F);
                        Gizmos.DrawWireSphere(splx[1].b, .125F);
                        Gizmos.DrawWireSphere(splx[2].b, .125F);
                        Gizmos.color = Color.yellow;

                        Gizmos.DrawWireSphere(Vertex.a, .125F);
                        Gizmos.DrawWireSphere(Vertex.b, .125F);
                        Gizmos.DrawLine(_p1, _p2);
                    break;
                    case 4:
                        // tetrahedron containing new vertex this should never happen
                    break;
                }

                Debug.Log("Converged non-intersecting @: " + count + " " + splx.Count);
                return GJKCASE.NONINTERSECTING;
            }
            else { 
                splx.Push(Vertex);
                LastVertex = Vertex;
                

                if(DoSimplex(ref D, ref Vertex, ref splx)) {
                    Debug.Log("Converged intersecting @: " + count + " " + splx.Count);
                    return GJKCASE.INTERSECTING;
                }
                D = -Vertex.v;
            }
        }

        Debug.Log("it: " + (count));
        return GJKCASE.DEGENERATE;
    }

    static int count = 0;
    public static int stopat = 1;

    public static bool DoSimplex(ref Vector3 D, ref MinkowskiVertex V, ref GJKSimplex splx) {
        // potential 0D -> 1D simplex
        if(splx.Count == 2) {
            if(count == stopat) {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(splx[0].v, splx[1].v);
            }

            return Simplex1D(ref D, ref V, ref splx);
        }
        // potential 1D -> 2D simplex generation
        else if(splx.Count == 3) { 
            if(count == stopat) {
                Vector3 a = splx[0].v;
                Vector3 b = splx[1].v;
                Vector3 c = splx[2].v;

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, a);    

                Gizmos.DrawWireSphere(a, .125F);    
                Gizmos.DrawWireSphere(b, .125F);    
                Gizmos.DrawWireSphere(c, .125F);    
                Gizmos.color = Color.white;
            }

            return Simplex2D(ref D, ref V, ref splx);
        }
        // potential 2D -> 3D simplex generation
        else if(splx.Count == 4) {
            if(count == stopat) {
                Color draw = count == stopat ? Color.white : Color.black;
                float radi = count == stopat ? 0.125F : 0.25F;
                Gizmos.color = draw;
                Vector3 a = splx[0].v;
                Vector3 b = splx[1].v;
                Vector3 c = splx[2].v;
                Vector3 d = splx[3].v;
        
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(a, c);
                Gizmos.DrawLine(a, d);
                
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, d);
                Gizmos.DrawLine(d, b);

                Gizmos.color = Color.red; // d
                Gizmos.DrawWireSphere(d, radi);
                Gizmos.color = Color.blue; // c 
                Gizmos.DrawWireSphere(c, radi);
                Gizmos.color = Color.green; // b
                Gizmos.DrawWireSphere(b, radi);
                Gizmos.color = Color.yellow; // a
                Gizmos.DrawWireSphere(a, radi);
                Gizmos.color = draw;

                Vector3 abc = Vector3.Cross(b - a, c - a);
                Vector3 acd = Vector3.Cross(c - a, d - a);
                Vector3 adb = Vector3.Cross(d - a, b - a);
                Vector3 dcb = Vector3.Cross(c - d, b - c);
                
                Vector3 centroid_abc = (a + b + c) / 3;
                Vector3 centroid_acd = (a + c + d) / 3;
                Vector3 centroid_adb = (a + d + b) / 3;                
                
                // normals
                abc.Normalize();
                acd.Normalize();
                adb.Normalize();
                dcb.Normalize();
                Gizmos.DrawLine(centroid_abc, centroid_abc + abc * 3.0F);
                Gizmos.DrawLine(centroid_acd, centroid_acd + acd * 3.0F);
                Gizmos.DrawLine(centroid_adb, centroid_adb + adb * 3.0F);
            }
            return Simplex3D(ref D, ref V, ref splx);
        }
        return false; // never should reach
    }

    public static bool Simplex1D(ref Vector3 D, ref MinkowskiVertex V, ref GJKSimplex splx) {
        MinkowskiVertex a = splx[0];
        MinkowskiVertex b = splx[1];

        Vector3 ab = b.v - a.v;
        Vector3 ao =     - a.v;

        if(Same(ab, ao)) {
            // splx is not modified
            D = Vector3.Cross(Vector3.Cross(ab, ao), ab);
        }
        else {
            // splx is now just A, search is now in ao
            splx.Clear();
            splx.Add(a);
            D = ao;
        }

        var (v1, v2) = VectorHeader.ClosestPointEdge((a.v, b.v), Vector3.zero);
        V.v = v2 - v1;

        return false;
    }

    public static bool Simplex2D(ref Vector3 D, ref MinkowskiVertex V, ref GJKSimplex splx) {
        MinkowskiVertex a = splx[0];
        MinkowskiVertex b = splx[1];
        MinkowskiVertex c = splx[2];

        Vector3 ab = b.v - a.v;
        Vector3 ac = c.v - a.v;
        Vector3 ao =   - a.v;

        Vector3 abc = Vector3.Cross(ab, ac);
        
        if(Same(Vector3.Cross(abc, ac), ao)) {
            if(Same(ac, ao)) {
                // AC
                splx.Clear();
                splx.Add(a);
                splx.Add(c);
                D = Vector3.Cross(Vector3.Cross(ac, ao), ac);

                var (v1, v2) = VectorHeader.ClosestPointEdge(
                    (a.v, c.v),
                    Vector3.zero
                );
                V.v = v2 - v1;

            } else {
                if(Same(ab, ao)) {
                    // AB
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(b);
                    D = Vector3.Cross(Vector3.Cross(ab, ao), ab);

                    var (v1, v2) = VectorHeader.ClosestPointEdge(
                        (a.v, b.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;

                } else {
                    // A
                    splx.Clear();
                    splx.Add(a);
                    D = ao;
                    V.v = a.v;
                }
            }
        } else {
            if(Same(Vector3.Cross(ab, abc), ao)) {
                if(Same(ab, ao)) {
                    // AB
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(b);
                    D = Vector3.Cross(Vector3.Cross(ab, ao), ab);
                    
                    var (v1, v2) = VectorHeader.ClosestPointEdge(
                        (a.v, b.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;
                }else {
                    // A
                    splx.Clear();
                    splx.Add(a);
                    D = ao;
                    V.v = a.v;
                }
            } else {
                if(Same(abc, ao)) {
                    // ABC
                    D = abc;
                    
                    var (v1, v2) = VectorHeader.ClosestPointTriangle(
                        (a.v, b.v, c.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;
                } else {
                    // NEGABC
                    splx.Clear();
                    splx.Add(c);
                    splx.Add(b);
                    splx.Add(a);
                    D = -abc;

                    var (v1, v2) = VectorHeader.ClosestPointTriangle(
                        (a.v, b.v, c.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;
                }
            }
        }
        return false; // not done building the simplex!
    }

    public static bool Simplex3D(ref Vector3 D, ref MinkowskiVertex V, ref GJKSimplex splx) {
        MinkowskiVertex a = splx[0];
        MinkowskiVertex b = splx[1];
        MinkowskiVertex c = splx[2];
        MinkowskiVertex d = splx[3];

        Vector3 ao = - a.v;

        Vector3 abc = Vector3.Cross(b.v - a.v, c.v - a.v);
        Vector3 acd = Vector3.Cross(c.v - a.v, d.v - a.v);
        Vector3 adb = Vector3.Cross(d.v - a.v, b.v - a.v);
        

        if(Same(abc, ao)) {
            if(Same(acd, ao)) {
                if(Same(adb, ao)) {
                    // A
                    splx.Clear();
                    splx.Add(a);
                    D = ao;
                    V.v = a.v;

                    return false;
                } else {
                    //AC
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(c);
                    D = Vector3.Cross(Vector3.Cross(c.v - a.v, ao), c.v - a.v);
                    
                    var (v1, v2) = VectorHeader.ClosestPointEdge(
                        (a.v, c.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;

                    return false;
                }
            } else {
                if(Same(adb, ao)) {
                    // AB
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(b);
                    D = Vector3.Cross(Vector3.Cross(b.v - a.v, ao), b.v - a.v);

                    var (v1, v2) = VectorHeader.ClosestPointEdge(
                        (a.v, b.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;

                    return false;
                } else {
                    // ABC
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(b);
                    splx.Add(c);
                    D = abc;

                    var(v1, v2) = VectorHeader.ClosestPointTriangle(
                        (a.v, b.v, c.v),
                        Vector3.zero
                    );

                    V.v = v2 - v1;

                    return false;
                }
            }
        } else {
            if(Same(acd, ao)) {
                
                if(Same(adb, ao)) {
                    
                    // AD
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(d);
                    D = Vector3.Cross(Vector3.Cross(d.v - a.v, ao), d.v - a.v);

                    var (v1, v2) = VectorHeader.ClosestPointEdge(
                        (a.v, d.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;
                    // if(count == stopat) {
                    //     Debug.Log("OK " + count);
                    // }

                    return false;
                } else {
                    // ACD
                    splx.Clear();
                    splx.Add(a);
                    splx.Add(c);
                    splx.Add(d);
                    D = acd;

                    var(v1, v2) = VectorHeader.ClosestPointTriangle(
                        (a.v, c.v, d.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;
                    
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
                    var(v1, v2) = VectorHeader.ClosestPointTriangle(
                        (a.v, d.v, b.v),
                        Vector3.zero
                    );
                    V.v = v2 - v1;
                    return false;
                }
                else {
                    V.v = Vector3.zero;
                    return true;
                }
            }
        }
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