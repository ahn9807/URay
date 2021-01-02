using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Material
    {
        public static URay_BSDF ParseBSDFFromMaterial(Material mat)
        {
            if(mat.GetFloat("_Metallic") > 0)
            {
                URay_Mirror returnMaterial = new URay_Mirror();
                returnMaterial.albedo = mat.GetColor("_Color");
                return returnMaterial;
            }
            //Currently we only parse URay_shader..
            if(mat.GetFloat("_Dielectric") != 0)
            {
                URay_Dielectric returnMaterial = new URay_Dielectric();
                returnMaterial.albedo = mat.GetColor("_Color");
                return new URay_Dielectric();
            } else
            {
                URay_Diffuse returnMaterial = new URay_Diffuse();
                returnMaterial.albedo = mat.GetColor("_Color");
                return returnMaterial;
            }
        }
    }
}
