using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Configuration;

namespace ImageMosaic
{
    public class ImageProcessing
    {
        private int quality = 6, resizeHeight = 119, resizeWidth = 119;
        private Size tileSize = new Size(12, 12);
        private List<ImageInfo> library;

        public Bitmap Resize(string srcFile)
        {
            if (!File.Exists(srcFile))
                return null;

            using (var scrBitmap = Bitmap.FromFile(srcFile))
            {
                var b = new Bitmap(resizeHeight, resizeWidth);

                using (var g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(scrBitmap, 0, 0, resizeHeight, resizeWidth);
                    g.Dispose();
                }

                return b;
            }
        }

        public ImageInfo GetAverageColor(Bitmap bmp, string filePath)
        {
            var imageInfo = new ImageInfo(filePath);

            int halfX = bmp.Width / 2;
            int halfY = bmp.Width / 2;

            imageInfo.AverageTL = getAverageColor(new Rectangle(0, 0, halfX, halfY), bmp, quality);
            imageInfo.AverageTR = getAverageColor(new Rectangle(halfX, 0, bmp.Width - halfX, halfY), bmp, quality);
            imageInfo.AverageBL = getAverageColor(new Rectangle(0, halfY, halfX, bmp.Height - halfY), bmp, quality);
            imageInfo.AverageBR = getAverageColor(new Rectangle(halfX, halfY, bmp.Width - halfX, bmp.Height - halfY), bmp, quality);

            return imageInfo;
        }

        private Color getAverageColor(Rectangle area, Bitmap bmp, int quality)
        {
            Int64 r = 0, g = 0, b = 0;
            var c = Color.Empty;
            int p = 0;

            var end = new Point(area.X + area.Width, area.Y + area.Height);

            for (int x = area.X = 0; x < end.X; x += quality)
            {
                for (int y = area.Y; y < end.Y; y += quality)
                {
                    c = bmp.GetPixel(x, y);
                    r += c.R;
                    g += c.G;
                    b += c.B;
                    p++;
                }
            }

            return Color.FromArgb(255, (int)(r / p), (int)(g / p), (int)(b / p));
        }

        private int GetBestImageIndex(Color c, int x, int y)
        {
            double bestPercent = double.MaxValue;
            int bestIndex = 0;
            const byte offset = 7;

            double difference;
            Color[] passColor;

            int r, g, b;

            for (int i = 0; i < library.Count(); i++)
            {
                passColor = new Color[4];
                passColor[0] = library[i].AverageTL;
                passColor[1] = library[i].AverageTR;
                passColor[2] = library[i].AverageBL;
                passColor[3] = library[i].AverageBR;

                r = passColor[0].R + passColor[1].R + passColor[2].R + passColor[3].R;
                g = passColor[0].G + passColor[1].G + passColor[2].G + passColor[3].G;
                b = passColor[0].B + passColor[1].B + passColor[2].B + passColor[3].B;

                r = Math.Abs(c.R - (r / 4));
                g = Math.Abs(c.G - (g / 4));
                b = Math.Abs(c.B - (b / 4));

                difference = r + g + b;
                difference /= 3 * 255;

                if (difference < bestPercent)
                {

                    Point p = new Point();

                    if (library[i].Data.Count > 0 && library[i].Data[0] != null)
                        p = (Point)library[i].Data[0];

                    if (p.IsEmpty)
                    {
                        bestPercent = difference;
                        bestIndex = i;
                    }
                    else if (p.X + offset <= x && p.Y + offset > y && p.Y - offset < y)
                    {
                        bestPercent = difference;
                        bestIndex = i;
                    }

                }
            }

            library[bestIndex].Data.Add(new Point(x, y));
            return bestIndex;
        }

        public void Render(Bitmap img, Dictionary<int, List<int>> xDic, int xLength, int yLength, PictureBox pictureBox1, PictureBox pictureBox2, string shapeName)
        {
            try
            {
                var g = default(Graphics);
                var g2 = default(Graphics);
                //var imageSq = new List<MosaicTile>();
                var newImg = new Bitmap(xLength * tileSize.Width, yLength * tileSize.Height);
                var pb2Img = new Bitmap(xLength * tileSize.Width, yLength * tileSize.Height);
                //var saveImg = new Bitmap(colorMap.GetLength(0) * tileSize.Width, colorMap.GetLength(1) * tileSize.Height);
                //var file = Graphics.FromImage(saveImg);
                var rand = new Random();

                pictureBox1.Invoke(new MethodInvoker(() =>
                {
                    pictureBox1.Image = newImg;
                    pictureBox2.Image = pb2Img;

                    g = Graphics.FromImage(pictureBox1.Image);

                    g2 = Graphics.FromImage(pictureBox2.Image);

                }));

                var b = new SolidBrush(Color.Black);
                g.FillRectangle(b, 0, 0, img.Width, img.Height);
                g2.FillRectangle(b, 0, 0, img.Width, img.Height);
                //file.FillRectangle(b, 0, 0, img.Width, img.Height);

                Rectangle destRect, srcRect;

                foreach (var xItem in xDic)
                {
                    foreach (var yItem in xItem.Value)
                    {
                        int x = xItem.Key;
                        int y = yItem;

                        string[] files = Directory.GetFiles(ConfigurationSettings.AppSettings["DownloadImagesFolderPath"])
                            .Where(file => !file.ToLower().Contains("processed"))
                            .ToArray();

                        string path = string.Empty;
                        if (files.Length > 0)
                        {
                            path = files[0];
                        }
                        else
                        {                           
                            var processedList = Directory.GetFiles(ConfigurationSettings.AppSettings["DownloadImagesFolderPath"])
                            .Where(file => file.ToLower().Contains("processed")).ToArray();
                            path = processedList[rand.Next(processedList.Length)];
                        }

                        using (Image source = Image.FromFile(path))
                        {
                            //imageSq.Add(new MosaicTile()
                            //{
                            //    X = x,
                            //    Y = y,
                            //    Image = path
                            //});

                            srcRect = new Rectangle(0, 0, source.Width, source.Height);

                            for (int i = 1; i < 15; )
                            {
                                pictureBox2.Invoke(new MethodInvoker(() =>
                                {
                                    pictureBox2.Image = PictureBoxZoom(source, new Size(i, i));
                                    int xloc = ((x * tileSize.Width));
                                    int yloc = ((y * tileSize.Width));
                                    if (xloc < 0)
                                        xloc = 0;
                                    if (yloc < 0)
                                        yloc = 0;

                                    pictureBox2.Location = new Point(xloc, yloc);
                                    pictureBox2.Refresh();

                                }));

                                Thread.Sleep(400 / i);
                                i += i;
                            }

                            destRect = new Rectangle(x * tileSize.Width, y * tileSize.Height, tileSize.Width, tileSize.Height);

                            g.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
                            //file.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);

                            pictureBox1.Invoke(new MethodInvoker(() =>
                            {
                                pictureBox1.Refresh();
                            }));
                        }

                        if (files.Length > 0)
                        {
                            //File.Delete(path);
                            
                            File.Move(path, Path.GetDirectoryName(path) + "\\processed_" + Path.GetFileName(path));
                        }

                    }
                }

                pictureBox2.Invoke(new MethodInvoker(() =>
                {
                    pictureBox1.Image.Save(string.Format(ConfigurationSettings.AppSettings["SaveMosaicImagefolder"] + "\\{0}.jpg", Guid.NewGuid().ToString("N")));
                    pictureBox2.Size = new System.Drawing.Size(5, 5);
                    pictureBox2.Location = new System.Drawing.Point(0, 0);

                }));

                //return new Mosaic()
                //{
                //    Image = saveImg,
                //    Tiles = imageSq
                //};
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Image PictureBoxZoom(Image img, Size size)
        {
            Bitmap bm = new Bitmap(img, Convert.ToInt32(img.Width / size.Width), Convert.ToInt32(img.Height / size.Height));
            Graphics grap = Graphics.FromImage(bm);
            grap.InterpolationMode = InterpolationMode.HighQualityBicubic;
            return bm;
        }
    }
}
