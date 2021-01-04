using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public abstract class URay_Acceleration
    {
        public abstract void AddMesh(Mesh mesh);
        public abstract void Build();
        public abstract bool RayIntersect(URay_Ray ray, out URay_Intersection its, bool shadowRay);
        public static bool MeshIntersection(Mesh mesh, int triIndex, URay_Ray ray, out float ret_u, out float ret_v, out float dist)
        {
            Vector3 pt0 = mesh.vertices[mesh.triangles[triIndex]];
            Vector3 pt1 = mesh.vertices[mesh.triangles[triIndex + 1]];
            Vector3 pt2 = mesh.vertices[mesh.triangles[triIndex + 2]];

            ret_u = 0f;
            ret_v = 0f;
            dist = Mathf.Infinity;
            Vector3 edge1 = pt1 - pt0;
            Vector3 edge2 = pt2 - pt0;

            Vector3 pVec = Vector3.Cross(ray.direction, edge2);
            float det = Vector3.Dot(edge1, pVec);
            if (det < Mathf.Epsilon)
            {
                return false;
            }
            Vector3 tVec = ray.origin - pt0;
            float u = Vector3.Dot(tVec, pVec);
            if (u < 0 || u > det)
            {
                return false;
            }
            Vector3 qVec = Vector3.Cross(tVec, edge1);
            float v = Vector3.Dot(ray.direction, qVec);
            if (v < 0 || u + v > det)
            {
                return false;
            }
            dist = Vector3.Dot(edge2, qVec);
            float invDet = 1 / det;
            dist *= invDet;
            ret_u = u * invDet;
            ret_v = v * invDet;

            return true;
        }
    }
}
