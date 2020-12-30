using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Scene
    {
        private Dictionary<int, URay_Object> objects;
        public URay_Integrator integrator;
        public URay_Acceleration accerlerationStructure;
        public Bounds bounds;

        public URay_Scene()
        {
            objects = new Dictionary<int, URay_Object>();
            integrator = new URay_MatsIntegrator();
            //accerlerationStructure = new URay_Acceleration();
            //accerlerationStructure.InitAccelerationStructure(Callback, bounds);
        }

        public void Callback()
        {
            Debug.Log("Completed Dictionary Build and Object Generation in ");
        }

        public void AddObject(GameObject gameObject)
        {
            //only add object with mesh
            if(gameObject.GetComponent<MeshRenderer>() != null)
            {
                URay_BSDF newObjectBSDF = URay_Material.ParseBSDFFromMaterial(gameObject.GetComponent<MeshRenderer>().sharedMaterial);
                URay_Object newURayObject = new URay_Object(gameObject.GetInstanceID(), gameObject.GetComponent<MeshFilter>().sharedMesh, newObjectBSDF);
                objects.Add(gameObject.GetInstanceID(), newURayObject);
            }
        }

        public URay_Integrator GetIntegrator()
        {
            return integrator;
        }

        public bool RayIntersect(URay_Ray ray, out URay_Intersection hit)
        {
            //bool isHit = URay_Raycast.Raycast(Vector3d.ToVector3(ray.origin), new Vector3((float)ray.direction.x, (float)ray.direction.y, (float)ray.direction.z), out hit);
            bool isHit = URay_Raycast.PhysicsRaycast(Vector3d.ToVector3(ray.origin), new Vector3((float)ray.direction.x, (float)ray.direction.y, (float)ray.direction.z), out hit);
            if (isHit)
            {
                hit.bsdf = objects[hit.objectID].bsdf;
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
                hit.f_n = hit.normal;
                hit.OBSystem(hit.f_n, out hit.f_s, out hit.f_t);
            } else
            {
                return false;
            }

            if(hit.distance < 0.001)
            {
                hit.distance = 0;

                return false;
            }

            return true;
        }
    }

    
}
