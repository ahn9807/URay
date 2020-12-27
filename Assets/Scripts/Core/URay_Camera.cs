using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Camera
    {
        public Vector3 origin;

        Vector3 lowerLeftCorner;
        Vector3 horizontal;
        Vector3 vertical;

        public URay_Camera(Camera renderCamera)
        {
            float aspectRatio = renderCamera.aspect;
            float viewPortHeight = 2.0f;
            float viewPortWidth = viewPortHeight * aspectRatio;
            float focalLength = 1.0f;

            this.origin = renderCamera.transform.position;
            this.horizontal = new Vector3(viewPortWidth, 0, 0);
            this.vertical = new Vector3(0, viewPortHeight, 0);
            this.lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - new Vector3(0, 0, focalLength);
        }

        public URay_Ray Sample(float u, float v)
        {
            return new URay_Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
        }
    }

}
