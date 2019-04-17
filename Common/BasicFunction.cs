using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography;
using System.Drawing;
using System.Web.Security;

namespace Common
{
    public static class BasicFunction
    {

        /// <summary> 
        /// SHA1加密字符串 
        /// </summary> 
        /// <param name="source">源字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public static string SHA1(string source)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "SHA1");
        } 

        #region 获取验证码
        
        public static string GetCode()
        {
            Random rd = new Random();
            int newPwd = rd.Next(1000,9999);
            return newPwd.ToString();
        }

        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">待加密字符串</param>
        /// <param name="code">16||32</param>
        /// <returns></returns>
        public static string MD5JiaMi(string str, int code)
        {
            if (code == 16) //16位MD5加密（取32位加密的9~25字符）
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }
            else//32位加密
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
            }
        }
        #endregion

        #region 过滤Html代码

        /// <summary>
        /// 过滤Html代码
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static string WebCodeFiler(string sourceStr)
        {
            if (String.IsNullOrEmpty(sourceStr) || sourceStr.Equals(""))
            {
                return "";
            }

            sourceStr = sourceStr.Replace("<", "＜");
            sourceStr = sourceStr.Replace(">", "＞");
            sourceStr = sourceStr.Replace("\\", "＼");
            sourceStr = sourceStr.Replace("/", "／");
            sourceStr = sourceStr.Replace("\'", "＇");
            sourceStr = sourceStr.Replace("\"", "＂");
            //sourceStr = sourceStr.Replace(",", "，");
            sourceStr = sourceStr.Replace("#", "＃");
            sourceStr = sourceStr.Replace("*", "＊");
            sourceStr = sourceStr.Replace(";", "；");
            sourceStr = sourceStr.Replace("--", "——");
            //sourceStr = sourceStr.Replace("_", "——");
            sourceStr = sourceStr.Replace("(", "（");
            sourceStr = sourceStr.Replace(")", "）");
            sourceStr = sourceStr.Replace("{", "｛");
            sourceStr = sourceStr.Replace("}", "｝");
            sourceStr = sourceStr.Replace("[", "［");
            sourceStr = sourceStr.Replace("]", "］");
            //sourceStr = sourceStr.Replace("script", "ｓｃｒｉｐｔ");

            return sourceStr;

        }

        #endregion

        #region 过滤SQL敏感字符
        public static string SqlCodeFilter(string sourceStr)
        {
            if (String.IsNullOrEmpty(sourceStr) || sourceStr.Equals(""))
            {
                return "";
            }

            sourceStr = sourceStr.Replace("'", "\"");
            //sourceStr = sourceStr.Replace(",", "，");
            sourceStr = sourceStr.Replace("#", "＃");
            sourceStr = sourceStr.Replace("*", "＊");
            sourceStr = sourceStr.Replace("/", "／");
            sourceStr = sourceStr.Replace("%", "％");
            //sourceStr = sourceStr.Replace(";", "；");
            sourceStr = sourceStr.Replace("--", "——");
            //sourceStr = sourceStr.Replace("_", "——");
            sourceStr = sourceStr.Replace("(", "（");
            sourceStr = sourceStr.Replace(")", "）");
            sourceStr = sourceStr.Replace("{", "｛");
            sourceStr = sourceStr.Replace("}", "｝");
            sourceStr = sourceStr.Replace("[", "［");
            sourceStr = sourceStr.Replace("]", "］");
            sourceStr = sourceStr.Replace("“", "\"");
            sourceStr = sourceStr.Replace("”", "\"");

            //sourceStr = sourceStr.Replace("exec", "ｅｘｅｃ");
            sourceStr = sourceStr.Replace("xp_", "ｘｐ＿");
            sourceStr = sourceStr.Replace("sp_", "ｓｐ＿");
            //sourceStr = sourceStr.Replace("declare", "ｄｅｃｌａｒｅ");
            //sourceStr = sourceStr.Replace("union", "ｕｎｉｏｎ");
            //sourceStr = sourceStr.Replace("cmd", "ｃｍｄ");


            return sourceStr;
        }

        #endregion

        #region 过滤敏感词

        /// <summary>
        /// 过滤敏感词
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="filerStr"></param>
        /// <returns></returns>
        public static string MinGanWordFiler(string sourceStr, string filerStr)
        {
            if (String.IsNullOrEmpty(sourceStr) || sourceStr.Equals(""))
            {
                return "";
            }

            if (String.IsNullOrEmpty(filerStr) || filerStr.Equals(""))
            {
                return sourceStr;
            }

            string[] filerStrL = filerStr.Split(',');
            foreach (string str in filerStrL)
            {
                if (String.IsNullOrEmpty(str) || str.Equals(""))
                {

                }
                else
                {
                    sourceStr = sourceStr.Replace(str, "×××");
                }
            }

            return sourceStr;
        }

        #endregion

        #region 计算汉字的字数
        /// <summary>
        /// 计算汉字的字数
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static int HanZiCount(
            string strSource
            )
        {
            if (String.IsNullOrEmpty(strSource)) return 0;

            return Regex.Matches(strSource, "[\u4e00-\u9fa5]").Count;
        }

        public static int HanZiCount2(
            string strSource
            )
        {
            if (String.IsNullOrEmpty(strSource)) return 0;

            return Regex.Matches(strSource, "[\u4400-\u9fa5\u3000-\u3003\uff00-\uff5e]]").Count;
        }

        #endregion

        #region 字数统计，去除html，空格
        /// <summary>
        /// 字数统计，去除html，空格
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static int WordNumCount(
            string strText
            )
        {
            if (String.IsNullOrEmpty(strText))
            {
                return 0;
            }

            strText = System.Text.RegularExpressions.Regex.Replace(strText, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, " ", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "\n", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "\r", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "\\s", "");

            strText = CharConver(strText);

            return strText.Length;
        }

        #endregion

        #region html代码过滤
        public static string HtmlFilter(
            string strText
            )
        {
            if (String.IsNullOrEmpty(strText))
            {
                return "";
            }

            strText = System.Text.RegularExpressions.Regex.Replace(strText, "<[^>]+>", "");


            return strText;
        }

        #endregion

        #region 判断是否汉字
        public static bool IsHanZi(string str)
        {
            if (str == null || str.Equals(""))
            {
                return false;
            }
            else
            {
                string parm = @"[\u4e00-\u9fa5]+";
                Regex rg = new Regex(parm);
                Match mh = rg.Match(str);//myChar是要比较的字符 
                if (mh.Success)
                {
                    //是汉字
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region 控制上传图片显示大小
        public static void ControlPicSize(double oldWidth, double oldHeight, double controlWidth, double controlHeight, out double newWidth, out double newHeight)
        {
            double limitWidth = 0;
            double limitHeight = 0;

            if (controlWidth.Equals(0))
            {
                limitWidth = 480;
            }
            else
            {
                limitWidth = controlWidth;
            }

            if (controlHeight.Equals(0))
            {
                limitHeight = 320;
            }
            else
            {
                limitHeight = controlHeight;
            }

            if (oldWidth == 0 || oldHeight == 0)
            {
                newWidth = 0;
                newHeight = 0;
            }
            else
            {
                if ((oldWidth > limitWidth) && (oldHeight > limitHeight))
                {
                    if (oldWidth > oldHeight)
                    {
                        newWidth = limitWidth;
                        newHeight = Convert.ToInt32((limitWidth / oldWidth) * oldHeight);
                    }
                    else
                    {
                        newHeight = limitHeight;
                        newWidth = Convert.ToInt32((limitHeight / oldHeight) * oldWidth);
                    }
                }
                else if ((oldWidth < limitWidth) && (oldHeight < limitHeight))
                {
                    newWidth = oldWidth;
                    newHeight = oldHeight;
                }
                else
                {
                    if ((oldWidth > limitWidth) && (oldHeight < limitHeight))
                    {
                        newWidth = limitWidth;
                        newHeight = Convert.ToInt32((limitWidth / oldWidth) * oldHeight);
                    }
                    else
                    {
                        newHeight = limitHeight;
                        newWidth = Convert.ToInt32((limitHeight / oldHeight) * oldWidth);
                    }
                }
            }
        }

        #endregion

        #region 字符转换函数
        /// <summary>
        /// 转换特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CharConver(
            string str
            )
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }

            str = str.Replace("&quot;", "\"");
            str = str.Replace("&amp;", "&");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&nbsp;", "");
            str = str.Replace("&ldquo;", "“");
            str = str.Replace("&rdquo;", "”");
            str = str.Replace("&lsquo;", "‘");
            str = str.Replace("&rsquo;", "’");

            str = str.Replace("&iexcl;", "?");
            str = str.Replace("&cent;", "￠");
            str = str.Replace("&pound;", "￡");
            str = str.Replace("&curren;", "¤");
            str = str.Replace("&yen;", "￥");
            str = str.Replace("&brvbar;", "|");
            str = str.Replace("&sect;", "§");
            str = str.Replace("&uml;", "¨");
            str = str.Replace("&copy;", "©");
            str = str.Replace("&ordf;", "a");
            str = str.Replace("&laquo;", "?");
            str = str.Replace("&not;", "?");
            str = str.Replace("&shy;", "\x7f");
            str = str.Replace("&reg;", "®");
            str = str.Replace("&macr;", "ˉ");
            str = str.Replace("&deg;", "°");
            str = str.Replace("&plusmn;", "±");
            str = str.Replace("&acute;", "′");
            str = str.Replace("&micro;", "μ");
            str = str.Replace("&para;", "?");
            str = str.Replace("&middot;", "·");
            str = str.Replace("&cedil;", "?");
            str = str.Replace("&ordm;", "o");
            str = str.Replace("&raquo;", "?");
            str = str.Replace("&frac14;", "?");
            str = str.Replace("&frac12;", "?");
            str = str.Replace("&frac34;", "?");
            str = str.Replace("&iquest;", "?");
            str = str.Replace("&Agrave;", "À");

            str = str.Replace("&Aacute;", "Á");
            str = str.Replace("&circ;", "Â");
            str = str.Replace("&Atilde;", "Ã");
            str = str.Replace("&Auml", "Ä");
            str = str.Replace("&ring;", "Å");
            str = str.Replace("&AElig;", "Æ");
            str = str.Replace("&Ccedil;", "Ç");
            str = str.Replace("&Egrave;", "È");
            str = str.Replace("&Eacute;", "É");
            str = str.Replace("&Ecirc;", "Ê");
            str = str.Replace("&Euml;", "Ë");
            str = str.Replace("&Igrave;", "Ì");
            str = str.Replace("&Iacute;", "Í");
            str = str.Replace("&Icirc;", "Î");
            str = str.Replace("&Iuml;", "Ï");
            str = str.Replace("&ETH;", "Ð");
            str = str.Replace("&Ntilde;", "Ñ");
            str = str.Replace("&Ograve;", "Ò");
            str = str.Replace("&Oacute;", "Ó");
            str = str.Replace("&Ocirc;", "Ô");
            str = str.Replace("&Otilde;", "Õ");
            str = str.Replace("&Ouml;", "Ö");
            str = str.Replace("&times;", "&times;");
            str = str.Replace("&Oslash;", "Ø");
            str = str.Replace("&Ugrave;", "Ù");
            str = str.Replace("&Uacute;", "Ú");
            str = str.Replace("&Ucirc;", "Û");
            str = str.Replace("&Uuml;", "Ü");
            str = str.Replace("&Yacute;", "Ý");
            str = str.Replace("&THORN;", "Þ");
            str = str.Replace("&szlig;", "ß");
            str = str.Replace("&agrave;", "à");

            str = str.Replace("&aacute;", "á");
            str = str.Replace("&acirc;", "â");
            str = str.Replace("&atilde;", "ã");
            str = str.Replace("&auml;", "ä");
            str = str.Replace("&aring;", "å");
            str = str.Replace("&aelig;", "æ");
            str = str.Replace("&ccedil;", "ç");
            str = str.Replace("&egrave;", "è");
            str = str.Replace("&eacute;", "é");
            str = str.Replace("&ecirc;", "ê");
            str = str.Replace("&euml;", "ë");
            str = str.Replace("&igrave;", "ì");
            str = str.Replace("&iacute;", "í");
            str = str.Replace("&icirc;", "î");
            str = str.Replace("&iuml;", "ï");
            str = str.Replace("&ieth;", "ð");
            str = str.Replace("&ntilde;", "ñ");
            str = str.Replace("&ograve;", "ò");
            str = str.Replace("&oacute;", "ó");
            str = str.Replace("&ocirc;", "ô");
            str = str.Replace("&otilde;", "õ");
            str = str.Replace("&ouml;", "ö");
            str = str.Replace("&divide;", "÷");
            str = str.Replace("&oslash;", "ø");
            str = str.Replace("&ugrave;", "ù");
            str = str.Replace("&uacute;", "ú");
            str = str.Replace("&ucirc;", "û");
            str = str.Replace("&uuml;", "ü");
            str = str.Replace("&yacute;", "ý");
            str = str.Replace("&thorn;", "þ");
            str = str.Replace("&yuml;", "ÿ");


            return str;
        }

        #endregion

        #region 将字符转换成decimal
        /// <summary>
        /// 将字符转换成decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ConvertToDecimal(object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion

        #region 生成GUID全球唯一号
        /// <summary>
        /// 生成GUID全球唯一号
        /// </summary>
        /// <returns></returns>
        public static string CreateGuid()
        {
            Guid g = Guid.NewGuid();
            return g.ToString().Replace("-", "");
        }

        #endregion

        #region 生成缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "WC":
                    if (originalImage.Width > width)
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    else
                    {
                        towidth = originalImage.Width;
                        toheight = originalImage.Height;
                    }
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion

        #region 生成图片缩略图
        /// <summary>  
        /// 在图片上生成图片水印
        /// </summary>  
        /// <param name="Path">原服务器图片路径 </param>  
        /// <param name="Path_syp">生成的带图片水印的图片路径 </param>  
        /// <param name="Path_sypf">水印图片路径 </param>  
        public static void AddWaterPic(string Path, string Path_syp, string Path_sypf,string style)
        {
            Path = HttpContext.Current.Server.MapPath(Path);
            Path_syp = HttpContext.Current.Server.MapPath(Path_syp);

            System.Drawing.Image image = System.Drawing.Image.FromFile(Path);
            System.Drawing.Image copyImage = System.Drawing.Image.FromFile(Path_sypf);
            Graphics g = Graphics.FromImage(image);
            int x=0;
            int y=0;

            switch (style)
            {
                case "C":
                    x = image.Width/2 - copyImage.Width/2;
                    y = Convert.ToInt32(image.Height * 0.8) - copyImage.Height/2;
                    break;
                case "RD":
                    x = (image.Width - copyImage.Width) - 20;
                    y = (image.Height - copyImage.Height) - 20;
                    break;
            }

            Rectangle rect = new Rectangle(x, y, copyImage.Width, copyImage.Height);
            g.DrawImage(copyImage, rect, 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
            g.Dispose();

            image.Save(Path_syp);
            image.Dispose();
        }
        #endregion
    }
}
