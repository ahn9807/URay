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

        public bool RayIntersect(URay_Ray ray, out URay_Intersection hit)
        {
            UnityEngine.RaycastHit h;

            Physics.Raycast(Vector3d.ToVector3(ray.origin), new Vector3((float)ray.direction.x, (float)ray.direction.y, (float)ray.direction.z), out h);

            hit = new URay_Intersection
            {
                position = new Vector3d(h.point),
                normal = new Vector3d(h.normal),
                t = h.distance,
            };


            if (h.transform != null && objects.ContainsKey(h.transform.gameObject.GetInstanceID()))
            {
                hit.tempTransform = h.transform;
                hit.bsdf = objects[h.transform.gameObject.GetInstanceID()].bsdf;
                hit.baryCentricCoordinate = h.barycentricCoordinate;
                hit.uv = h.textureCoord;
                hit.f_n = h.normal;
                //If object has mesh collider.. calculate normal for shading
                //Calcualte f_n for shading
                /*
                if ((h.collider as MeshCollider).sharedMesh != null)
                {
                    Mesh mesh = (h.collider as MeshCollider).sharedMesh;
                    Vector3[] normals = mesh.normals;
                    int[] triangles = mesh.triangles;

                    // Extract local space normals of the triangle we hit
                    Vector3 n0 = normals[triangles[h.triangleIndex * 3 + 0]];
                    Vector3 n1 = normals[triangles[h.triangleIndex * 3 + 1]];
                    Vector3 n2 = normals[triangles[h.triangleIndex * 3 + 2]];

                    // interpolate using the barycentric coordinate of the hitpoint
                    Vector3 baryCenter = h.barycentricCoordinate;

                    // Use barycentric coordinate to interpolate normal
                    Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
                    // normalize the interpolated normal
                    hit.f_n = interpolatedNormal.normalized;
                }
                */

                hit.OBSystem(hit.f_n, out hit.f_s, out hit.f_t);
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
