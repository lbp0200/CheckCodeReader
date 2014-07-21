using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadCheckCode
{
    public class CheckCodeReader
    {
        Dictionary<int, string> codeTable = new Dictionary<int, string> { { 0, "0000111111100000111111111000111111111110111000000011011000000000101100000000010111000000011001111111111100011111111100000111111100" }, { 1, "001100000000100110000000010011000000001011111111111101111111111110111111111111000000000000100000000000010000000000001" }, { 2, "001100000001101100000001110110000001111011000000110101100000110010111000110001011111110000100111110000010001110000001" }, { 3, "00110000000110110001100001011000110000101100011000010110011110011011111111111100111100111110001110001110" }, { 4, "0000000011100000000011110000000011011000000111001100000111000110000110000011000111111111111011111111111101111111111110000000001100" }, { 5, "01111110000110111111000001011001100000101100110000010110011100011011000111111101100011111110110000111110" }, { 6, "0000011111100000111111111000111111111110111100110011011100110000101100011000010110001110011011000111111100110001111110000000011110" }, { 7, "011000000000001100000000010110000000111011000001111101100001111000110011110000011011100000001111000000000111000000000" }, { 8, "0000000001110000111001111100111111111110111111110011011001110000101100011100010111111111001001111111111100011100111110000000001110" }, { 9, "0001111000000001111110001101111111100010111001110001011000011000101100001100110111001100111001111111111100011111111100000111111000" } };
        Bitmap img;
        private Color __c;
        private int t = 128;
        public int Threshold
        {
            get
            {
                return t;
            }
            set
            {
                t = value;
            }
        }
        private int __blackNum = 0;
        private int b = 0;
        private int w = 255;
        private int __count;
        List<int> XList = new List<int>();
        List<int> YList = new List<int>();
        private bool isWhilteLine;

        public CheckCodeReader(Image _img)
        {
            img = new Bitmap(_img);
        }

        #region 二值化图片
        /// <summary>  
        /// 二值化图片  
        /// 就是将图像上的像素点的灰度值设置为0或255  
        /// </summary>  
        /// <returns>处理后的验证码</returns>  
        public Bitmap BinaryZaTion()
        {
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    __c = img.GetPixel(x, y);
                    //灰度值  
                    int __tc = (__c.R + __c.G + __c.B) / 3;
                    //大于阙值 黑色  
                    if (__tc < t)
                    {
                        img.SetPixel(x, y, Color.FromArgb(__c.A, b, b, b));
                        //黑色点个数自加  
                        __blackNum++;
                    }
                    //大于阙值 白色  
                    else
                    {
                        img.SetPixel(x, y, Color.FromArgb(__c.A, w, w, w));
                    }
                }
            }
            return img;
        }
        #endregion
        #region 分割图片
        /// <summary>  
        /// 分割图片  
        /// </summary>  
        /// <returns>处理后的验证码</returns>  
        public List<int> CutImg()
        {
            var lstImage = new List<int>();
            //Y轴分割  
            CutY();
            //区域个数  
            __count = 0;
            if (XList.Count > 1)
            {
                //x起始值  
                int __start = XList[0];
                //x结束值  
                int __end = XList[XList.Count - 1];
                //x索引  
                int __idx = 0;
                while (__start != __end)
                {
                    //区域宽度  
                    int __w = __start;
                    //区域个数自加  
                    __count++;
                    while (XList.Contains(__w) && __idx < XList.Count)
                    {
                        //区域宽度自加  
                        __w++;
                        //x索引自加  
                        __idx++;
                    }
                    //区域X轴坐标  
                    int x = __start;
                    //区域Y轴坐标  
                    int y = 0;
                    //区域宽度  
                    int width = __w - __start;
                    //区域高度  
                    int height = img.Height;
                    /* 
                     * X轴分割当前区域 
                     */
                    CutX(img.Clone(new Rectangle(x, y, width, height), img.PixelFormat));
                    if (YList.Count > 1 && YList.Count != img.Height)
                    {
                        int y1 = YList[0];
                        int y2 = YList[YList.Count - 1];
                        if (y1 != 1)
                        {
                            y = y1 - 1;
                        }
                        height = y2 - y1 + 1;
                    }

                    var tmpPic = img.Clone(new Rectangle(x, y, width, height), img.PixelFormat);
                    var strPic = PixlPercent(tmpPic);

                    decimal tmpDec = 0M;
                    int key = 0;
                    bool hasGet = false;
                    foreach (var item in codeTable)
                    {
                        decimal disNumber = LevenshteinDistance.LevenshteinDistancePercent(item.Value, strPic);
                        if (disNumber == 1M)
                        {
                            hasGet = true;
                            lstImage.Add(item.Key);
                            break;
                        }
                        if (disNumber > tmpDec)
                        {
                            tmpDec = disNumber;
                            key = item.Key;
                        }
                    }

                    if (!hasGet)
                    {
                        lstImage.Add(key);
                    }
                    //GDI+绘图对象  
                    //Graphics g = Graphics.FromImage(img);
                    //g.SmoothingMode = SmoothingMode.HighQuality;
                    //g.CompositingMode = CompositingMode.SourceOver;
                    //g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    ////画出验证码区域  
                    //g.DrawRectangle(new Pen(Brushes.Green), );
                    //g.Dispose();
                    //起始值指向下一组  
                    if (__idx < XList.Count)
                    {
                        __start = XList[__idx];
                    }
                    else
                    {
                        __start = __end;
                    }

                }
            }
            return lstImage;
        }
        #endregion

        #region Y轴字符分割图片
        /// <summary>  
        /// 得到Y轴分割点  
        /// 判断每一竖行是否有黑色  
        /// 有则添加  
        /// </summary>  
        /// <param name="img">要验证的图片</param>  
        private void CutY()
        {
            XList.Clear();
            for (int x = 0; x < img.Width; x++)
            {
                isWhilteLine = false;
                for (int y = 0; y < img.Height; y++)
                {
                    __c = img.GetPixel(x, y);
                    if (__c.R == w)
                    {
                        isWhilteLine = true;
                    }
                    else
                    {
                        isWhilteLine = false;
                        break;
                    }
                }
                if (!isWhilteLine)
                {
                    XList.Add(x);
                }
            }
        }
        #endregion

        #region X轴字符分割图片
        /// <summary>  
        /// 得到X轴分割点  
        /// 判断每一横行是否有黑色  
        /// 有则添加  
        /// </summary>  
        /// <param name="tempImg">临时区域</param>  
        private void CutX(Bitmap tempImg)
        {
            YList.Clear();
            for (int x = 0; x < tempImg.Height; x++)
            {
                isWhilteLine = false;
                for (int y = 0; y < tempImg.Width; y++)
                {
                    __c = tempImg.GetPixel(y, x);
                    if (__c.R == w)
                    {
                        isWhilteLine = true;
                    }
                    else
                    {
                        isWhilteLine = false;
                        break;
                    }
                }
                if (!isWhilteLine)
                {
                    YList.Add(x);
                }
            }
            tempImg.Dispose();
        }
        #endregion
        #region 黑色像素比列
        /// <summary>  
        /// 计算黑色像素比列  
        /// </summary>  
        /// <param name="tempimg"></param>  
        /// <returns></returns>  
        public string PixlPercent(Bitmap tempimg)
        {
            var tmp = new StringBuilder();
            int w_h = tempimg.Width * tempimg.Height;
            for (int x = 0; x < tempimg.Width; x++)
            {
                for (int y = 0; y < tempimg.Height; y++)
                {
                    __c = tempimg.GetPixel(x, y);
                    if (__c.R == b)
                    {
                        tmp.Append('1');
                    }
                    else
                    {
                        tmp.Append('0');
                    }
                }
            }
            tempimg.Dispose();

            return tmp.ToString();
        }
        #endregion
    }
}
