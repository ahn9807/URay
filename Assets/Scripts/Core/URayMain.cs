using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace URay
{
    public class URayMain : MonoBehaviour
    {
        public RawImage screenImage;
        Image uRayImage;
        public int width;
        public int height;

        Texture2D mainTexture;
        List<Color> colors;

        public void Render()
        {
            width = 1280;
            height = 720;
            uRayImage = new Image(width, height, 25, screenImage);
            ImageBlock[] imageBlocks = uRayImage.GetImageBlocks();

            foreach (ImageBlock block in imageBlocks)
            {
                for (int x = 0; x < block.width; x++)
                {
                    for (int y = 0; y < block.height; y++)
                    {
                        Vector2 pos = block.GetPixelPosition(x, y);
                        block.SetPixel(x, y, new Color(pos.x / width, pos.y / height, 0.25f, 1));
                    }
                }

                uRayImage.PutImageBlock(block);
            }
            uRayImage.Apply();
        }
    }
}
