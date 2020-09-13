using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ariane.Controls
{
    public class AutoDisableImage : Image
    {
        protected override void OnRender(DrawingContext dc)
        {
            BitmapSource bitmapSource = Source as BitmapSource;
            if (bitmapSource == null)
            {
                return;
            }

            if (IsEnabled)
            {
                // Disable gray
                bitmapSource = new FormatConvertedBitmap(bitmapSource, PixelFormats.Gray32Float, null, 0);
            }

            dc.DrawImage(bitmapSource, new Rect(new Point(), RenderSize));
        }
    }
}
