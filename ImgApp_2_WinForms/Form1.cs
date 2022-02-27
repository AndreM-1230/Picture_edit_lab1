using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ImgApp_2_WinForms
{
    public partial class Form1 : Form
    {
        private Bitmap image1 = null;
        private Bitmap image2 = null;
        private Bitmap image3 = null;


        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public Form1()
        {
            InitializeComponent();
            image1 = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            pictureBox1.Image = image1;
            image2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Image = image2;
            image3 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            pictureBox3.Image = image3;

        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog  = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (image3 != null)
                {
                    pictureBox3.Image = null;
                    image3.Dispose();
                }

                image3 = new Bitmap(openFileDialog.FileName);
                pictureBox3.Image = image3;
                image1 = new Bitmap(image3);
                pictureBox1.Image = image1;
                if (pictureBox3.Visible == false)
                {
                    bOpenone.Text = "Изменить 1";
                    pictureBox3.Visible = true;
                    bOpenone.Location = new System.Drawing.Point(900, 121);
                    bOpentwo.Visible = true;
                    label2.Visible = true;
                    comboBox2.Visible = true;
                    bDraw.Visible = true;
                    bSave.Visible = true;
                }
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileFialog = new SaveFileDialog();
            saveFileFialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileFialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            saveFileFialog.RestoreDirectory = true;

            if (saveFileFialog.ShowDialog() == DialogResult.OK)
            {
                if (image1 != null)
                {
                    image1.Save(saveFileFialog.FileName);
                }
            }
        }

        //Попиксельная сумма
        static void pixel(Bitmap image1, Bitmap image2, Bitmap image3)
        {
            var w = image1.Width;
            var h = image1.Height;

            
                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {

                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);

                        int r = (int)Clamp(pix1.R + pix2.R, 0, 255);
                        int g = (int)Clamp(pix1.G + pix2.G, 0, 255);
                        int b = (int)Clamp(pix1.B + pix2.B, 0, 255);

                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);

                    }
                }
        }

        //Произведение
        static void multiplication(Bitmap image1, Bitmap image2, Bitmap image3)
        {
            var w = image1.Width;
            var h = image1.Height;

                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {

                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);

                        int r = (int)Clamp(pix1.R * pix2.R / 255, 0, 255);
                        int g = (int)Clamp(pix1.G * pix2.G / 255, 0, 255);
                        int b = (int)Clamp(pix1.B * pix2.B / 255, 0, 255);

                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);

                    }
                }
        }

        //Маска круг
        static void pixel_mask_circle(Bitmap image1, Bitmap image2, Bitmap image3, int x, int y, int rad)
        {
            var w = image1.Width;
            var h = image1.Height;
            int r, g, b;
                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {
                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);
                        r = pix1.R;
                        g = pix1.G;
                        b = pix1.B;
                    if (Math.Pow((i - x), 2) + Math.Pow((j - y), 2) <= rad * rad)
                        {
                            r = pix2.R;
                            g = pix2.G;
                            b = pix2.B;
                        }
                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);
                    }
                }
        }

        static int product(int Px, int Py, int Ax, int Ay, int Bx, int By)
        {
            return (Bx - Ax) * (Py - Ay) - (By - Ay) * (Px - Ax);
        }
        //Маска прямоугольник
        static void pixel_mask_rectangle(Bitmap image1, Bitmap image2, Bitmap image3, int x, int y, int ww, int hh)
        {
            var w = image3.Width;
            var h = image3.Height;

            int x1, x2, x3, x4;
            int y1, y2, y3, y4;
            int r, g, b;
            y1 = y2 = y + hh / 2; // y1y4
            y3 = y4 = y - hh / 2; // y2 y3
            x1 = x4 = x - ww / 2; // x1 x2
            x2 = x3 = x + ww / 2; // x3 x4

                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {
                        int p1 = product(j, i, x1, y1, x2, y2),
                            p2 = product(j, i, x2, y2, x3, y3),
                            p3 = product(j, i, x3, y3, x4, y4),
                            p4 = product(j, i, x4, y4, x1, y1);
                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);
                        r = pix2.R;
                        g = pix2.G;
                        b = pix2.B;
                        if ((p1 < 0 && p2 < 0 && p3 < 0 && p4 < 0) ||
                            (p1 > 0 && p2 > 0 && p3 > 0 && p4 > 0))
                        {
                        }
                        else
                        {
                        r = pix1.R;
                        g = pix1.G;
                        b = pix1.B;
                    }
                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);
                    }
                }
        }

        //Маска квадрат
        static void pixel_mask_square(Bitmap image1, Bitmap image2, Bitmap image3, int x, int y, int wh)
        {
            var w = image1.Width;
            var h = image1.Height;

            int x1, x2, x3, x4;
            int y1, y2, y3, y4;
            y1 = y2 = y + wh / 2; // y1y4
            y3 = y4 = y - wh / 2; // y2 y3
            x1 = x4 = x - wh / 2; // x1 x2
            x2 = x3 = x + wh / 2; // x3 x4
            int r, g, b;
                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {
                        int p1 = product(j, i, x1, y1, x2, y2),
                            p2 = product(j, i, x2, y2, x3, y3),
                            p3 = product(j, i, x3, y3, x4, y4),
                            p4 = product(j, i, x4, y4, x1, y1);
                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);
                        r = pix2.R;
                        g = pix2.G;
                        b = pix2.B;

                        if ((p1 < 0 && p2 < 0 && p3 < 0 && p4 < 0) ||
                            (p1 > 0 && p2 > 0 && p3 > 0 && p4 > 0))
                        {
                        }
                        else
                        {
                            r = pix1.R;
                            g = pix1.G;
                            b = pix1.B;
                    }
                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);
                    }
                }
        }

        //Среднее арифметическое

        static void arithmetic(Bitmap image1, Bitmap image2, Bitmap image3)
        {
            var w = image1.Width;
            var h = image1.Height;

                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {

                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);

                        int r = (int)Clamp(pix1.R + pix2.R / 2, 0, 255);
                        int g = (int)Clamp(pix1.G + pix2.G / 2, 0, 255);
                        int b = (int)Clamp(pix1.B + pix2.B / 2, 0, 255);

                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);

                    }
                }
        }

        //Минимум
        static void min(Bitmap image1, Bitmap image2, Bitmap image3)
        {
            var w = image1.Width;
            var h = image1.Height;

                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {

                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);

                        int r = pix2.R;
                        int g = pix2.G;
                        int b = pix2.B;

                        if ((int)pix1.R < (int)pix2.R)
                        {
                            r = pix1.R;
                        }
                        else
                        {
                            r = pix2.R;
                        }

                        if (pix1.G < pix2.G)
                        {
                            g = pix1.G;
                        }
                        else
                        {
                            g = pix2.G;
                        }

                        if (pix1.B < pix2.B)
                        {
                            b = pix1.B;
                        }
                        else
                        {
                            b = pix2.B;

                        }

                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);

                    }
                }
        }

        //Максимум
        static void max(Bitmap image1, Bitmap image2, Bitmap image3)
        {
            var w = image1.Width;
            var h = image1.Height;

                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {

                        var pix1 = image3.GetPixel(j, i);
                        var pix2 = image2.GetPixel(j, i);

                        int r = pix2.R;
                        int g = pix2.G;
                        int b = pix2.B;

                        if ((int)pix1.R > (int)pix2.R)
                        {
                            r = pix1.R;
                        }
                        else
                        {
                            r = pix2.R;
                        }

                        if (pix1.G > pix2.G)
                        {
                            g = pix1.G;
                        }
                        else
                        {
                            g = pix2.G;
                        }

                        if (pix1.B > pix2.B)
                        {
                            b = pix1.B;
                        }
                        else
                        {
                            b = pix2.B;

                        }

                        pix1 = Color.FromArgb(r, g, b);
                        image1.SetPixel(j, i, pix1);

                    }
                }
        }


        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void bDraw_Click(object sender, EventArgs e)
        {
            var w = image3.Width;
            var h = image3.Height;
            image1 = new Bitmap(image3);
            if (pictureBox2.Visible == true)
            {
                if (w * h != image2.Width * image2.Height)
                {
                    if (w * h > image2.Width * image2.Height)
                    {
                        image2 = ResizeImage(image2, w, h);
                    }
                    else if (w * h < image2.Width * image2.Height)
                    {

                        w = image2.Width;
                        h = image2.Height;
                        image3 = ResizeImage(image3, w, h);
                    }
                }
                image1 = new Bitmap(w, h);
            
            

                int index = comboBox1.FindString(comboBox1.Text);
                comboBox1.SelectedIndex = index;
                switch (index)
                {
                    case 0:
                        //попиксельно сумма
                        pixel(image1, image2, image3);
                        break;

                    case 1:
                        // произведение
                        multiplication(image1, image2, image3);
                        break;

                    case 2:
                        //среднее-арифметическое
                        arithmetic(image1, image2, image3);
                        break;

                    case 3:
                        //минимум
                        min(image1, image2, image3);
                        break;

                    case 4:
                        //максимум
                        max(image1, image2, image3);
                        break;

                    case 5:
                        //маска
                        int indexmask = comboBox3.FindString(comboBox3.Text);
                        comboBox3.SelectedIndex = indexmask;
                        var w1 = w / 2;
                        var h1 = h / 2;
                        int r;
                        switch (indexmask)
                        {   
                            case 0:
                                //круг
                                if (w > h)
                                {
                                    r = h / 3;
                                }
                                else
                                {
                                    r = w / 3;
                                }
                                pixel_mask_circle(image1, image2, image3, h1, w1, r);
                                break;
                            case 1:
                                //квадрат
                                if (w > h)
                                {
                                    r = h / 3;
                                }
                                else
                                {
                                    r = w / 3;
                                }
                                pixel_mask_square(image1, image2, image3, w1, h1, r);
                                break;
                            case 2:
                                //прямоугольник
                                int ww = w / 2;
                                int hh = h / 2;
                                pixel_mask_rectangle(image1, image2, image3, w1, h1, ww, hh);
                                break;
                        }
                        break;

                }
            }

            int indexrgb = comboBox2.FindString(comboBox2.Text);
            comboBox2.SelectedIndex = indexrgb;
            switch (indexrgb)
            {
                case 0:
                    //_RGB

                    break;

                case 1:
                    //R
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix1 = image1.GetPixel(j, i);
                            int r = pix1.R;
                            int g = 0;
                            int b = 0;
                            pix1 = Color.FromArgb(r, g, b);
                            image1.SetPixel(j, i, pix1);
                        }
                    }
                    break;
                case 2:
                    //G
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix1 = image1.GetPixel(j, i);
                            int r = 0;
                            int g = pix1.G;
                            int b = 0;
                            pix1 = Color.FromArgb(r, g, b);
                            image1.SetPixel(j, i, pix1);
                        }
                    }
                    break;
                case 3:
                    //B
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix1 = image1.GetPixel(j, i);
                            int r = 0;
                            int g = 0;
                            int b = pix1.B;
                            pix1 = Color.FromArgb(r, g, b);
                            image1.SetPixel(j, i, pix1);
                        }
                    }
                    break;
                case 4:
                    //RG
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix1 = image1.GetPixel(j, i);
                            int r = pix1.R;
                            int g = pix1.G;
                            int b = 0;
                            pix1 = Color.FromArgb(r, g, b);
                            image1.SetPixel(j, i, pix1);
                        }
                    }
                    break;
                case 5:
                    //GB
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix1 = image1.GetPixel(j, i);
                            int r = 0;
                            int g = pix1.G;
                            int b = pix1.B;
                            pix1 = Color.FromArgb(r, g, b);
                            image1.SetPixel(j, i, pix1);
                        }
                    }
                    break;
                case 6:
                    //RB
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix1 = image1.GetPixel(j, i);
                            int r = pix1.R;
                            int g = 0;
                            int b = pix1.B;
                            pix1 = Color.FromArgb(r, g, b);
                            image1.SetPixel(j, i, pix1);
                        }
                    }
                    break;
               
            }

            MessageBox.Show("Готово " + (image1.Width).ToString() +" x "+ (image1.Height).ToString());
            pictureBox1.Image = image1;
            //if (index ==1)
            //{



            //using Graphics g = Graphics.FromImage(image1);
            //Random r = new Random();
            //for (int i = 0; i < 10; ++i)
            //{
            //    var color = Color.FromArgb(r.Next(255), 
            //                              r.Next(255), 
            //                               r.Next(255));

            //    using Brush b = new SolidBrush(color);

            //    g.FillEllipse(b,r.Next(image1.Width), r.Next(image1.Width),20,20);
            //}
            //}
            //pictureBox1.Refresh();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBox1.FindString(comboBox1.Text);
            comboBox1.SelectedIndex = index;
            if(index == 5)
            {
                comboBox3.Visible = true;
                label3.Visible = true;
            }
            else
            {
                comboBox3.Visible = false;
                label3.Visible = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (image2 != null)
                {
                    pictureBox2.Image = null;
                    image2.Dispose();
                }
                
                image2 = new Bitmap(openFileDialog.FileName);
                pictureBox2.Image = image2;
                if(pictureBox2.Visible == false)
                {
                    bOpentwo.Text = "Изменить 2";
                    pictureBox2.Visible = true;
                    bOpentwo.Location = new System.Drawing.Point(900, 259);
                    label1.Visible = true;
                    comboBox1.Visible = true;
                    button1.Visible = true;
                    
                }
                
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Image = image3;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
