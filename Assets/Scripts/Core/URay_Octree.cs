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

            for(int i=0;i<childrenCenters.Length;i++)
            {
                URay_Octree o = new URay_Octree();
                o.parent = parent;
                o.bounds = new Bounds(childrenCenters[i], childrenSize);
                children.Add(o);
                if(generations > 0)
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
            if(this.ContainsTriangle(triangle))
            {
                finalNode = this;
                for(int i=0;i<children.Count;i++)
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

        public bool ContainsTriangle(URay_Triangle triangle)
        {
            return bounds.Contains(triangle.pt0)
                   | bounds.Contains(triangle.pt1)
                   | bounds.Contains(triangle.pt2);
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
}
