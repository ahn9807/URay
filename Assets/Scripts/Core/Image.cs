using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace URay
{
    public class Image
    {
        RawImage screenImage;
        int imageWidth;
        int imageHeight;
        int chunkSize;
        int blockSize;
        int blockOffsetHeight;
        int blockOffsetWidth;
        Texture2D mainTexture;
        ImageBlock[] blocks;

        public Image(int width, int height, int chunkSize, RawImage image)
        {
            this.imageWidth = width;
            this.imageHeight = height;
            this.chunkSize = chunkSize;
            this.screenImage = image;
            this.mainTexture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            this.blockSize = Mathf.RoundToInt(Mathf.Sqrt(chunkSize));
            this.blockOffsetHeight = height / blockSize;
            this.blockOffsetWidth = width / blockSize;
            blocks = new ImageBlock[chunkSize];
            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    blocks[j + i * blockSize] = new ImageBlock(blockOffsetWidth, blockOffsetHeight, new Vector2(i, j));
                }
            }

            image.texture = this.mainTexture;
        }

        public ImageBlock[] GetImageBlocks()
        {
            return blocks;
        }

        public void PutImageBlock(ImageBlock block)
        {
            int startX = (int)block.offset.x * blockOffsetWidth;
            int startY = (int)block.offset.y * blockOffsetHeight;
            mainTexture.SetPixels(startX, startY, block.width, block.height, block.GetPixels());
        }

        public void Apply()
        {
            mainTexture.Apply();
        }
    }
    public struct ImageBlock
    {
        public Vector2 offset;
        public int width;
        public int height;
        Color[] pixels;

        public ImageBlock(int width, int height, Vector2 offset)
        {
            this.height = height;
            this.width = width;
            this.pixels = new Color[width * height];
            this.offset = offset;
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.black;
            }
        }

        public Vector2 GetPixelPosition(int x, int y)
        {
            return new Vector2(x + width * offset.x, y + offset.y * height);
        }

        public void SetPixel(int x, int y, Color col)
        {
            pixels[x + y * width] = col;
        }

        public Color GetPixel(int x, int y)
        {
            return pixels[x + y * width];
        }

        public Color[] GetPixels()
        {
            return pixels;
        }
    }
}