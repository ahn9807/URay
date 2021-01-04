using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_LinearAcceleration : URay_Acceleration
    {
        Mesh mesh;

        public override void AddMesh(Mesh mesh)
        {
            if(this.mesh == null)
            {
                this.mesh = mesh;
                return;
            }

            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = this.mesh;
            combine[1].mesh = mesh;
            Mesh meshc = new Mesh();
            meshc.CombineMeshes(combine);
            this.mesh = meshc;
        }

        public override void Build()
        {
            throw new System.NotImplementedException();
        }

        public override bool RayIntersect(URay_Ray ray, out URay_Intersection its, bool shadowRay)
        {
            bool foundIntersection = false;
            uint face = 0;
            its = new URay_Intersection();

            for(int i=0;i<mesh.triangles.Length;i += 3)
            {
                float u, v, t;
                if(MeshIntersection(mesh, i, ray, out u, out v, out t))
                {
                    if(shadowRay)
                    {
                        return true;
                    }

                    its.baryCentricCoordinate = new Vector2(u, v);
                    its.distance = t;
                    its.mesh = mesh;
                    face = (uint)i;
                    foundIntersection = true;
                }
            }

            if(foundIntersection)
            {
                Vector3 bary = new Vector3(1 - (its.uv.x + its.uv.y), its.uv.x, its.uv.y);
                int idx0 = mesh.triangles[face];
                int idx1 = mesh.triangles[face + 1];
                int idx2 = mesh.triangles[face + 2];

                Vector3 p0 = mesh.vertices[idx0];
                Vector3 p1 = mesh.vertices[idx1];
                Vector3 p2 = mesh.vertices[idx2];

                its.point = ray.origin + ray.direction * its.distance;
                its.uv = bary.x * mesh.uv[idx0] + bary.y * mesh.uv[idx1] + bary.z * mesh.uv[idx2];
                its.normal = bary.x * mesh.normals[idx0] + bary.y * mesh.normals[idx1] + bary.z * mesh.normals[idx2];

                return true;
            } else
            {
                return false;
            }
        }
    }

}
