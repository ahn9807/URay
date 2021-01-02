using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Dielectric : URay_BSDF
    {
        public float int_IOR;
        public float ext_IOR;
        public Color albedo;

        public URay_Dielectric()
        {
            albedo = new Color(1f, 1f, 1f);
            int_IOR = 1.5046f;
            ext_IOR = 1.0000277f;
        }

        public override Color Eval(BSDFQueryRecord queryRecord)
        {
            return new Color(0f,0f,0f);
        }
        public override float Pdf(BSDFQueryRecord queryRecord)
        {
            return 0f;
        }
        public override Color Sample(BSDFQueryRecord queryRecord)
        {
            Vector3 normal = new Vector3(0, 1f, 0);
            Vector3 lightIn = -queryRecord.wi.normalized;
            float reflectivity;
            float ni = int_IOR;
            float np = ext_IOR;
            float cosine;

            //When light comes from inside
            if(Vector3.Dot(lightIn, normal) > 0)
            {
                normal = -normal;
            } else
            {
                var temp = ni;
                ni = np;
                np = temp;
            }

            //reflectivity = fresnel(-Vector3.Dot(-lightIn, normal), np, ni);

            reflectivity = (1 - ni / np) / (1 + ni / np);
            reflectivity *= reflectivity;
            cosine = -Vector3.Dot(lightIn, normal);
            reflectivity += (1.0f - reflectivity) * Mathf.Pow((1 - cosine), 5);

            if (!Refraction(lightIn, normal, ni / np, ref queryRecord.wo))
                reflectivity = 1;

            if(URay_Sampler.UniformNumber() < reflectivity)
            {
                Reflection(lightIn, normal, ref queryRecord.wo);
            }

            return albedo;
        }
        public new bool IsDiffuse()
        {
            return true;
        }

        private float fresnel(float cosThetaI, float extIOR, float intIOR)
        {
            float etaI = extIOR, etaT = intIOR;

            if (extIOR == intIOR)
                return 0.0f;

            /* Swap the indices of refraction if the interaction starts
               at the inside of the object */
            if (cosThetaI < 0.0f)
            {
                var temp = etaI;
                etaI = etaT;
                etaT = temp;
                cosThetaI = -cosThetaI;
            }

            /* Using Snell's law, calculate the squared sine of the
               angle between the normal and the transmitted ray */
            float eta = etaI / etaT,
                  sinThetaTSqr = eta * eta * (1 - cosThetaI * cosThetaI);

            if (sinThetaTSqr > 1.0f)
                return 1.0f;  /* Total internal reflection! */

            float cosThetaT = Mathf.Sqrt(1.0f - sinThetaTSqr);

            float Rs = (etaI * cosThetaI - etaT * cosThetaT)
                     / (etaI * cosThetaI + etaT * cosThetaT);
            float Rp = (etaT * cosThetaI - etaI * cosThetaT)
                     / (etaT * cosThetaI + etaI * cosThetaT);

            return (Rs * Rs + Rp * Rp) / 2.0f;
        }

        private bool Reflection(Vector3 p, Vector3 n, ref Vector3 result)
        {
            result = p - 2.0f * Vector3.Dot(n, p) * n;
            return true;
        }

        private bool Refraction(Vector3 p, Vector3 n, float ninp, ref Vector3 result)
        {
            Vector3 np = p.normalized;
            float dt = Vector3.Dot(np, n);
            float isRefracted = 1.0f - ninp * ninp * (1 - dt * dt);
            if(isRefracted < 0)
            {
                return false;
            } else
            {
                result = ninp * (np - n * dt) - n * Mathf.Sqrt(isRefracted);
                return true;
            }
        }
    }
}
