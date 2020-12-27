using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace URay
{
    public class URayMain : MonoBehaviour
    {
        public RawImage screenImage;
        public Camera renderCamera;
        public GameObject tempSphere;
        Image uRayImage;

        Texture2D mainTexture;
        List<Color> colors;

        public void Render()
        { 
            //Image
            float aspectRatio = 16 / 9f;
            int imageWidth = 1080;
            int imageHeight = (int)(imageWidth / aspectRatio);
            uRayImage = new Image(imageWidth, imageHeight, 25, screenImage);
            ImageBlock[] imageBlocks = uRayImage.GetImageBlocks();

            //Camera
            float viewPortHeight = 2f;
            float viewPortWidth = viewPortHeight * aspectRatio;
            float focalLength = 1.0f;

            Vector3 origin = renderCamera.transform.position;
            Vector3 horizontal = new Vector3(viewPortWidth, 0, 0);
            Vector3 vertical = new Vector3(0, viewPortHeight, 0);
            Vector3 lowerLeftCorner = origin - horizontal / 2 - vertical / 2 - new Vector3(0, 0, focalLength);

            foreach (ImageBlock block in imageBlocks)
            {
                for (int y = 0; y < block.height; y++)
                {
                    for (int x = 0; x < block.width; x++)
                    {
                        float u = block.GetPixelPosition(x, y).x;
                        float v = block.GetPixelPosition(x, y).y;
                        u /= (imageWidth - 1);
                        v /= (imageHeight - 1);
                        Ray r = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
                        Color pixelColor = RayColor(r);
                        //pixelColor = new Color(u, v, 0.25f);
                        block.SetPixel(x, y, pixelColor);
                    }
                }

                uRayImage.PutImageBlock(block);
            }
            uRayImage.Apply();
        }

        bool HitSphere(Vector3 center, float radius, Ray r)
        {
            Vector3 oc = r.origin - center;
            float a = Vector3.Dot(r.direction, r.direction);
            float b = 2.0f * Vector3.Dot(oc, r.direction);
            float c = Vector3.Dot(oc, oc) - radius * radius;
            float discriminant = b * b - 4 * a * c;
            return discriminant > 0;
        }

        Color RayColor(Ray ray)
        {
            Intersection its;
            bool hit = Scene.RayIntersect(ray, out its);
            if(hit)
            {
                Vector3 color = 0.5f * (its.normal + Vector3.one);
                return new Color(color.x, color.y, color.z);
            }

            Vector3 unitDirection = ray.direction.normalized;
            float t = 0.5f * (unitDirection.y + 1f);
            return (1.0f - t) * Color.white + t * new Color(0.5f, 0.7f, 1.0f, 1);
        }
    }
}
