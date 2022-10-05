using Api.Entity;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Utilities
{
    public class QRCodeHelper
    {
        public static string Generator(string content, string text)
        {
            try
            {
                //创建一个新的QRCodeGenerator实例
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);
                //放入生成的二维码
                QRCode qrCode = new QRCode(qrCodeData);
                //获取到二维码图形
                Bitmap qrCodeImage = qrCode.GetGraphic(15, Color.Black, Color.White, false);

                /* GetGraphic方法参数说明
                 * public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)
                 * int pixelsPerModule:生成二维码图片的像素大小 ，我这里设置的是5 
                 * Color darkColor：暗色   一般设置为Color.Black 黑色
                 * Color lightColor:亮色   一般设置为Color.White  白色
                 * Bitmap icon :二维码 水印图标 例如：Bitmap icon = new Bitmap(路径); 默认为NULL ，加上这个二维码中间会显示一个图标
                 * int iconSizePercent： 水印图标的大小比例 ，可根据自己的喜好设置 
                 * int iconBorderWidth： 水印图标的边框
                 * bool drawQuietZones:静止区，位于二维码某一边的空白边界,用来阻止读者获取与正在浏览的二维码无关的信息 即是否绘画二维码的空白边框区域 默认为true
                */

                //新图形(给出自定义大小,可以解决二维码生成时因为内容而影响图片大小的问题)
                /*
                 * 250为宽高 
                 * +30为上下左右各留出15的空白区域
                 * 35为我要给图片底下添加字体 最一行行 35是行高 提前调试得知
                 */
                Bitmap newBM = new Bitmap(250 + 30, 250 + (35) + 15);
                //新画布
                Graphics newGP = Graphics.FromImage(newBM);
                //清除所有背景色并指定背景颜色
                newGP.Clear(Color.White);
                // 插值算法的质量
                newGP.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //将旧图画入新图中
                /*
                 * qrCodeImage:旧图
                 * new Rectangle(15, 15, 250, 250):在画布上操作的定位及宽高(前两个参数为距左边,距顶部,后边为宽高)  此处宽高为自定义
                 * new Rectangle(0, 0, qrCodeImage.Width, qrCodeImage.Height):要操作图片的定位及宽高
                 * GraphicsUnit.Pixel:使用像素为单位
                 */
                newGP.DrawImage(qrCodeImage, new Rectangle(15, 15, 250, 250), new Rectangle(0, 0, qrCodeImage.Width, qrCodeImage.Height), GraphicsUnit.Pixel);
                //设置字体
                Font font = new Font("楷体", 30f, FontStyle.Bold, GraphicsUnit.Pixel);

                //以下为文字居中处理(可以换行)
                RectangleF rec = new RectangleF((float)(newBM.Width * 0.10), newBM.Height - (35 + 15), (float)(newBM.Width * 0.80), font.Height * 2);
                Brush fontBrush = SystemBrushes.ControlText;

                //此处为设置居中方式可以让换行后的文字也居中
                StringFormat sformat = new StringFormat();
                sformat.Alignment = StringAlignment.Center;
                sformat.LineAlignment = StringAlignment.Center;
                newGP.DrawString(text, font, fontBrush, rec, sformat);
                //资源释放
                newGP.Dispose();

                string picPath = $"{Config.NewAttachFolder}/{Guid.NewGuid()}.jpg";

                newBM.Save(picPath);

                return picPath;
            }
            catch (Exception)
            {
                throw new MsgException("二维码生成失败，请联系系统管理员！");
            }
        }
    }
}
