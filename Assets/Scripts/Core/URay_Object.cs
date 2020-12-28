using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Object
    {
        public int objectID;
        public URay_BSDF bsdf;

        public URay_Object(int id, URay_BSDF bsdf)
        {
            this.objectID = id;
            this.bsdf = bsdf;
        }
    }
}
