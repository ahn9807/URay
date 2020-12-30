using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Object
    {
        public int objectID;
        public URay_BSDF bsdf;
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        public Vector3[] normals;

        public URay_Object(int id, Mesh mesh, URay_BSDF bsdf)
        {
            this.objectID = id;
            this.bsdf = bsdf;
            if(mesh.uv != null)
            {
                this.uvs = mesh.uv;
            }
            if(mesh.triangles != null)
            {
                this.triangles = mesh.triangles;
            }
            if(mesh.vertices != null)
            {
                this.vertices = mesh.vertices;
            }
            if(mesh.normals != null)
            {
                this.normals = mesh.normals;
            }
        }
    }
}
