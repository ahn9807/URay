using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public abstract class URay_Integrator
    {
        public virtual void Preprocess(URay_Scene scene) { }

        public abstract Color Li(URay_Scene scene, URay_Ray ray, int depth = 0);
    }

}
