using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Octree
    {
        public List<URay_Octree> children;
        public URay_Octree parent;
        public Bounds bounds;
        public List<URay_Triangle> triangles;

        public URay_Octree()
        {
            this.children = new List<URay_Octree>();
            this.triangles = new List<URay_Triangle>();
            this.parent = null;
        }

        public URay_Octree(Bounds parentBounds, int generations)
        {
            this.bounds = parentBounds;
            this.children = new List<URay_Octree>();
            this.triangles = new List<URay_Triangle>();
            this.parent = null;
            CreateChildren(this, generations);
        }

        public void BuildOctree(List<URay_Triangle> mesh, Bounds bounds, int generations)
        {
            triangles = new List<URay_Triangle>();
            this.bounds = bounds;
            foreach (var tri in mesh)
            {
                if (OverlapTriangle(tri))
                {
                    triangles.Add(tri);
                }
            }

            children = new List<URay_Octree>();
            Vector3 c = bounds.center;
            float u = bounds.extents.x * 0.5f;
            float v = bounds.extents.y * 0.5f;
            float w = bounds.extents.z * 0.5f;
            Vector3 childrenSize = bounds.extents;
            Vector3[] childrenCenters = {
            new Vector3(c.x + u, c.y + v, c.z + w),
            new Vector3(c.x + u, c.y + v, c.z - w),
            new Vector3(c.x + u, c.y - v, c.z + w),
            new Vector3(c.x + u, c.y - v, c.z - w),
            new Vector3(c.x - u, c.y + v, c.z + w),
            new Vector3(c.x - u, c.y + v, c.z - w),
            new Vector3(c.x - u, c.y - v, c.z + w),
            new Vector3(c.x - u, c.y - v, c.z - w)
            };

            for (int i = 0; i < childrenCenters.Length; i++)
            {
                URay_Octree o = new URay_Octree();
                o.parent = parent;
                o.bounds = new Bounds(childrenCenters[i], childrenSize);
                children.Add(o);
                if (generations > 0)
                {
                    o.BuildOctree(triangles, bounds, generations - 1);
                }
            }
        }

        protected void CreateChildren(URay_Octree parent, int generations)
        {
            children = new List<URay_Octree>();
            Vector3 c = parent.bounds.center;
            float u = parent.bounds.extents.x * 0.5f;
            float v = parent.bounds.extents.y * 0.5f;
            float w = parent.bounds.extents.z * 0.5f;
            Vector3 childrenSize = parent.bounds.extents;
            Vector3[] childrenCenters = {
                new Vector3(c.x + u, c.y + v, c.z + w),
                new Vector3(c.x + u, c.y + v, c.z - w),
                new Vector3(c.x + u, c.y - v, c.z + w),
                new Vector3(c.x + u, c.y - v, c.z - w),
                new Vector3(c.x - u, c.y + v, c.z + w),
                new Vector3(c.x - u, c.y + v, c.z - w),
                new Vector3(c.x - u, c.y - v, c.z + w),
                new Vector3(c.x - u, c.y - v, c.z - w)
            };

            for (int i = 0; i < childrenCenters.Length; i++)
            {
                URay_Octree o = new URay_Octree();
                o.parent = parent;
                o.bounds = new Bounds(childrenCenters[i], childrenSize);
                children.Add(o);

                if (generations > 0)
                {
                    o.CreateChildren(o, generations - 1);
                }
            }
        }

        public URay_Octree IndexTriangle(URay_Triangle t)
        {
            return IndexTriangle(this, t);
        }

        public URay_Octree IndexTriangle(URay_Octree parentNode, URay_Triangle triangle)
        {
            URay_Octree finalNode = parentNode;
            if (this.ContainsTriangle(triangle))
            {
                finalNode = this;
                for (int i = 0; i < children.Count; i++)
                {
                    finalNode = children[i].IndexTriangle(this, triangle);
                    if (finalNode != this) return finalNode;
                }
                return finalNode;
            }
            return finalNode;
        }

        public bool AddTriangle(URay_Triangle t)
        {
            triangles.Add(t);
            return true;
        }

        public bool OverlapTriangle(URay_Triangle triangle)
        {
            return bounds.Contains(triangle.pt0)
               || bounds.Contains(triangle.pt1)
               || bounds.Contains(triangle.pt2);
        }

        public bool ContainsTriangle(URay_Triangle triangle)
        {
            return bounds.Contains(triangle.pt0)
                   && bounds.Contains(triangle.pt1)
                   && bounds.Contains(triangle.pt2);
        }

        public void Clear()
        {
            int total = ClearOctree(this);
            Debug.Log("Total Nodes Cleared: " + total);
        }

        protected int ClearOctree(URay_Octree o)
        {
            int count = 0;
            for (int i = 0; i < o.children.Count; i++)
            {
                count += ClearOctree(o.children[i]);
            }
            o.triangles.Clear();
            o.triangles.TrimExcess();
            o.parent = null;
            o.children.Clear();
            o.children.TrimExcess();
            count++;
            return count;
        }
    }

    public class URay_OctreeAcceleration
    {
        public URay_Octree octree;
        public static URay_OctreeAcceleration singleton;
        private int octreeDepth = 4;
        private int objectsPerChunk = 1000;
        public delegate void CallbackMethod();

        public URay_OctreeAcceleration()
        {
            singleton = this;
        }

        public void InitAccelerationStructure(CallbackMethod del, Bounds bounds)
        {
            bounds = new Bounds();
            bounds.center = new Vector3(0, 0, 0);
            bounds.extents = new Vector3(10, 10, 10);
            octree = new URay_Octree(bounds, octreeDepth);
            BuildOctree(del);
        }

        public void BuildOctree(CallbackMethod del)
        {
            GameObject[] gameObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            GameObject curGO;
            URay_Triangle[] curTris;
            MeshFilter curMeshFilter;
            URay_Octree finalNode;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                curGO = gameObjects[i];
                if (curGO == null) continue;
                curMeshFilter = curGO.GetComponent<MeshFilter>();
                if (!curMeshFilter) continue;
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
            Vector3[] normals = mesh.normals;
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
                    normals[vIndex[i + 0]],
                    normals[vIndex[i + 1]],
                    normals[vIndex[i + 2]],
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
                    normals[vIndex[i + 0]],
                    normals[vIndex[i + 1]],
                    normals[vIndex[i + 2]],
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

    public class URay_OctreeRaycast
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

            if (hit == null)
            {
                hit = new URay_Intersection();
                hit.point = new Vector3(0, 0, 0);
            }

            if (hit != null)
            {
                return true;
            }
            return false;
        }

        public static bool PhysicsRaycast(Vector3 origin, Vector3 direction, out URay_Intersection uhit)
        {
            origin = origin + direction * 0.00001f;
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
            URay_Octree octree = URay_OctreeAcceleration.GetOctree();

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
            //if (octree.children.Count == 0)
            {
                if (octree.triangles.Count != 0)
                {
                    for (int k = 0; k < octree.triangles.Count; k++)
                    {
                        if (TriangleIntersection(octree.triangles[k], ray, out float curDist, out Vector2 baryCoord))
                        {
                            if (curDist < dist)
                            {
                                hit = BuildRaycastHit(octree.triangles[k], ray, curDist, baryCoord);
                                dist = curDist;
                            }
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

        static URay_Intersection BuildRaycastHit(URay_Triangle hitTriangle, Ray ray, float distance, Vector2 barycentricCoordinate)
        {
            URay_Intersection returnedHit = new URay_Intersection();
            returnedHit.objectID = hitTriangle.objectID;
            returnedHit.distance = distance;
            returnedHit.baryCentricCoordinate = barycentricCoordinate;
            returnedHit.uv = hitTriangle.uv_pt0 + ((hitTriangle.uv_pt1 - hitTriangle.uv_pt0) * barycentricCoordinate.x) + ((hitTriangle.uv_pt2 - hitTriangle.uv_pt0) * barycentricCoordinate.y);
            returnedHit.normal = (hitTriangle.n_pt0 + hitTriangle.n_pt1 + hitTriangle.n_pt2) / 3.0f;

            //HACK:  Below only returns the center of the hit triangle.  A close approximate, but not accurate.  
            returnedHit.point = ray.origin + ray.direction * distance;
            return returnedHit;
        }

        static bool TriangleIntersection(URay_Triangle triangle, Ray ray, out float dist, out Vector2 baryCoord)
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
