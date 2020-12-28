using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Material
    {
        public static URay_BSDF ParseBSDFFromMaterial(Material mat)
        {
            //Currently we only parse URay_shader..
            if(mat.GetFloat("_Dielectric") != 0)
            {
                return new URay_Mirror();
            } else
            {
                return new URay_Diffuse();
            }
        }
    }

}
