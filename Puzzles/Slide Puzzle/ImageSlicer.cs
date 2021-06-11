using UnityEngine;

/// <summary> Used to cut up an image into pieces for puzzles
/// </summary>
public class ImageSlicer
{
    #region Variables


    #endregion

        /// <summary>
        /// Cuts up an image in blocks, and returns them as an array.
        /// </summary>
        /// <param name="image">The whole image, to be cut up</param>
        /// <param name="blocksPerRow">The amount of blocks, horizontally</param>
        /// <param name="blocksPerColumn">The amount of blocks, vertically</param>
        /// <returns></returns>
        public static Texture2D[,] GetSlices(Texture2D image, int blocksPerRow, int blocksPerColumn)
        {
            int imageSize = image.width;
            int blockSize = imageSize / blocksPerRow;

            Texture2D[,] blocks = new Texture2D[blocksPerRow, blocksPerColumn];

            for (int y = 0; y < blocksPerColumn; y++)
            {
                for (int x = 0; x < blocksPerRow; x++)
                {
                    Texture2D block = new Texture2D(blockSize, blockSize);
                    block.wrapMode = TextureWrapMode.Clamp;
                    block.SetPixels(image.GetPixels(x*blockSize, y*blockSize, blockSize, blockSize));
                    block.Apply();
                    blocks[x,y] = block;
                }
            }

            return blocks;
        }


}