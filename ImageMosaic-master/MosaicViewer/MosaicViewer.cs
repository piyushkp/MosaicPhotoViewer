using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;

namespace MosaicViewer
{
    public partial class MosaicViewer : Form
    {
        string sourceFile;
        string imageFolder;
        string saveImagefolder;
        string shapeFolderPath;
        bool isFullscreen = false;
        private FormWindowState winState;
        private FormBorderStyle brdStyle;
        private bool topMost;
        private Rectangle bounds;

        public MosaicViewer()
        {
            //saveImagefolder = ConfigurationSettings.AppSettings["SaveMosaicImagefolder"];

            InitializeComponent();

            pictureBox2.Parent = pictureBox1;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.BackColor = Color.Transparent;

            //new Thread(() =>
            //{
            //    Thread.CurrentThread.IsBackground = true;

            //    GetRotatedShape();

            //}).Start();

            var task = new Task(() => GetRotatedShape(),
                    TaskCreationOptions.LongRunning);
            task.Start();
        }

        public void MosaicGenerator(string sourceFile, string shapeName)
        {
            try
            {
                var watch = Stopwatch.StartNew();

                //var _mosaicGenerator = new MosaicGenerator();
                //var _mosaic = _mosaicGenerator.Generate(sourceFile, imageFolder, pictureBox1, pictureBox2, shapeName);

                ImageMosaic.MosaicGenerator.Generate(sourceFile, imageFolder, pictureBox1, pictureBox2, shapeName);

                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds / (1000 * 60);

                //MessageBox.Show("Mosaic photo generated in " + elapsedMs + " minutes.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetRotatedShape()
        {
            shapeFolderPath = ConfigurationSettings.AppSettings["ShapeFolderPath"];

            string[] shapefiles = Directory.GetFiles(shapeFolderPath);

            foreach (var item in shapefiles)
            {
                MosaicGenerator(item, Path.GetFileNameWithoutExtension(item).ToLower());

                pictureBox2.Invoke(new MethodInvoker(() =>
                {
                    pictureBox2.Image = null;
                    pictureBox2.Size = new System.Drawing.Size(800, 600);
                    pictureBox2.Location = new System.Drawing.Point(0, 0);

                }));
                
                Thread.Sleep(100);
            }

            GetRotatedShape();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isFullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                //this.AutoScroll = false;
                isFullscreen = true;
            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Maximized;
                this.AutoScroll = true;
                isFullscreen = false;
            }
        }      

      }
}
