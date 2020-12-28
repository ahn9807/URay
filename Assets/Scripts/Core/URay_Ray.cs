using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Ray
    {
        public Vector3d origin;
        public Vector3d direction;

        public URay_Ray() { }
        public URay_Ray(Vector3d origin, Vector3d direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Vector3d At(double t)
        {
            return origin + t * direction;
        }
    }

    public class Intersection
    {
        public Vector3d position;
        public Vector3d normal;
        public double t;
        public Transform tempTransform;
        public Mesh mesh;
        public URay_BSDF bsdf;
        public Vector3 baryCentricCoordinate;
        public Vector2 uv;
        public Vector3 f_n, f_s, f_t;

        public Vector3 ToWorld(Vector3 vec)
        {
            return f_s * vec.x + f_t * vec.z + f_n * vec.y;
        }

        public Vector3 ToLocal(Vector3 vec)
        {
            return new Vector3(Vector3.Dot(vec, f_s), Vector3.Dot(vec, f_n), Vector3.Dot(vec, f_t));
        }

        public URay_BSDF GetBSDF()
        {
            return bsdf;
        }

        public void coordinateSystem(Vector3 a, out Vector3 b, out Vector3 c)
        {
            if(Mathf.Abs(a.x) > Mathf.Abs(a.z))
            {
                float invLen = 1.0f / Mathf.Sqrt(a.x * a.x + a.y * a.y);
                c = new Vector3(a.y * invLen, -a.x * invLen, 0.0f);
            } else
            {
                float invLen = 1.0f / Mathf.Sqrt(a.z * a.z + a.y * a.y);
                c = new Vector3(0.0f, -a.z * invLen, a.y * invLen);
            }
            b = Vector3.Cross(c, a);
        }
    }
}

