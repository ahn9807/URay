using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Unity.Jobs;
using Unity.Collections;

namespace URay
{
    public class URay_Main : MonoBehaviour
    {
        public RawImage screenImage;
        public Camera renderCamera;
        public int spp = 100;
        public URay_Image uRayImage;
        URay_Scene uRayScene;

        public void Render()
        {
            //Image
            float aspectRatio = renderCamera.aspect;
            int imageWidth = 720;
            int imageHeight = (int)(imageWidth / aspectRatio);
            screenImage.texture = null;
            uRayImage = new URay_Image(imageWidth, imageHeight, 25, screenImage);
            uRayScene = new URay_Scene();
            URay_Camera uRayCamera = new URay_Camera(renderCamera);
            URay_ImageBlock[] imageBlocks = uRayImage.GetImageBlocks();

            foreach(GameObject gameObject in GameObject.FindObjectsOfType<GameObject>())
            {
                uRayScene.AddObject(gameObject);
            }

            foreach (URay_ImageBlock block in imageBlocks)
            {
                
                //ThreadStart threadStart = delegate
                //{
                //    RenderBlock(uRayScene, block, imageWidth, imageHeight, uRayCamera);
                //};

                //new Thread(threadStart).Start();
                
                /*
                Can't do threading due to raycast is implemented in Unity API Side!!
                We have to fix this problem to accelerate our computations
                Also we can't use unity job system due to the same reason
                So We habe to make our own raycast system and corresponding accleration system..
                */
                //This code is temporary code. Use unity's default raycast system with collider
                StartCoroutine(IERenderBlock(uRayScene, block, imageWidth, imageHeight, uRayCamera));
                //Unity Job system
            }
        }

        public void RenderBlock(URay_Scene scene, URay_ImageBlock block, int blockWidth, int blockHeight, URay_Camera uRay_Camera)
        {
            URay_Integrator integrator = scene.GetIntegrator();

            for (int y = 0; y < block.height; y++)
            {
                for (int x = 0; x < block.width; x++)
                {
                    Color pixelColor = new Color(0, 0, 0);
                    for (int s = 0; s < spp; s++)
                    {
                        float u = block.GetPixelPosition(x, y).x + URay_Sampler.UniformNumber();
                        float v = block.GetPixelPosition(x, y).y + URay_Sampler.UniformNumber();

                        u /= (blockWidth - 1);
                        v /= (blockHeight - 1);

                        URay_Ray r = uRay_Camera.Sample(u, v);
                        Color col = integrator.Li(scene, r, 32) / (float)spp;
                        float scale = 1.0f / spp;
                        pixelColor += new Color(Mathf.Sqrt(scale * col.r), Mathf.Sqrt(scale * col.g), Mathf.Sqrt(scale * col.b));
                    }
                    //pixelColor = new Color(u, v, 0.25f);

                    block.SetPixel(x, y, pixelColor);
                }
            }

            uRayImage.PutImageBlock(block);
            uRayImage.Apply();
        }

        IEnumerator IERenderBlock(URay_Scene scene, URay_ImageBlock block, int imageWdith, int imageHeight, URay_Camera uRay_Camera)
        {
            RenderBlock(scene, block, imageWdith, imageHeight, uRay_Camera);
            uRayImage.Apply();
            yield return null;
        }
    }
}
