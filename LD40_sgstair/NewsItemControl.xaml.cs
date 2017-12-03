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
    /// Interaction logic for NewsItemControl.xaml
    /// </summary>
    public partial class NewsItemControl : UserControl
    {
        public NewsItemControl()
        {
            InitializeComponent();
        }
        Brush defaultBrush = new SolidColorBrush(Color.FromArgb(0x99, 255, 255, 255));
        Brush ImportantBrush = new SolidColorBrush(Color.FromArgb(0x99, 255, 215, 215));
        MediaEvent News;

        internal void BindNews(MediaEvent newsItem)
        {
            News = newsItem;

            LabelHeader.Content = News.Heading;
            LabelDescription.Content = News.SubHeading;
            LabelOutcome.Content = News.OutcomeText;
            if(newsItem.Important)
            {
                RectBackground.Fill = ImportantBrush;
            }
        }

    }
}
