using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Mirror : URay_BSDF
    {
        public Color albedo;

        public URay_Mirror()
        {
            albedo = new Color(1f, 1f, 1f);
        }

        public override Color Eval(BSDFQueryRecord queryRecord)
        {
            return Color.black;
        }
        public override float Pdf(BSDFQueryRecord queryRecord)
        {
            return 0f;
        }
        public override Color Sample(BSDFQueryRecord queryRecord)
        {
            if(queryRecord.wi.y <= 0)
            {
                return new Color(0, 0, 0);
            }

            queryRecord.wo = new Vector3(-queryRecord.wi.x, queryRecord.wi.y, -queryRecord.wi.z);

            return albedo;
        }
        public new bool IsDiffuse()
        {
            return false;
        }
    }
}

