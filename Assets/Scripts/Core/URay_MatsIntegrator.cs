using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_MatsIntegrator : URay_Integrator
    {
        public override Color Li(URay_Scene scene, URay_Ray ray, int depth = 0)
        {
            URay_Intersection its;
            BSDFQueryRecord bsdfQueryRecord;

            if (depth <= 0)
            {
                return new Color(0, 0, 0);
            }
            bool hit = scene.RayIntersect(ray, out its);
            if (hit)
            {
                bsdfQueryRecord = new BSDFQueryRecord(its.ToLocal(-Vector3d.ToVector3(ray.direction)));
                bsdfQueryRecord.uv = its.uv;

                Color albedo = its.GetBSDF().Sample(bsdfQueryRecord);
                return albedo * Li(scene, new URay_Ray(new Vector3d(its.point), new Vector3d(its.ToWorld(bsdfQueryRecord.wo))), depth - 1);
            }

            //background color
            Vector3d unitDirection = ray.direction.normalized;
            float t = 0.5f * ((float)unitDirection.y + 1f);

            return (1.0f - t) * Color.white + t * new Color(0.5f, 0.7f, 1.0f, 1);
        }
    }

}
