using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Vectors;
using UnityEngine;
using static BooleanGJK;

public class DistanceGJK
{
    public static bool Same(Vector3 v1, Vector3 v2) {
        return VectorHeader.Dot(v1, v2) > 0;
    }
    
    public static int iteration = 0;
    public static float GJK(in ConvexPolyhedron a, in ConvexPolyhedron b) {
        GJKSimplex splx = new GJKSimplex();
        const float eps = 0.0165F;

        bool isDupe(MinkowskiVertex p) {
            for(int i = 0;i < splx.Count;i++) {
                if((p.v - splx[i].v).magnitude < eps)
                    return true;
                else
                    continue;
            }
            return false;
        }

        // let a = support(a,  v)
        // let b = support(b, -v)
        // let v = a - b
        // splx.add(v)
        var v = a.Origin - b.Origin;
        iteration = 0;

        do {
            var d = -v;
            var A = a.Support(d);
            var B = b.Support(-d);
            var W = new MinkowskiVertex(A, B, A - B);

            float v1 = Vector3.Dot(v, v);
            
            if (isDupe(W) || v1 - Vector3.Dot(W.v, v) < eps * v1)
            {
                Debug.Log(iteration);
                DrawClosest(splx, v);
                return v.magnitude;
            }
            else {
                splx.Push(W);
                MutateSimplex(ref v, ref splx);
            }

            // 1. Check Duplicates (termination #1)
            // 2. Check Movement toward Origin (termination #2)
            // 3. DoSimplex() (termination #3)
        } while(iteration++ < (a.Count + b.Count) && splx.Count < 4);

        DrawClosest(splx, v);
        return v.magnitude; // we are intersecting

        static void DrawClosest(GJKSimplex splx, Vector3 v)
        {
            Gizmos.color = Color.yellow;
            switch (splx.Count)
            {
                case 1:
                    Gizmos.DrawLine(splx[0].a, splx[0].b);
                    break;
                case 2:
                    float lambda = VectorHeader.Barycentric1DClamped((splx[0].v, splx[1].v), v);
                    Gizmos.DrawLine(
                        (splx[0].a * (1 - lambda) + splx[1].a * (lambda)),
                        (splx[0].b * (1 - lambda) + splx[1].b * (lambda))
                    );
                    break;
                case 3:
                    Vector3 bary = VectorHeader.Barycentric2DClamped((splx[0].v, splx[1].v, splx[2].v), v);
                    Gizmos.DrawLine(
                        (splx[0].a * bary[0] + splx[1].a * bary[1] + splx[2].a * bary[2]),
                        (splx[0].b * bary[0] + splx[1].b * bary[1] + splx[2].b * bary[2])
                    );
                    break;
                case 4:
                    Vector4 vol = VectorHeader.Barycentric3DClamped((splx[0].v, splx[1].v, splx[2].v, splx[3].v), v);
                    Gizmos.DrawWireSphere(
                        (splx[0].a * vol[0] + splx[1].a * vol[1] + splx[2].a * vol[2] + splx[3].a * vol[3]),
                        0.0625F
                    );

                break;
            }
            Gizmos.color = Color.white;
        }
    }

    public static int stopat = 0;

    public static void MutateSimplex(ref Vector3 v, ref GJKSimplex splx) {
        switch(splx.Count) {
            case 1:
                Simplex0D(ref v, ref splx);
                break;
            case 2:
                Simplex1D(ref v, ref splx);
                break;
            case 3:
                Simplex2D(ref v, ref splx);
                break;
            case 4:
                Simplex3D(ref v, ref splx);
                break;
        }
    }

    public static void Simplex0D(ref Vector3 v, ref GJKSimplex splx) { 
        if(iteration == stopat) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(splx[0].v, 0.0625F);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(splx[0].v, Vector3.zero);
        }

        v = splx[0].v;
    }

    public static void Simplex1D(ref Vector3 v, ref GJKSimplex splx) { 
        (int r, Vector3 cv) = VectorHeader.Barycentric1D_GJK((splx[0].v, splx[1].v), (0x1, 0x2), Vector3.zero);

        if(iteration == stopat) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cv, 0.0625F);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(cv, Vector3.zero);
            Gizmos.DrawLine(splx[0].v, splx[1].v);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(splx[0].v, 0.0625F);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(splx[1].v, 0.0625F);
        }

        v = cv;
        switch(r) {
            case 0x1: // A
                var a = splx[0];
                splx.Clear();
                splx.Push(a);
            break;
        }
    }

    public static void Simplex2D(ref Vector3 v, ref GJKSimplex splx) {
        (int r, Vector3 cv) = VectorHeader.Barycentric2D_GJK((splx[0].v, splx[1].v, splx[2].v), (0x1, 0x2, 0x4), Vector3.zero);
        var a = splx[0];
        var b = splx[1];
        var c = splx[2];

        if(iteration == stopat) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cv, 0.0625F);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(cv, Vector3.zero);
            Gizmos.DrawLine(splx[0].v, splx[1].v);
            Gizmos.DrawLine(splx[1].v, splx[2].v);
            Gizmos.DrawLine(splx[2].v, splx[0].v);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(splx[0].v, 0.0625F);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(splx[1].v, 0.0625F);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(splx[2].v, 0.0625F);
        }

        v = cv;
        switch(r) {
            case 0x1: // A
                splx.Clear();
                splx.Push(a);
            break;
            case 0x3: // AB
                splx.Clear();
                splx.Add(b);
                splx.Add(a);
            break;
            case 0x5: // AC
                splx.Clear();
                splx.Add(c);
                splx.Add(a);
            break;
            case 0x7:
                // ABC (up/down does not matter here as (v - o) * -1 flips this)
                splx.Clear();
                float signed_vol = Vector3.Dot(Vector3.zero - v, Vector3.Cross(splx[2].v - splx[0].v, splx[1].v - splx[0].v));
                int sign = (int) Mathf.Sign(signed_vol);
                if(sign == 1) {
                    splx.Add(c);
                    splx.Add(b);
                    splx.Add(a);
                }
                else {
                    splx.Add(a);
                    splx.Add(b);
                    splx.Add(c);
                }
            break;
        }
    }

    public static void Simplex3D(ref Vector3 v, ref GJKSimplex splx) {
        var a = splx[0];
        var b = splx[1];
        var c = splx[2];
        var d = splx[3];
        (int r, Vector3 cv) = VectorHeader.Barycentric3D_GJK((a.v, b.v, c.v, d.v), Vector3.zero);

        if(iteration == stopat) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cv, 0.0625F);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(cv, Vector3.zero);
            Gizmos.DrawLine(splx[0].v, splx[1].v);
            Gizmos.DrawLine(splx[0].v, splx[2].v);
            Gizmos.DrawLine(splx[0].v, splx[3].v);

            Gizmos.DrawLine(splx[1].v, splx[2].v);
            Gizmos.DrawLine(splx[2].v, splx[3].v);
            Gizmos.DrawLine(splx[3].v, splx[1].v);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(splx[0].v, 0.0625F);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(splx[1].v, 0.0625F);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(splx[2].v, 0.0625F);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(splx[3].v, 0.0625F);
        }
    
        // Debug.Log(iteration + " " + r);

        v = cv;
        switch(r) {
            case 0x0: // ENCLOSED
            break;
            case 0x1: // A
                splx.Clear();
                splx.Add(a);
            break;
            case 0x2: // B
                splx.Clear();
                splx.Add(b);
            break;
            case 0x3: // AB
                splx.Clear();
                splx.Add(a);
                splx.Add(b);
            break;
            case 0x4: // C
                splx.Clear();
                splx.Add(c);
            break;
            case 0x5: // AC
                splx.Clear();
                splx.Add(a);
                splx.Add(c);
            break;
            case 0x6: // BC
                splx.Clear();
                splx.Add(b);
                splx.Add(c);
            break;
            case 0x7: // ABC
                splx.Clear();
                splx.Add(a);
                splx.Add(b);
                splx.Add(c);
            break;
            case 0x8: // D
                splx.Clear();
                splx.Add(d);
            break;
            case 0x9: // AD
                splx.Clear();
                splx.Add(a);
                splx.Add(d);
            break;
            case 10: // BD
                splx.Clear();
                splx.Add(b);
                splx.Add(d);
            break;
            case 11: // ABD
                splx.Clear();
                splx.Add(d);
                splx.Add(b);
                splx.Add(a);
            break;
            case 12: // CD
                splx.Clear();
                splx.Add(c);
                splx.Add(d);
            break;
            case 13: // ACD
                splx.Clear();
                splx.Add(a);
                splx.Add(c);
                splx.Add(d);
            break;
            case 14: // BCD
                splx.Clear();
                splx.Add(d);
                splx.Add(c);
                splx.Add(b);
            break;
        }
    }
}
