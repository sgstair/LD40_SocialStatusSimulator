using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LD40_sgstair
{
    /// <summary>
    /// Interaction logic for PercentBar.xaml
    /// </summary>
    public partial class PercentBar : UserControl
    {
        public PercentBar()
        {
            InitializeComponent();
            SizeChanged += PercentBar_SizeChanged;
        }

        private void PercentBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        double PercentOriginal, PercentNow;
        bool TwoSided;

        public void SetPercent(double originalPercent, double nowPercent, bool twoSided)
        {
            PercentOriginal = originalPercent;
            PercentNow = nowPercent;
            TwoSided = twoSided;
            Redraw();
        }

        public void Redraw()
        {
            double w = ActualWidth;

            double xbase = 0;
            double dx = ActualWidth;
            if(TwoSided)
            {
                xbase = ActualWidth / 2;
                dx = ActualWidth / 2;
            }

            DrawRectangle(xbase, xbase + PercentOriginal * dx, ref OriginalFill);
            DrawRectangle(xbase, xbase + PercentNow * dx, ref NewFill);

            if (PercentOriginal < 0)
            {
                // Red
                OriginalFill.Fill = Brushes.DarkRed;
            }
            else
            {
                // Green
                OriginalFill.Fill = Brushes.DarkGreen;
            }

            if(PercentNow < 0)
            {
                NewFill.Fill = Brushes.Red;
            }
            else
            {
                NewFill.Fill = Brushes.Green;
            }

        }

        public void DrawRectangle(double x1, double x2, ref Rectangle rc)
        {
            double w = ActualWidth;

            Thickness th;
            th = rc.Margin;
            if(x2<x1)
            {
                double temp = x2;
                x2 = x1;
                x1 = temp;
            }

            th.Left = x1;
            th.Right = w - x2;

            rc.Margin = th;
        }

    }
}
