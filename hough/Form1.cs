using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
namespace hough
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap a, b, c;
        Rectangle clone;
        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog inputimg;
            inputimg =new OpenFileDialog();
            if(inputimg.ShowDialog()==DialogResult.OK)
            {
                a = new Bitmap(inputimg.FileName);
                pictureBox1.Image = a;
                clone = new Rectangle(0, 0, a.Width, a.Height);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (a == null)
            {
                MessageBox.Show("請先載入圖片");
                return;
            }
            int[,,] source = GetRGBData(a);   
            int[] filterValue = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
            int[,,] newimg = tomatoFilter(filterValue,source);
            b= setRGBData(newimg);
            pictureBox2.Image = b;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (a == null)
            {
                MessageBox.Show("請先載入圖片");
                return;
            }
            int[,,] source = GetRGBData(a);
            int[] filterValue = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            int[,,] newimg = tomatoFilter(filterValue, source);
            b = setRGBData(newimg);
            pictureBox2.Image = b;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (a == null)
            {
                MessageBox.Show("請先載入圖片");
                return;
            }
            int[,,] source = GetRGBData(a);

            int[] filterValue = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            int[,,] newimg1 = tomatoFilter(filterValue, source);

            int[] filterValue2 = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
            int[,,] newimg2 = tomatoFilter(filterValue2, source);
            int temp;
            for(int i=0;i<a.Width-1;i++)
            {
                for(int j=0;j<a.Height;j++)
                {
                    temp = newimg1[i, j, 0] + newimg2[i, j, 0];
                    if (temp > 255)
                        temp = 255;
                    if (temp < 0)
                        temp = 0;
                    for(int p=0;p<3;p++)
                        newimg1[i, j, p] = temp;
                }
            }

            b = setRGBData(newimg1);
            pictureBox2.Image = b;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (a == null)
            {
                MessageBox.Show("請先載入圖片");
                return;
            }
            int[,,] source = GetRGBData(a);


            int[] filterValue = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            int[,,] newimg1 = tomatoFilter(filterValue, source);

            int[] filterValue2 = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
            int[,,] newimg2 = tomatoFilter(filterValue2, source);
            int temp;
            for (int i = 1; i < a.Width - 1; i++)
            {
                for (int j = 1; j < a.Height; j++)
                {
                    temp = newimg1[i, j, 0] + newimg2[i, j, 0];
                    if (temp > 255)
                        temp = 255;
                    if (temp < 0)
                        temp = 0;
                    for (int p = 0; p < 3; p++)
                        newimg1[i, j, p] = 255-temp;
                }
            }

            b = setRGBData(newimg1);
            pictureBox2.Image = b;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (a == null)
            {
                MessageBox.Show("請先載入圖片");
                return;
            }
            int[,,] source = GetRGBData(a);
            source = meanFilter(source);
            int[] filterValue = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            int[,,] newimg1 = tomatoFilter(filterValue, source);

            int[] filterValue2 = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
            int[,,] newimg2 = tomatoFilter(filterValue2, source);
            int temp;
            int bitemp = 40;   //二質化的門檻
            for (int i = 0; i < a.Width - 1; i++)
            {
                for (int j = 0; j < a.Height; j++)
                {
                    temp = newimg1[i, j, 0] + newimg2[i, j, 0];
                    if (temp > bitemp)
                        temp = 255;
                    if (temp <= bitemp)
                        temp = 0;
                    for (int p = 0; p < 3; p++)
                        newimg1[i, j, p] = temp;
                }
            }
            //開始畫parameter  //newimg1=二直圖  
            int range = Convert.ToInt32(Math.Sqrt(a.Width * a.Width + a.Height * a.Height)); //半徑rhoMax
            int[,] scouringPad = new int[range * 2 + 1, 181];
            int rho;
            
            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    if (newimg1[j, i, 0] == 255)
                    {
                        //開始畫parameter的圖
                        for (int ti = 0; ti < 181; ti++)
                        {
                            rho = Convert.ToInt16( Math.Cos(ti - 90)*i + Math.Sin(ti - 90)*j);
                            scouringPad[rho + range, ti]++;
                        }
                    }
                }
            }
            int xx=0,yy=0;
            int rhoMax = 0, totalMax = 0;
            int thetaMax=100;
            for (int ii = 0; ii < 1 + 2 * range; ii++)
            {
                for (int j = 0; j < 181; j++)
                {
                    if (scouringPad[ii, j] > totalMax)
                    {
                        xx = ii;
                        yy = j;
                        totalMax = scouringPad[ii,j];
                        rhoMax = ii-range;
                        thetaMax = j-90;
                       
                    }
                }
            }
            double chang = totalMax * 0.4;
            int[,] ccc= new int[1000, 3];
            int c_count = 0;
            for (int ii = 0; ii < 1 + 2 * range; ii++)
            {
                for (int j = 0; j < 181; j++)
                {
                    if (scouringPad[ii, j] > chang)
                    {
                        xx = ii;
                        yy = j;
                        totalMax = scouringPad[ii, j];
                        rhoMax = ii - range;
                        thetaMax = j - 90;
                        int cc=0;
                        int high;
                        if (thetaMax == 0)
                            break;
                        for (int i = 0; i < a.Width; i++)
                        {
                            high = Convert.ToInt32((rhoMax - Math.Sin(thetaMax) * i) / Math.Cos(thetaMax));
                            
                            if (high >= 0)
                            {
                                if (high < a.Height)
                                {
                                    if (newimg1[i, high, 0] == 255)
                                    {
                                        cc++;
                                        if(i==a.Width-1)
                                        {
                                            if (cc > 0.2 * range)
                                            {
                                                ccc[c_count, 0] = i;
                                                ccc[c_count, 1] = i - cc;
                                                ccc[c_count, 2] = cc;
                                                c_count++;
                                            }
                                            cc = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (cc > 0.2 * range)
                                        {
                                            ccc[c_count, 0] = i;
                                            ccc[c_count, 1] = i - cc;
                                            ccc[c_count, 2] = cc;
                                            c_count++;
                                        }
                                        cc = 0;
                                    }
                                }
                            }
                        }
                        
                    }
                }
            }
            int h;
            for (int i = 0; i <= c_count; i++)
            {
                for (int pp = ccc[i, 1]; pp <= ccc[i, 0]; pp++)
                {
                    h = Convert.ToInt32((rhoMax - Math.Sin(thetaMax) * pp) / Math.Cos(thetaMax));
                    if(h>=0)
                    {
                        if(h<a.Height)
                        {
                            source[pp, h, 0] = 255;
                            source[pp, h, 1] = 0;
                            source[pp, h, 2] = 0;
                        }
                    }
                }
            }
            b = setRGBData(source);
            pictureBox2.Image = b;
            
        }
        int[,,] tomatoFilter(int[] k, int[,,] source)
        {
            int[,,] newimg = new int[source.GetLength(0), source.GetLength(1), 3];
            int total, count;
            for (int i = 1; i < a.Width - 1; i++)
            {
                for (int j = 1; j < a.Height - 1; j++)
                {
                    total = 0; count = 0;
                    for (int p = i - 1; p <= i + 1; p++)
                    {
                        for (int q = j - 1; q <= j + 1; q++)
                        {
                            total += k[count] * (source[p, q, 0] + source[p, q, 1] + source[p, q, 2]) / 3;
                            count++;
                        }
                    }
                    if (total > 255)
                        total = 255;
                    else if (total < 0)
                    {
                        total = 0;
                    }
                    for (int p = 0; p < 3; p++)
                        newimg[i, j, p] = total;
                }
            }
            return newimg;
        }
        int[,,] meanFilter( int[,,] source)
        {
            int[,,] newimg = new int[source.GetLength(0), source.GetLength(1), 3];
            int total, count;
            for (int i = 1; i < a.Width - 1; i++)
            {
                for (int j = 1; j < a.Height - 1; j++)
                {
                    total = 0; count = 0;
                    for (int p = i - 1; p <= i + 1; p++)
                    {
                        for (int q = j - 1; q <= j + 1; q++)
                        {
                            total +=  (source[p, q, 0] + source[p, q, 1] + source[p, q, 2]) / 3;
                            count++;
                        }
                    }
                    total /= 9;
                    if (total > 255)
                        total = 255;
                    else if (total < 0)
                    {
                        total = 0;
                    }
                    for (int p = 0; p < 3; p++)
                        newimg[i, j, p] = total;
                }
            }
            return newimg;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public static int[,,] GetRGBData(Bitmap bitImg)
        {
            int height = bitImg.Height;
            int width = bitImg.Width;
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            // get the starting memory place
            IntPtr imgPtr = bitmapData.Scan0;
            //scan width
            int stride = bitmapData.Stride;
            //scan ectual
            int widthByte = width * 3;
            // the byte num of padding
            int skipByte = stride - widthByte;
            //set the place to save values
            int[,,] rgbData = new int[width, height, 3];
            #region
            unsafe//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        //B channel
                        rgbData[i, j, 2] = p[0];
                        p++;
                        //g channel
                        rgbData[i, j, 1] = p[0];
                        p++; 
                        //R channel
                        rgbData[i, j, 0] = p[0];
                        p++;
                    }
                    p += skipByte;
                }
            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return rgbData;
        }
        public static Bitmap setRGBData(int[,,] rgbData)
        {
            Bitmap bitImg;
            int width = rgbData.GetLength(0);
            int height = rgbData.GetLength(1);
            bitImg = new Bitmap(width, height, PixelFormat.Format24bppRgb);// 24bit per pixel 8x8x8
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            //get image starting place
            IntPtr imgPtr = bitmapData.Scan0;
            //image scan width
            int stride = bitmapData.Stride;
            int widthByte = width * 3;
            int skipByte = stride - widthByte;
            #region
            unsafe
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        p[0] = (byte)rgbData[i, j, 2];
                        p++;
                        p[0] = (byte)rgbData[i, j, 1];
                        p++;
                        p[0] = (byte)rgbData[i, j, 0];
                        p++;
                    }
                    p += skipByte;
                }

            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return bitImg;
        }
    }
}
