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
            hit = new URay_Intersection();
            List<URay_Intersection> hits = new List<URay_Intersection>();

            hits = INTERNAL_RaycastAll(ray);

            hits = SortResults(hits);
            if (hits.Count > 0)
            {
                hit = hits[0];
                return true;
            }
            return false;
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out URay_Intersection hit)
        {
            Ray ray = new Ray(origin, direction);
            hit = new URay_Intersection();
            List<URay_Intersection> hits = new List<URay_Intersection>();

            hits = INTERNAL_RaycastAll(ray);

            hits = SortResults(hits);
            if (hits.Count > 0)
            {
                hit = hits[0];
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

        public static URay_Intersection[] RaycastAll(Ray ray)
        {
            return INTERNAL_RaycastAll(ray).ToArray();
        }

        static List<URay_Intersection> INTERNAL_RaycastAll(Ray ray)
        {
            List<URay_Intersection> hits = new List<URay_Intersection>();
            URay_Octree octree = URay_Acceleration.GetOctree();

            if (octree.bounds.IntersectRay(ray))
            {
                hits = RecurseOctreeBounds(octree, ray);
            }

            hits = SortResults(hits);
            return hits;
        }

        static bool INTERNAL_Raycast(Ray ray, out URay_Intersection hit)
        {
            hit = new URay_Intersection();
            List<URay_Intersection> hits = new List<URay_Intersection>();

            URay_Octree octree = URay_Acceleration.GetOctree();

            if (octree.bounds.IntersectRay(ray))
            {
                hits = RecurseOctreeBounds(octree, ray);
            }

            hits = SortResults(hits);
            if (hits.Count > 0)
            {
                hit = hits[0];
            }
            return hits.Count > 0;
        }

        static List<URay_Intersection> RecurseOctreeBounds(URay_Octree octree, Ray ray)
        {
            List<URay_Intersection> hits = new List<URay_Intersection>();
            float dist = 0f;
            Vector2 baryCoord = new Vector2();
            for (int i = 0; i < octree.children.Count; i++)
            {
                if (octree.children[i].bounds.IntersectRay(ray))
                {
                    for (int k = 0; k < octree.children[i].triangles.Count; k++)
                    {
                        if (TestIntersection(octree.children[i].triangles[k], ray, out dist, out baryCoord))
                        {
                            hits.Add(BuildRaycastHit(octree.children[i].triangles[k], dist, baryCoord));
                        }
                    }
                    hits.AddRange(RecurseOctreeBounds(octree.children[i], ray));
                }
            }
            return hits;
        }

        static URay_Intersection BuildRaycastHit(URay_Triangle hitTriangle, float distance, Vector2 barycentricCoordinate)
        {
            URay_Intersection returnedHit = new URay_Intersection();
            returnedHit.objectID = hitTriangle.objectID;
            returnedHit.distance = distance;
            returnedHit.baryCentricCoordinate = barycentricCoordinate;
            returnedHit.uv = hitTriangle.uv_pt0 + ((hitTriangle.uv_pt1 - hitTriangle.uv_pt0) * barycentricCoordinate.x) + ((hitTriangle.uv_pt2 - hitTriangle.uv_pt0) * barycentricCoordinate.y);
            returnedHit.normal = Vector3.Cross((hitTriangle.pt0 - hitTriangle.pt1), (hitTriangle.pt1 - hitTriangle.pt2));

            //HACK:  Below only returns the center of the hit triangle.  A close approximate, but not accurate.  
            returnedHit.point = hitTriangle.position + (hitTriangle.pt0 + hitTriangle.pt1 + hitTriangle.pt2) / 3;
            return returnedHit;

        }

        /// <summary>
        /// Tests the intersection.
        /// Implementation of the Moller/Trumbore intersection algorithm
        /// </summary>
        /// <returns>
        /// Bool if the ray does intersect
        /// out dist - the distance along the ray at the intersection point
        /// out hitPoint -
        /// </returns>
        /// <param name='triangle'>
        /// If set to <c>true</c> triangle.
        /// </param>
        /// <param name='ray'>
        /// If set to <c>true</c> ray.
        /// </param>
        /// <param name='dist'>
        /// If set to <c>true</c> dist.
        /// </param>
        /// <param name='baryCoord'>
        /// If set to <c>true</c> barycentric coordinate of the intersection point.
        /// </param>
        /// http://www.cs.virginia.edu/~gfx/Courses/2003/ImageSynthesis/papers/Acceleration/Fast%20MinimumStorage%20RayTriangle%20Intersection.pdf
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

        static List<URay_Intersection> SortResults(List<URay_Intersection> input)
        {

            URay_Intersection a = new URay_Intersection();
            URay_Intersection b = new URay_Intersection();
            bool swapped = true;
            while (swapped)
            {
                swapped = false;
                for (int i = 1; i < input.Count; i++)
                {
                    if (input[i - 1].distance > input[i].distance)
                    {
                        a = input[i - 1];
                        b = input[i];
                        input[i - 1] = b;
                        input[i] = a;
                        swapped = true;
                    }
                }
            }

            return input;
        }
    }
}
