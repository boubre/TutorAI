using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace DynamicGeometry.UI.RegionShading
{
    [Category(BehaviorCategories.Regions)]
    [Order(1)]
    public class ShadedRegionCreator : Behavior
    {
        public override string Name
        {
            get { return "Mark Region"; }
        }

        public override FrameworkElement CreateIcon()
        {
            var uri = new Uri("LiveGeometry;component/MarkRegion.png", UriKind.Relative);
            var streamInfo = Application.GetResourceStream(uri);
            BitmapImage source = new BitmapImage();
            source.SetSource(streamInfo.Stream);
            return new Image() { Source = source, Stretch = Stretch.None };
        }

        //Code just shows how to add a bmp to canvas.
        /*void MarkRegion()
        {
            WriteableBitmap bmp = new WriteableBitmap(100, 100);
            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                {
                    int pixel;
                    if ((i + j) % 12 <= 3)
                        pixel = unchecked((int)0xFFFF0000);
                    else if ((i + j) % 12 <= 7)
                        pixel = unchecked((int)0xFF00FF00);
                    else
                        pixel = unchecked((int)0xFF0000FF);

                    bmp.Pixels[i * 100 + j] = pixel;
                }

            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = bmp;
            Canvas.SetTop(img, 50);
            Canvas.SetLeft(img, 30);
            drawingHost.CurrentDrawing.Canvas.Children.Add(img);
        }*/
    }
}
