using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace URay
{
    public class URay_Main : MonoBehaviour
    {
        public RawImage screenImage;
        public Camera renderCamera;
        public int spp = 100;
        URay_Image uRayImage;

        public void Render()
        { 
            //Image
            float aspectRatio = 16 / 9f;
            int imageWidth = 1080;
            int imageHeight = (int)(imageWidth / aspectRatio);
            uRayImage = new URay_Image(imageWidth, imageHeight, 25, screenImage);
            URay_ImageBlock[] imageBlocks = uRayImage.GetImageBlocks();

            //Camera
            URay_Camera uRay_Camera = new URay_Camera(renderCamera);

            foreach (URay_ImageBlock block in imageBlocks)
            {
                /*
                ThreadStart threadStart = delegate
                {
                    RenderBlock(block, imageWidth, imageHeight, uRay_Camera);
                };

                new Thread(threadStart).Start();
                Can do threading due to raycast is implemented in Unity API Side!!
                We have to fix this problem to accelerate our computation
                */
                RenderBlock(block, imageWidth, imageHeight, uRay_Camera);
            }
        }

        public void RenderBlock(URay_ImageBlock block, int imageWidth, int imageHeight, URay_Camera uRay_Camera)
        {
            for (int y = 0; y < block.height; y++)
            {
                for (int x = 0; x < block.width; x++)
                {
                    Color pixelColor = new Color(0, 0, 0);
                    for (int s = 0; s < spp; s++)
                    {
                        float u = block.GetPixelPosition(x, y).x + Random.Range(0, 1f);
                        float v = block.GetPixelPosition(x, y).y + Random.Range(0, 1f);

                        u /= (imageWidth - 1);
                        v /= (imageHeight - 1);

                        URay_Ray r = uRay_Camera.Sample(u, v);
                        pixelColor += (RayColor(r) / (float)spp);
                    }
                    //pixelColor = new Color(u, v, 0.25f);
                    block.SetPixel(x, y, pixelColor);
                }
            }

            uRayImage.PutImageBlock(block);
            uRayImage.Apply();
        }

        Color RayColor(URay_Ray ray)
        {
            Intersection its;
            bool hit = URay_Scene.RayIntersect(ray, out its);
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
