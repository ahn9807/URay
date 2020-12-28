using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Unity.Jobs;

namespace URay
{
    public class URay_Main : MonoBehaviour
    {
        public RawImage screenImage;
        public Camera renderCamera;
        public int spp = 100;
        URay_Image uRayImage;
        URay_Scene uRayScene;

        public void Render()
        {
            //Image
            float aspectRatio = renderCamera.aspect;
            int imageWidth = 1080;
            int imageHeight = (int)(imageWidth / aspectRatio);
            screenImage.texture = null;
            uRayImage = new URay_Image(imageWidth, imageHeight, 25, screenImage);
            uRayScene = new URay_Scene();
            URay_ImageBlock[] imageBlocks = uRayImage.GetImageBlocks();

            foreach(GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
            {
                uRayScene.AddObject(gameObject);
            }

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
                Can't do threading due to raycast is implemented in Unity API Side!!
                We have to fix this problem to accelerate our computations
                Also we can't use unity job system due to the same reason
                So We habe to make our own raycast system and corresponding accleration system..
                */
                //This code is temporary code. Use unity's default raycast system with collider
                StartCoroutine(IERenderBlock(uRayScene, block, imageWidth, imageHeight, uRay_Camera));
            }
        }

        public void RenderBlock(URay_Scene scene, URay_ImageBlock block, int imageWidth, int imageHeight, URay_Camera uRay_Camera)
        {
            for (int y = 0; y < block.height; y++)
            {
                for (int x = 0; x < block.width; x++)
                {
                    Color pixelColor = new Color(0, 0, 0);
                    for (int s = 0; s < spp; s++)
                    {
                        double u = block.GetPixelPosition(x, y).x + URay_Sampler.UniformNumber();
                        double v = block.GetPixelPosition(x, y).y + URay_Sampler.UniformNumber();

                        u /= (imageWidth - 1);
                        v /= (imageHeight - 1);

                        URay_Ray r = uRay_Camera.Sample(u, v);
                        Color col = (PathBruteIntegration(r, scene, 10) / (float)spp);
                        float scale = 1.0f / spp;
                        pixelColor += new Color(Mathf.Sqrt(scale * col.r), Mathf.Sqrt(scale * col.g), Mathf.Sqrt(scale * col.b));
                    }
                    //pixelColor = new Color(u, v, 0.25f);

                    block.SetPixel(x, y, pixelColor);
                }
            }

            uRayImage.PutImageBlock(block);
        }

        IEnumerator IERenderBlock(URay_Scene scene, URay_ImageBlock block, int imageWdith, int imageHeight, URay_Camera uRay_Camera)
        {
            RenderBlock(scene, block, imageWdith, imageHeight, uRay_Camera);
            uRayImage.Apply();
            yield return null;
        }

        Color PathBruteIntegration(URay_Ray ray, URay_Scene scene, int depth)
        {
            URay_Intersection its;
            BSDFQueryRecord bsdfQueryRecord;

            if(depth <= 0)
            {
                return new Color(0, 0, 0);
            }
            bool hit = scene.RayIntersect(ray, out its);
            if(hit)
            {
                bsdfQueryRecord = new BSDFQueryRecord(its.ToLocal(-Vector3d.ToVector3(ray.direction)));
                bsdfQueryRecord.uv = its.uv;

                Color albedo = its.GetBSDF().Sample(bsdfQueryRecord);
                return albedo * PathBruteIntegration(new URay_Ray(its.position, new Vector3d(its.ToWorld(bsdfQueryRecord.wo))), scene, depth -1);
            }


            //background color
            Vector3d unitDirection = ray.direction.normalized;
            float t = 0.5f * ((float)unitDirection.y + 1f);
            return (1.0f - t) * Color.white + t * new Color(0.5f, 0.7f, 1.0f, 1);
        }
    }
}
