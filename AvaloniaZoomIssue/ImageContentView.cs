using System;

using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
        }

        internal void InitZoom(double zoomLevel)
        {
            ZoomLevel = zoomLevel;

            SetupImageSize(zoomLevel);
        }

        internal void ZoomIn()
        {
            AnimateZoomLevel(
                ZoomLevel,
                ZoomLevel + ZOOM_STEP,
                mDefaultZoomAnimationDuration);
        }

        internal void ZoomOut()
        {
            AnimateZoomLevel(
                ZoomLevel,
                ZoomLevel - ZOOM_STEP,
                mDefaultZoomAnimationDuration);
        }

        internal void ZoomOneToOne()
        {
            AnimateZoomLevel(
                ZoomLevel,
                1,
                mDefaultZoomAnimationDuration);
        }

        internal void ZoomToValue(double value)
        {
            AnimateZoomLevel(
                ZoomLevel,
                value,
                mDefaultZoomAnimationDuration);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ZoomLevelProperty)
            {
                ZoomLevel = change.GetNewValue<double>();
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

        void AnimateZoomLevel(double from, double to, TimeSpan duration)
        {
            if (to < MIN_ZOOM_LEVEL)
                to = MIN_ZOOM_LEVEL;

            DoubleTransition doubleTransition = new DoubleTransition();
            doubleTransition.Duration = duration;
            doubleTransition.Property = ZoomLevelProperty;
            doubleTransition.Easing = new CubicEaseOut();

            doubleTransition.Apply(
                this, Clock ?? AvaloniaLocator.Current.GetService<IGlobalClock>(),
                from,
                to);
        }

        void OnZoomLevelChanged(double zoomLevel)
        {
            SetupImageSize(zoomLevel);
        }

        void SetupImageSize(double zoomLevel)
        {
            Size targetSize = new Size(
                mImageView.ImageSize.Width * zoomLevel, mImageView.ImageSize.Height * zoomLevel);

            mImageView.SetZoomLevel(zoomLevel);
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