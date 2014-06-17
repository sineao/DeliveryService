using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace DeliveryService.Utility
{
    public class Utilities
    {
        #region ResizeImage
        public static byte[] ResizeImage(byte[] originalFile, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
        {
            MemoryStream ms = new MemoryStream(originalFile);
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromStream(ms);

            ImageFormat imageFormat = ImageFormat.Png;
            if (FullsizeImage.RawFormat == ImageFormat.Png)
                imageFormat = ImageFormat.Png;
            else if (FullsizeImage.RawFormat == ImageFormat.Jpeg)
                imageFormat = ImageFormat.Jpeg;
            else if (FullsizeImage.RawFormat == ImageFormat.Gif)
                imageFormat = ImageFormat.Gif;

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (OnlyResizeIfWider)
            {
                if (FullsizeImage.Width <= NewWidth)
                {
                    NewWidth = FullsizeImage.Width;
                }
            }

            int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                NewHeight = MaxHeight;
            }

            System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

            //return resized picture
            ms = new MemoryStream();

            NewImage.Save(ms, imageFormat);

            FullsizeImage.Dispose();

            return ms.ToArray();
        }
        #endregion

        #region MapURL
        public static string MapURL(string AbsolutePath)
        {
            return AbsolutePath.Replace(System.Web.HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], "/").Replace('\\','/');
        }
        #endregion
    }
}