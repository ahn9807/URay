using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Scene
    {
        private Dictionary<int, URay_Object> objects;

        public URay_Scene()
        {
            objects = new Dictionary<int, URay_Object>();
        }

        public void AddObject(GameObject gameObject)
        {
            //only add object with mesh
            if(gameObject.GetComponent<MeshRenderer>() != null)
            {
                URay_BSDF newObjectBSDF = URay_Material.ParseBSDFFromMaterial(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
                URay_Object newURayObject = new URay_Object(gameObject.GetInstanceID(), newObjectBSDF);
                objects.Add(gameObject.GetInstanceID(), newURayObject);
            }
        }

        public bool RayIntersect(URay_Ray ray, out Intersection hit)
        {
            UnityEngine.RaycastHit h;

            Physics.Raycast(Vector3d.ToVector3(ray.origin), new Vector3((float)ray.direction.x, (float)ray.direction.y, (float)ray.direction.z), out h);

            hit = new Intersection
            {
                position = new Vector3d(h.point),
                normal = new Vector3d(h.normal),
                t = h.distance,
            };

            if(h.transform != null && objects.ContainsKey(h.transform.gameObject.GetInstanceID()))
            {
                hit.tempTransform = h.transform;
                hit.bsdf = objects[h.transform.gameObject.GetInstanceID()].bsdf;
                hit.baryCentricCoordinate = h.barycentricCoordinate;
                hit.uv = h.textureCoord;
                hit.f_n = h.normal;
                hit.coordinateSystem(hit.f_n, out hit.f_s, out hit.f_t);
            } else
            {
                return false;
            }

            if(hit.t < 0.001)
            {
                hit.t = 0;

                return false;
            }

            return h.collider != null;
        }
    }
}
