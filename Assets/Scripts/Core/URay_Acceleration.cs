using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Acceleration
    {
        public URay_Octree octree;
        public static URay_Acceleration singleton;
        private int octreeDepth = 3;
        private int objectsPerChunk = 1000;
        public delegate void CallbackMethod();

        public URay_Acceleration()
        {
            singleton = this;
        }

        public void InitAccelerationStructure(CallbackMethod del, Bounds bounds)
        {
            bounds = new Bounds();
            bounds.center = new Vector3(0, 0, 0);
            bounds.extents = new Vector3(10, 10, 10);
            octree = new URay_Octree(bounds, octreeDepth);
            PopulateOctree(del);
        }

        public void PopulateOctree(CallbackMethod del)
        {
            GameObject[] gameObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            GameObject curGO;
            URay_Triangle[] curTris = new URay_Triangle[] { };
            MeshFilter curMeshFilter = null;
            URay_Octree finalNode;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                curGO = gameObjects[i];
                if (curGO == null) continue;
                curMeshFilter = curGO.GetComponent<MeshFilter>();
                if (!curMeshFilter) continue;
                curTris = new URay_Triangle[] { };
                curTris = GetTriangles(curGO);
                for (int k = 0; k < curTris.Length; k++)
                {
                    finalNode = octree.IndexTriangle(curTris[k]);
                    finalNode.AddTriangle(curTris[k]);
                }

                if (i % objectsPerChunk == 1)
                {
                    return;
                }
            }

            del();
            Debug.Log("Created Database");
            Debug.Log("Total Indexed Triangles: " + GetTriangleCount(octree));

        }

        int GetTriangleCount(URay_Octree o)
        {
            int count = 0;
            count = o.triangles.Count;
            foreach (URay_Octree oct in o.children)
            {
                count += GetTriangleCount(oct);
            }
            return count;
        }

        URay_Triangle[] GetTriangles(GameObject go)
        {
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            int[] vIndex = mesh.triangles;
            Vector3[] verts = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            List<URay_Triangle> triangleList = new List<URay_Triangle>();
            int i = 0;
            while (i < vIndex.Length)
            {
                if (uvs.Length != 0)
                {
                    triangleList.Add(
                    new URay_Triangle(
                    verts[vIndex[i + 0]],
                    verts[vIndex[i + 1]],
                    verts[vIndex[i + 2]],
                    uvs[vIndex[i + 0]],
                    uvs[vIndex[i + 1]],
                    uvs[vIndex[i + 2]],
                    go.transform));
                }
                else
                {
                    triangleList.Add(
                    new URay_Triangle(
                    verts[vIndex[i + 0]],
                    verts[vIndex[i + 1]],
                    verts[vIndex[i + 2]],
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 0),
                    go.transform));
                }

                i += 3;
            }
            return triangleList.ToArray();
        }

        //This is slow and really not necessary, just a nice visual
        public void DrawOctree(URay_Octree oct)
        {
            Gizmos.DrawWireCube(oct.bounds.center, oct.bounds.size);

            foreach (URay_Octree o in oct.children)
            {
                DrawOctree(o);
            }
        }

        public static URay_Octree GetOctree()
        {
            return singleton.octree;
        }

        //Get an approximation of memory usage out of the garbage collector while purposely clearing out the tree
        void Destroy()
        {
            Debug.Log("Mem Before Clear: " + System.GC.GetTotalMemory(true) / 1024f / 1024f);
            octree.Clear();
            octree = null;
            Debug.Log("Mem After Clear: " + System.GC.GetTotalMemory(true) / 1024f / 1024f);
        }
    }
}
