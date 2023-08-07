using System.Drawing;

namespace NESHOP.Contacts
{
    public interface IBllBarcode
    {
        byte[] BitmapToBytes(Bitmap bitmap);

    }
}
