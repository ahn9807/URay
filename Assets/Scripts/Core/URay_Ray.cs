using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public struct URay_Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public URay_Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }

        public Vector3 At(float t)
        {
            return origin + t * direction;
        }
    }

    public class URay_Intersection
    {
        public Vector3 point;
        public Vector3 normal;
        public float distance;
        public int objectID;
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

        public void OBSystem(Vector3 a, out Vector3 b, out Vector3 c)
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

            //b = new Vector3();
            //c = new Vector3();
            //Vector3.OrthoNormalize(ref n, ref b, ref c);
        }
    }
}

