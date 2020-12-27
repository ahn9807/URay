using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Scene
    {
        public static bool RayIntersect(URay_Ray ray, out Intersection hit)
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
