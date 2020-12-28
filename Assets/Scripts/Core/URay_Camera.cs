using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URay
{
    public class URay_Camera
    {
        public Vector3d origin;

        Vector3d lowerLeftCorner;
        Vector3d horizontal;
        Vector3d vertical;

        public URay_Camera(Camera renderCamera)
        {
            double aspectRatio = renderCamera.aspect;
            double viewPortHeight = 2.0f;
            double viewPortWidth = viewPortHeight * aspectRatio;
            double focalLength = 1.0f;

            this.origin = new Vector3d(renderCamera.transform.position);
            this.horizontal = new Vector3d(viewPortWidth, 0, 0);
            this.vertical = new Vector3d(0, viewPortHeight, 0);
            this.lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - new Vector3d(0, 0, focalLength);
        }

        public URay_Ray Sample(double u, double v)
        {
            Vector3d direction = lowerLeftCorner + u * horizontal + v * vertical - origin;
            return new URay_Ray(origin, new Vector3d(direction.x, direction.y, -direction.z));
        }
    }

}
