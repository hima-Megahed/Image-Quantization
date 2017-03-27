using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        int K=0,distinct_colors=0;
        public MainForm()
        {
            InitializeComponent();
        }

        public static RGBPixel[,] ImageMatrix, ImageMatrix_Tmp;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                Graph constructed = new Graph();
                    
                //constructed = ALL_FUNCTIONS.construct_ALL(ImageMatrix);
                //Graph MST=ALL_FUNCTIONS.set_MST(constructed);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                int rw=ImageMatrix.GetLength(0) , cl=ImageMatrix.GetLength(1);
                ImageMatrix_Tmp=new RGBPixel[rw,cl];
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();

            distinct_colors=ALL_FUNCTIONS.get_distinct(ref ImageMatrix);
            //ALL_FUNCTIONS.test();
            ALL_FUNCTIONS.Set_Mst();
                 // to handl if user input more than one cluster in same image and make operation on original image
            ImageMatrix_Tmp = ImageMatrix.Clone() as RGBPixel[,]; 
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            int.TryParse(ClustorBox.Text, out K);
            
            if (string.IsNullOrWhiteSpace(ClustorBox.Text))
            {
                MessageBox.Show("Please enter number of cluster !");
            }
            else if (int.Parse(ClustorBox.Text) > distinct_colors || int.Parse(ClustorBox.Text)<1)
            {
                MessageBox.Show("Invalid number of clusters !");
            }
            else
            {
                
                List<List<RGBPixel>> groups = new List<List<RGBPixel>>();
                RGBPixel[, ,] Update = new RGBPixel[256, 256, 256];
                double sigma = double.Parse(txtGaussSigma.Text);
                int maskSize = (int)nudMaskSize.Value;
                ALL_FUNCTIONS.Build_tree();
                 ALL_FUNCTIONS.buildAdjacencyList();
                ALL_FUNCTIONS.CutEdges( K);
                groups = ALL_FUNCTIONS.cluster( K);
                ALL_FUNCTIONS.GetPalette(ref groups);
                Update = ALL_FUNCTIONS.UpdateColor(ref groups);
                ALL_FUNCTIONS.UpdatedMatrix(Update, ImageMatrix);
                ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
                                                                                // Get the original image after make edition on it ..
                 ImageMatrix = ImageMatrix_Tmp.Clone() as RGBPixel[,];
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var bitmap = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            string Path="";
            SaveFileDialog s = new SaveFileDialog();
            s.FileName = "Image";// Default file name
            s.DefaultExt = ".bmp";// Default file extension
   
            if (s.ShowDialog() == DialogResult.OK)
            {
                Path = s.FileName;
                pictureBox2.DrawToBitmap(bitmap, pictureBox2.ClientRectangle);
                ImageFormat imageFormat = ImageFormat.Bmp;
                bitmap.Save(Path, imageFormat);
            }
            

        }

        
    }
}