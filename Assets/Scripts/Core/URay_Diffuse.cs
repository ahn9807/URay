using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Diffuse : URay_BSDF
    {
        public Color albedo;

        public URay_Diffuse()
        {
            albedo = new Color(0.7f, 0.5f, 0.3f);
        }

        public override Color Eval(BSDFQueryRecord queryRecord)
        {
            return albedo;
        }
        public override float Pdf(BSDFQueryRecord queryRecord)
        {
            return INV_PI * queryRecord.wo.z;
        }
        public override Color Sample(BSDFQueryRecord queryRecord)
        {
            queryRecord.wo = URay_Sampler.UniformHemisphere();

            return albedo;
        }
        public new bool IsDiffuse() 
        { 
            return true;
        }
    }
}

