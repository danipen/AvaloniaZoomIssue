using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

using System.IO;
using System.Net;

namespace AvaloniaZoomIssue
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ImageView imageView = new ImageView();
            imageView.SetImage(new Bitmap(@"..\..\..\sample.jpg"));

            mImageContentView = new ImageContentView(imageView);
            mImageContentView.ZoomOneToOne();

            mZoomIn.Click += ZoomIn_Click;
            mZoomOut.Click += ZoomOut_Click;

            mContentPanel.Children.Add(mImageContentView);
        }

        private void ZoomOut_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            mImageContentView.ZoomOut();
        }

        private void ZoomIn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            mImageContentView.ZoomIn();
        }

        ImageContentView mImageContentView;
    }
}