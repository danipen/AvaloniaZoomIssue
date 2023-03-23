using System;

using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;

namespace AvaloniaZoomIssue
{
    internal class ImageContentView : Panel
    {
        internal interface IImageView
        {
            Size ImageSize { get; }
            Size Size { get; set; }
            Layoutable Layoutable { get; }
            void SetZoomLevel(double zoomLevel);
        }

        internal static readonly StyledProperty<double> ZoomLevelProperty =
            AvaloniaProperty.Register<ImageContentView, double>("ZoomLevelProperty");

        internal double ZoomLevel
        {
            get { return GetValue<double>(ZoomLevelProperty); }
            set { SetValue<double>(ZoomLevelProperty, value); }
        }

        internal ScrollViewer ScrollViewer { get { return mScrollViewer; } }
        internal IImageView ImageView { get { return mImageView; } }

        internal ImageContentView(IImageView imageView)
        {
            BuildComponents(imageView);

            Transitions = new()
            {
                new DoubleTransition
                {
                    Duration = mDefaultZoomAnimationDuration,
                    Property = ZoomLevelProperty,
                    Easing = new CubicEaseOut(),
                }
            };
        }

        internal void InitZoom(double zoomLevel)
        {
            ZoomLevel = zoomLevel;

            SetupImageSize(zoomLevel);
        }

        internal void ZoomIn()
        {
            ZoomLevel += ZOOM_STEP;
        }

        internal void ZoomOut()
        {
            ZoomLevel = Math.Max(ZoomLevel - ZOOM_STEP, MIN_ZOOM_LEVEL);
        }

        internal void ZoomOneToOne()
        {
            ZoomLevel = 1;
        }

        internal void ZoomToValue(double value)
        {
            ZoomLevel = value;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ZoomLevelProperty)
            {
                OnZoomLevelChanged(change.GetNewValue<double>());
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            mDragOrigin = e.GetPosition(mImageView.Layoutable);
            mIsDragging = true;
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            mIsDragging = false;
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (mIsDragging)
            {
                Point delta = mDragOrigin - e.GetPosition(mImageView.Layoutable);

                mScrollViewer.Offset = new Vector(
                    mScrollViewer.Offset.X + delta.X,
                    mScrollViewer.Offset.Y + delta.Y);
            }
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);

            mIsDragging = false;
        }

        void OnZoomLevelChanged(double zoomLevel)
        {
            SetupImageSize(zoomLevel);
        }

        void SetupImageSize(double zoomLevel)
        {
            Size targetSize = new Size(
                mImageView.ImageSize.Width * zoomLevel, mImageView.ImageSize.Height * zoomLevel);

            mImageView.Size = targetSize;
        }

        void BuildComponents(IImageView imageView)
        {
            mImageView = imageView;

            mScrollViewer = new ScrollViewer();
            mScrollViewer.Content = mImageView;
            mScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            mScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            this.Children.Add(mScrollViewer);
        }

        Point mDragOrigin;
        bool mIsDragging;

        ScrollViewer mScrollViewer;
        IImageView mImageView;
        const double MIN_ZOOM_LEVEL = 0.08f;
        const double ZOOM_STEP = 0.3f;

        static TimeSpan mDefaultZoomAnimationDuration = TimeSpan.FromMilliseconds(300);
    }
}