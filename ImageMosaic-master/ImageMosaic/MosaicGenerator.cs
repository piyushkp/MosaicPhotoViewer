using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace ImageMosaic
{
    public static class MosaicGenerator
    {
        public static void Generate(string imageToMash, string srcImageDirectory, PictureBox pictureBox1, PictureBox pictureBox2, string shapeName)
        {
            try
            {
                var _imageProcessing = new ImageProcessing();

                //var _mosaic = new Mosaic();                

                using (var source = new Bitmap(imageToMash))
                {
                    Dictionary<int, List<int>> xDic = new Dictionary<int, List<int>>();

                    CreateMap(source, xDic);
                    Random rand = new Random();
                    Shuffle(ref xDic);
                    int xLength = (int)source.Width / 12;
                    int yLength = (int)source.Height / 12;
                    
                    //_mosaic = _imageProcessing.Render(source, _colorMap, pictureBox1, pictureBox2, shapeName);
                    _imageProcessing.Render(source, xDic, xLength, yLength, pictureBox1, pictureBox2, shapeName);
                }

                //return _mosaic;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CreateMap(Bitmap img, Dictionary<int, List<int>> xDic)
        {
            int horizontalTiles = (int)img.Width / 12;
            int verticalTiles = (int)img.Height / 12;
            int tileWidth = (img.Width - img.Width % horizontalTiles) / horizontalTiles;
            int tileHeight = (img.Height - img.Height % verticalTiles) / verticalTiles;

            Int64 r, g, b;
            int pixelCount;
            Color c;

            int xPos, yPos;


            for (int x = 0; x < horizontalTiles; x++)
            {
                bool isBlack = true;
                List<int> yList = new List<int>();
                for (int y = 0; y < verticalTiles; y++)
                {                   
                    r = 0;
                    g = 0;
                    b = 0;
                    pixelCount = 0;

                    for (xPos = tileWidth * x; xPos < x * tileWidth + tileWidth; xPos++)
                    {
                        for (yPos = tileHeight * y; yPos < y * tileHeight + tileHeight; yPos++)
                        {
                            c = img.GetPixel(xPos, yPos);
                            r += c.R; g += c.G; b += c.B;
                            pixelCount++;
                        }
                    }

                    //colorMap[x, y] = Color.FromArgb(255, (int)r / pixelCount, (int)g / pixelCount, (int)b / pixelCount);
                    if (Color.FromArgb(255, (int)r / pixelCount, (int)g / pixelCount, (int)b / pixelCount).Name != "ff000000")
                    {
                        isBlack = false;
                        yList.Add(y);
                    }
                }
                if (!isBlack)
                {
                    Shuffle(yList);
                    xDic.Add(x, yList);
                }
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Shuffles a dictionary using the Fisher-Yates algorithm
        /// </summary>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="dictionary">Dictionary to shuffle</param>
        public static void Shuffle<TKey, TValue>(ref Dictionary<TKey, TValue> dictionary)
        {
            KeyValuePair<TKey, TValue>[] keyValuePairs = dictionary.ToArray();
            Random random = new Random();
            for (int i = keyValuePairs.Length - 1; i >= 0; i--)
            {
                int j = random.Next(0, i + 1);
                KeyValuePair<TKey, TValue> temp = keyValuePairs[i];
                keyValuePairs[i] = keyValuePairs[j];
                keyValuePairs[j] = temp;
            }
            dictionary = keyValuePairs.ToDictionary(k => k.Key, k => k.Value);
        }

    }
}
