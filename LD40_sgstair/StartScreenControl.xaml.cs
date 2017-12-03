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
    /// Interaction logic for StartScreenControl.xaml
    /// </summary>
    public partial class StartScreenControl : UserControl
    {
        public StartScreenControl()
        {
            InitializeComponent();
        }

        internal MainWindow LinkedWindow;

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            LinkedWindow?.StartGame();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            LinkedWindow?.Exit();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            LinkedWindow?.StartTest();
        }
    }
}
