using NESHOP.Contacts;
using System.Drawing.Imaging;
using System.Drawing;

namespace NESHOP.DAL
{
    public class BllBarcode: IBllBarcode
    {
        public byte[] BitmapToBytes(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

    }
}
