//#define OCTREE

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
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
#if OCTREE
            accerlerationStructure = new URay_Acceleration();
            accerlerationStructure.InitAccelerationStructure(Callback, bounds);
#endif
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
            // Perform a single raycast using RaycastCommand and wait for it to complete
            // Setup the command and result buffers
            //var results = new NativeArray<RaycastHit>(1, Allocator.TempJob);

            //var commands = new NativeArray<RaycastCommand>(1, Allocator.TempJob);

            //// Set the data of the first command
            //Vector3 origin = Vector3.forward * -10;

            //Vector3 direction = Vector3.forward;

            //commands[0] = new RaycastCommand(origin, direction);

            //// Schedule the batch of raycasts
            //JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));

            //// Wait for the batch processing job to complete
            //handle.Complete();

            //// Copy the result. If batchedHit.collider is null there was no hit
            //RaycastHit h = results[0];

            //results.Dispose();
            //commands.Dispose();

            //bool isHit = h.collider != null;

            //hit = new URay_Intersection();
            //hit.normal = h.normal;
            //hit.baryCentricCoordinate = h.barycentricCoordinate;
            //hit.distance = h.distance;
            //if(h.transform != null && h.transform.gameObject != null)
            //    hit.objectID = h.transform.gameObject.GetInstanceID();

#if OCTREE
            bool isHit = URay_Raycast.Raycast(Vector3d.ToVector3(ray.origin), new Vector3((float)ray.direction.x, (float)ray.direction.y, (float)ray.direction.z), out hit);
#else
            bool isHit = URay_Raycast.PhysicsRaycast(Vector3d.ToVector3(ray.origin), new Vector3((float)ray.direction.x, (float)ray.direction.y, (float)ray.direction.z), out hit);
#endif
            if (isHit)
            {
                if(objects.ContainsKey(hit.objectID))
                {
                    hit.bsdf = objects[hit.objectID].bsdf;
                }
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
