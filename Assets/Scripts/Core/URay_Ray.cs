using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public URay_Ray() { }
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

    public class Intersection
    {
        public Vector3 position;
        public Vector3 normal;
        public float t;
    }
}

