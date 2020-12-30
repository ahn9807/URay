using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Triangle
    {
        public Vector3 pt0;
        public Vector3 pt1;
        public Vector3 pt2;

        public Vector2 uv_pt0;
        public Vector2 uv_pt1;
        public Vector2 uv_pt2;

        public Vector3 n_pt0;
        public Vector3 n_pt1;
        public Vector3 n_pt2;

        public Vector3 position;
        public int objectID;

        public URay_Triangle(Vector3 pt0, Vector3 pt1, Vector3 pt2, Vector2 uv_pt0, Vector2 uv_pt1, Vector2 uv_pt2, Vector3 n_pt0, Vector3 n_pt1, Vector3 n_pt2, Transform trans)
        {
            this.pt0 = pt0;
            this.pt1 = pt1;
            this.pt2 = pt2;
            this.uv_pt0 = uv_pt0;
            this.uv_pt1 = uv_pt1;
            this.uv_pt2 = uv_pt2;
            this.n_pt0 = n_pt0;
            this.n_pt1 = n_pt1;
            this.n_pt2 = n_pt2;
            this.position = trans.position;
            this.objectID = trans.gameObject.GetInstanceID();
            UpdateVerts(trans);
        }

        public void UpdateVerts(Transform trans)
        {
            pt0 = trans.TransformPoint(pt0);
            pt1 = trans.TransformPoint(pt1);
            pt2 = trans.TransformPoint(pt2);
            n_pt0 = trans.TransformDirection(n_pt0);
            n_pt1 = trans.TransformDirection(n_pt1);
            n_pt2 = trans.TransformDirection(n_pt2);
        }
    }
}
