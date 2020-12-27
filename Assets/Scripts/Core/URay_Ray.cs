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
    }
}

