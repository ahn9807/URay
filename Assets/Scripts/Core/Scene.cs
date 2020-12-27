using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class Scene
    {
        public static bool RayIntersect(Ray ray, out Intersection hit)
        {
            UnityEngine.RaycastHit h;

            Physics.Raycast(ray.origin, new Vector3(ray.direction.x, ray.direction.y, -ray.direction.z), out h);

            hit = new Intersection
            {
                position = h.point,
                normal = h.normal,
                t = h.distance
            };

            return h.collider != null;
        }
    }
}
