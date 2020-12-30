using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Raycast
    {

        static Vector3 edge1 = new Vector3();
        static Vector3 edge2 = new Vector3();
        static Vector3 tVec = new Vector3();
        static Vector3 pVec = new Vector3();
        static Vector3 qVec = new Vector3();

        static float det = 0;
        static float invDet = 0;
        static float u = 0;
        static float v = 0;

        static float epsilon = 0.0001f;

        public static bool Raycast(Ray ray, out URay_Intersection hit)
        {
            return Raycast(ray.origin, ray.direction, out hit);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out URay_Intersection hit)
        {
            Ray ray = new Ray(origin, direction);

            hit = INTERNAL_Raycast(ray);

            if (hit != null)
            {
                return true;
            }
            return false;
        }

        public static bool PhysicsRaycast(Vector3 origin, Vector3 direction, out URay_Intersection uhit)
        {
            bool isHit = Physics.Raycast(origin, direction, out RaycastHit hit);

            uhit = new URay_Intersection();
            uhit.normal = hit.normal;
            uhit.point = hit.point;
            uhit.baryCentricCoordinate = hit.barycentricCoordinate;
            if (hit.transform != null)
                uhit.objectID = hit.transform.gameObject.GetInstanceID();
            else
                uhit.objectID = -1;
            uhit.distance = hit.distance;
            uhit.uv = hit.textureCoord;

            return isHit;
        }

        static URay_Intersection INTERNAL_Raycast(Ray ray)
        {
            URay_Intersection hit = null;
            URay_Octree octree = URay_Acceleration.GetOctree();

            if (octree.bounds.IntersectRay(ray))
            {
                SearchOctree(octree, ray, ref hit);
            }

            return hit;
        }

        static void SearchOctree(URay_Octree octree, Ray ray, ref URay_Intersection hit)
        {
            SearchOctree(octree, ray, ref hit, float.MaxValue);
        }

        static void SearchOctree(URay_Octree octree, Ray ray, ref URay_Intersection hit, float dist)
        {
            //If Node is Leaf Node
            if (octree.triangles.Count != 0)
            {
                for (int k = 0; k < octree.triangles.Count; k++)
                {
                    if (TestIntersection(octree.triangles[k], ray, out float curDist, out Vector2 baryCoord))
                    {
                        if (curDist < dist)
                        {
                            hit = BuildRaycastHit(octree.triangles[k], curDist, baryCoord);
                            dist = curDist;
                        }
                    }
                }
            }

            for (int i = 0; i < octree.children.Count; i++)
            {
                if (octree.children[i].bounds.IntersectRay(ray))
                {
                    SearchOctree(octree.children[i], ray, ref hit, dist);
                }
            }
        }

        static URay_Intersection BuildRaycastHit(URay_Triangle hitTriangle, float distance, Vector2 barycentricCoordinate)
        {
            URay_Intersection returnedHit = new URay_Intersection();
            returnedHit.objectID = hitTriangle.objectID;
            returnedHit.distance = distance;
            returnedHit.baryCentricCoordinate = barycentricCoordinate;
            returnedHit.uv = hitTriangle.uv_pt0 + ((hitTriangle.uv_pt1 - hitTriangle.uv_pt0) * barycentricCoordinate.x) + ((hitTriangle.uv_pt2 - hitTriangle.uv_pt0) * barycentricCoordinate.y);
            returnedHit.normal = -(hitTriangle.n_pt0 + hitTriangle.n_pt1 + hitTriangle.n_pt2) / 3;

            //HACK:  Below only returns the center of the hit triangle.  A close approximate, but not accurate.  
            returnedHit.point = hitTriangle.position + (hitTriangle.pt0 + hitTriangle.pt1 + hitTriangle.pt2) / 3;
            return returnedHit;

        }

        static bool TestIntersection(URay_Triangle triangle, Ray ray, out float dist, out Vector2 baryCoord)
        {
            baryCoord = Vector2.zero;
            dist = Mathf.Infinity;
            edge1 = triangle.pt1 - triangle.pt0;
            edge2 = triangle.pt2 - triangle.pt0;

            pVec = Vector3.Cross(ray.direction, edge2);
            det = Vector3.Dot(edge1, pVec);
            if (det < epsilon)
            {
                return false;
            }
            tVec = ray.origin - triangle.pt0;
            u = Vector3.Dot(tVec, pVec);
            if (u < 0 || u > det)
            {
                return false;
            }
            qVec = Vector3.Cross(tVec, edge1);
            v = Vector3.Dot(ray.direction, qVec);
            if (v < 0 || u + v > det)
            {
                return false;
            }
            dist = Vector3.Dot(edge2, qVec);
            invDet = 1 / det;
            dist *= invDet;
            baryCoord.x = u * invDet;
            baryCoord.y = v * invDet;
            return true;
        }
    }
}
