using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AvaloniaZoomIssue
{
    internal class ImageView : UserControl, ImageContentView.IImageView
    {
        // Testing
        internal IImage Image { get { return mImage; } }

         internal void SetImage(IImage image)
        {
            mImage = image;

            InvalidateVisual();
        }

        internal void CleanImage()
        {
            mImage = null;

            InvalidateVisual();
        }

        Size ImageContentView.IImageView.ImageSize
        {
            get
            {
                if (mImage == null)
                    return default;

                return mImage.Size;
            }
        }

        Size ImageContentView.IImageView.Size
        {
            get { return Bounds.Size; }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        Layoutable ImageContentView.IImageView.Layoutable
        {
            get { return this; }
        }

        void ImageContentView.IImageView.SetZoomLevel(double zoomLevel) { }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (mImage == null)
                return;

            Rect boundsRect = new Rect(0, 0, Bounds.Width, Bounds.Height);
            Rect imageRect = new Rect(0, 0, mImage.Size.Width, mImage.Size.Height);

            DrawImage(context, mImage, boundsRect, imageRect);
        }

        void DrawImage(
            DrawingContext context,
            IImage image,
            Rect boundsRect,
            Rect imageRect)
        {
            context.DrawImage(
                image,
                imageRect,
                boundsRect,
                BitmapInterpolationMode.MediumQuality);
        }

        IPen mBorderPen;
        IImage mImage;
    }
}
