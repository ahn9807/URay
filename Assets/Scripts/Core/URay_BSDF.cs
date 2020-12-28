using UnityEditor;
using UnityEngine;

namespace URay
{
    public abstract class URay_BSDF
    {
        public static float INV_PI = 1.0f / Mathf.PI;
        public abstract Color Eval(BSDFQueryRecord queryRecord);
        public abstract float Pdf(BSDFQueryRecord queryRecord);
        public abstract Color Sample(BSDFQueryRecord queryRecord);
        public bool IsDiffuse() { return false; }
    }

    public class BSDFQueryRecord
    {
        //incident direction in local space
        public Vector3 wi;
        //outgoing direction in local space
        public Vector3 wo;
        //UV
        public Vector2 uv;

        public BSDFQueryRecord(Vector3 wi)
        {
            this.wi = wi;
        }

        public BSDFQueryRecord(Vector3 wi, Vector3 wo)
        {
            this.wi = wi;
            this.wo = wo;
        }
    }
}