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

            Physics.Raycast(Vector3d.ToVector3(ray.origin), new Vector3((float)ray.direction.x, (float)ray.direction.y, (float)-ray.direction.z), out h);

            hit = new Intersection
            {
                position = new Vector3d(h.point),
                normal = new Vector3d(h.normal),
                t = h.distance
            };

            if(hit.t < 0.001)
            {
                hit.t = 0;

                return false;
            }

            return h.collider != null;
        }
    }
}
