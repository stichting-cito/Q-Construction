using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Citolab.QConstruction.Backend.HelperClasses
{
    public static class MediaHelper
    {
        private const int ThumbnailSize = 350;
        private const int ThumbnailQuality = 75;

        /// <summary>
        ///     Return a thumbnail size image of the specified image.
        /// </summary>
        /// <param name="mediaStream"></param>
        /// <returns></returns>
        public static byte[] GetThumbnail(Stream mediaStream)
        {
            byte[] result;
            using (var image = Image.FromStream(mediaStream))
            {
                result = GetResized(image, ThumbnailSize, ThumbnailQuality);
            }
            return result;
        }

        private static byte[] GetResized(Image image, int size, int quality)
        {
            byte[] result;

            int width, height;
            if (image.Width > image.Height)
            {
                width = size;
                height = Convert.ToInt32(image.Height * size / (double) image.Width);
            }
            else
            {
                width = Convert.ToInt32(image.Width * size / (double) image.Height);
                height = size;
            }
            var resized = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, width, height);

                var qualityParamId = Encoder.Quality;
                var encoderParameters = new EncoderParameters(1)
                {
                    Param = {[0] = new EncoderParameter(qualityParamId, quality)}
                };
                var codec = ImageCodecInfo.GetImageDecoders()
                    .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                using (var resizeStream = new MemoryStream())
                {
                    resized.Save(resizeStream, codec, encoderParameters);
                    result = resizeStream.ToArray();
                }
            }
            return result;
        }
    }
}