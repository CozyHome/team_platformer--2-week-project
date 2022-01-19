using UnityEngine;

namespace com.cozyhome.Vectors
{
    public static class VectorHeader
    {
        public static float Dot(Vector2 _a, Vector2 _b)
        {
            float _d = 0;
            for (int i = 0; i < 2; i++)
                _d += _a[i] * _b[i];
            return _d;
        }

        public static Vector2 ProjectVector(Vector2 _v, Vector2 _n)
        {
            Vector2 _c = Vector2.zero;
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _c[i] = _n[i] * _d;

            return _c;
        }

        public static void ProjectVector(ref Vector2 _v, Vector2 _n)
        {
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _v[i] = _n[i] * _d;
        }

        public static Vector2 ClipVector(Vector2 _v, Vector2 _n)
        {
            Vector2 _c = Vector2.zero;
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _c[i] = _v[i] - _n[i] * _d;

            return _c;
        }

        public static void ClipVector(ref Vector2 _v, Vector2 _n)
        {
            float _d = Dot(_v, _n);

            for (int i = 0; i < 2; i++)
                _v[i] = _v[i] - _n[i] * _d;
        }

        public static float Dot(Vector3 _a, Vector3 _b)
        {
            float _d = 0;
            for (int i = 0; i < 3; i++)
                _d += _a[i] * _b[i];
            return _d;
        }

        public static Vector3 ProjectVector(Vector3 _v, Vector3 _n)
        {
            Vector3 _c = Vector3.zero;
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _c[i] = _n[i] * _d;
            return _c;
        }

        public static void ProjectVector(ref Vector3 _v, Vector3 _n)
        {
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _v[i] = _n[i] * _d;
        }

        public static Vector3 ClipVector(Vector3 _v, Vector3 _n)
        {
            Vector3 _c = Vector3.zero;
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _c[i] = _v[i] - _n[i] * _d;
            return _c;
        }

        public static void ClipVector(ref Vector3 _v, Vector3 _n)
        {
            float _d = Dot(_v, _n);
            for (int i = 0; i < 3; i++)
                _v[i] = _v[i] - _n[i] * _d;
        }

        public static Vector3 ClosestPointOnPlane(
            Vector3 _point,
            Vector3 _planecenter,
            Vector3 _planenormal)
        => _point + ProjectVector(_planecenter - _point, _planenormal);

        public static Vector3 CrossProjection(
            Vector3 _v,
            Vector3 _u,
            Vector3 _n)
        {
            float _m = _v.magnitude;
            Vector3 _r = Vector3.Cross(_v, _u);
            _v = Vector3.Cross(_n, _r);
            _v.Normalize();
            return _v * _m;
        }

        public static void CrossProjection(
            ref Vector3 _v,
            Vector3 _u,
            Vector3 _n)
        {
            float _m = _v.magnitude;
            Vector3 _r = Vector3.Cross(_v, _u);
            Vector3 _f = Vector3.Cross(_n, _r);
            if (_f.sqrMagnitude > 0)
            {
                _v = _f;
                _v.Normalize();
                _v *= _m;
            }
        }

        public static float LinePlaneIntersection((Vector3 p, Vector3 r) line, (Vector3 x, Vector3 n) plane) 
        {
            // (c - p) * n =  upper
            // (r) * n = lower
            float l = Dot(line.r, plane.n);
            if(Mathf.Approximately(l, 0F))
                return -1F;
            else
            {
                float u = Dot(plane.x - line.p, plane.n);
                return u / l;
            }
        }

        public static (Vector3 a, Vector3 b) ClosestPointTriangle(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {

            Vector3 ao = p - tri.a;
            Vector3 bo = p - tri.b;
            Vector3 co = p - tri.c;
            
            Vector3 ab = tri.b - tri.a;
            Vector3 bc = tri.c - tri.b;
            Vector3 ca = tri.a - tri.c;  

            Vector3 abc  = Vector3.Cross(ab, bc);
            Vector3 ab_n = Vector3.Cross(ab, abc);
            Vector3 bc_n = Vector3.Cross(bc, abc);
            Vector3 ca_n = Vector3.Cross(ca, abc);

            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };
            
            // vertex regions
            if(Same(ao, ca) && !Same(ao, ab)) {
                return (p, tri.a);
            }
            else if(Same(bo, ab) && !Same(bo, bc)) {
                return (p, tri.b);
            }
            else if(Same(co, bc) && !Same(co, ca)) {
                return (p, tri.c);
            }
            
            // edge regions
            if(Same(ab_n, ao)) {
                // isolate vertex b
                if(Same(ao, ab) && Same(bo, ab))
                    return (p, tri.b);
                else if(!Same(ao, ab) && !Same(bo, ab))
                    return (p, tri.a); // vert a
                else // vert ab
                    return (p, p - VectorHeader.ProjectVector(ao, ab_n.normalized));
            }
            else if(Same(bc_n, bo)) {
                // isolate vertex c
                if(Same(bo, bc) && Same(co, bc))
                    return (p, tri.c);
                else if(!Same(bo, bc) && !Same(co, bc)) // isolate vertex b
                    return (p, tri.b);
                else
                    return (p, p - VectorHeader.ProjectVector(bo, bc_n.normalized));
            }
            else if(Same(ca_n, co)) {
                if(Same(ao, ca) && Same(co, ca))
                    return (p, tri.a);
                else if(!Same(ao, ca) && !Same(co, ca))
                    return (p, tri.c);
                else 
                    return (p, p - VectorHeader.ProjectVector(co, ca_n.normalized));
            }
            else
                return (p, p - VectorHeader.ProjectVector(co, abc.normalized));
        } 

        public static (Vector3 a, Vector3 b) ClosestPointEdge(
        (Vector3 a, Vector3 b) edge,
        Vector3 p) {
            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };
            
            Vector3 ao = p - edge.a;
            Vector3 bo = p - edge.b;
            Vector3 ab = edge.b - edge.a;
            Vector3 abo = Vector3.Cross(ao, bo);
            
            if(Same(ao, ab) && Same(bo, ab)) {
                return (p, edge.b);
            }
            else if(!Same(ao, ab) && !Same(bo, ab)) {
                return (p, edge.a);
            }
            else {
                return (p, p - VectorHeader.ProjectVector(p - edge.a, Vector3.Cross(abo, ab).normalized));
            }
        }

        public static float Barycentric1DClamped(
        (Vector3 a, Vector3 b) edge,
        Vector3 p) {
            
            Vector3 ao = p - edge.a;
            Vector3 bo = p - edge.b;
            Vector3 ab = edge.b - edge.a;
            Vector3 abo = Vector3.Cross(ao, bo);
            
            return Mathf.Clamp01(VectorHeader.Dot(p - edge.a, Vector3.Cross(abo, ab).normalized));
        }

        public static Vector3 Barycentric2D(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {

            Vector3 ao = p - tri.a;
            Vector3 bo = p - tri.b;
            Vector3 co = p - tri.c;
            
            Vector3 ab = tri.b - tri.a;
            Vector3 bc = tri.c - tri.b;
            Vector3 ca = tri.a - tri.c;  

            Vector3 abc  = Vector3.Cross(ab, bc);
            float area = abc.magnitude;
            abc /= area;

            Vector3 v = new Vector3(
            Vector3.Dot(Vector3.Cross(p - tri.b, tri.c - p), abc) / area, // 0 -> ab x ao oab
            Vector3.Dot(Vector3.Cross(p - tri.c, tri.a - p), abc) / area, // 1 -> bc x bo obc
            0F);
            v[2] = 1 - v[0] - v[1];  // 2 -> ca x co oca
            return v;
            
        } 

        public static Vector3 Barycentric2DClamped(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {

            Vector3 ao = p - tri.a;
            Vector3 bo = p - tri.b;
            Vector3 co = p - tri.c;
            
            Vector3 ab = tri.b - tri.a;
            Vector3 bc = tri.c - tri.b;
            Vector3 ca = tri.a - tri.c;  

            Vector3 abc  = Vector3.Cross(ab, ca);
            float area = abc.magnitude;
            abc /= area;

            Vector3 ab_n = Vector3.Cross(abc, ab);
            Vector3 bc_n = Vector3.Cross(abc, bc);
            Vector3 ca_n = Vector3.Cross(abc, ca);
            
            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };
            
            Vector3 Bary(Vector3 p) {
                Vector3 v = new Vector3(
                Vector3.Dot(Vector3.Cross(p - tri.b, tri.c - p), abc) / area, // 0 -> ab x ao oab
                Vector3.Dot(Vector3.Cross(p - tri.c, tri.a - p), abc) / area, // 1 -> bc x bo obc
                0F);
                v[2] = 1 - v[0] - v[1];  // 2 -> ca x co oca
                return v;
            }

            // hierarchy:
                // normals
                    // verts
                        // edges
            if(Same(ab_n, ao)) {
                // isolate vertex b
                if(Same(ao, ab) && Same(bo, ab))
                    return Bary(tri.b);
                else if(!Same(ao, ab) && !Same(bo, ab))
                    return Bary(tri.a); // vert a
                else // vert ab
                    return Bary(p - VectorHeader.ProjectVector(ao, ab_n.normalized));
            }
            else if(Same(bc_n, bo)) {
                // isolate vertex c
                if(Same(bo, bc) && Same(co, bc))
                    return Bary(tri.c);
                else if(!Same(bo, bc) && !Same(co, bc)) // isolate vertex b
                    return Bary(tri.b);
                else
                    return Bary(p - VectorHeader.ProjectVector(bo, bc_n.normalized));
            }
            else if(Same(ca_n, co)) {
                if(Same(ao, ca) && Same(co, ca))
                    return Bary(tri.a);
                else if(!Same(ao, ca) && !Same(co, ca))
                    return Bary(tri.c);
                else 
                    return Bary(p - VectorHeader.ProjectVector(co, ca_n.normalized));
            }
            else
                return Bary(p);
        }

        public enum Barycentric2DState {
            A,
            B,
            C,
            AB,
            BC,
            CA,
            ABC
        };

        public static Barycentric2DState Barycentric2DVoronoi(
        (Vector3 a, Vector3 b, Vector3 c) tri,
            Vector3 p) {

            Vector3 ao = p - tri.a;
            Vector3 bo = p - tri.b;
            Vector3 co = p - tri.c;
            
            Vector3 ab = tri.b - tri.a;
            Vector3 bc = tri.c - tri.b;
            Vector3 ca = tri.a - tri.c;  

            Vector3 abc  = Vector3.Cross(ab, ca);
            float area = abc.magnitude;
            abc /= area;

            Vector3 ab_n = Vector3.Cross(abc, ab);
            Vector3 bc_n = Vector3.Cross(abc, bc);
            Vector3 ca_n = Vector3.Cross(abc, ca);
            
            bool Same(Vector3 v1, Vector3 v2) {
                return VectorHeader.Dot(v1, v2) > 0;
            };
            
            // Vector3 Bary(Vector3 p) {
            //     Vector3 v = new Vector3(
            //     Vector3.Dot(Vector3.Cross(p - tri.b, tri.c - p), abc) / area, // 0 -> ab x ao oab
            //     Vector3.Dot(Vector3.Cross(p - tri.c, tri.a - p), abc) / area, // 1 -> bc x bo obc
            //     0F);
            //     v[2] = 1 - v[0] - v[1];  // 2 -> ca x co oca
            //     return v;
            // }

            // hierarchy:
                // normals
                    // verts
                        // edges
            if(Same(ab_n, ao)) {
                // isolate vertex b
                if(Same(ao, ab) && Same(bo, ab))
                    return Barycentric2DState.B; //Bary(tri.b);
                else if(!Same(ao, ab) && !Same(bo, ab))
                    return Barycentric2DState.A; // Bary(tri.a); // vert a
                else // vert ab
                    return Barycentric2DState.AB; // Bary(p - VectorHeader.ProjectVector(ao, ab_n.normalized));
            }
            else if(Same(bc_n, bo)) {
                // isolate vertex c
                if(Same(bo, bc) && Same(co, bc))
                    return Barycentric2DState.C; //Bary(tri.c);
                else if(!Same(bo, bc) && !Same(co, bc)) // isolate vertex b
                    return Barycentric2DState.B; // Bary(tri.b);
                else
                    return Barycentric2DState.BC; // Bary(p - VectorHeader.ProjectVector(bo, bc_n.normalized));
            }
            else if(Same(ca_n, co)) {
                if(Same(ao, ca) && Same(co, ca))
                    return Barycentric2DState.A; // Bary(tri.a);
                else if(!Same(ao, ca) && !Same(co, ca))
                    return Barycentric2DState.C; // Bary(tri.c);
                else 
                    return Barycentric2DState.CA; // Bary(p - VectorHeader.ProjectVector(co, ca_n.normalized));
            }
            else
                return Barycentric2DState.ABC; // Bary(p);
        }
    }
}