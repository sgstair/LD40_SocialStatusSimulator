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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartScreen.LinkedWindow = this;
            GameOverScreen.LinkedWindow = this;
            GameUI.LinkedWindow = this;

            Closing += MainWindow_Closing;
            RestoreWindowPosition();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowPosition();
        }

        void SaveWindowPosition()
        {
            Properties.Settings.Default.WindowX = Left.ToString();
            Properties.Settings.Default.WindowY = Top.ToString();

            Properties.Settings.Default.Save();
        }
        void RestoreWindowPosition()
        {
            int x, y;
            if (int.TryParse(Properties.Settings.Default.WindowX, out x))
            {
                if (int.TryParse(Properties.Settings.Default.WindowY, out y))
                {
                    Left = x;
                    Top = y;
                }
            }
        }

        public void MainMenu()
        {
            StartScreen.Visibility = Visibility.Visible;
            GameOverScreen.Visibility = Visibility.Hidden;
            GameUI.Visibility = Visibility.Hidden;
        }

        public void StartGame()
        {
            GameUI.StartNewGame();
            StartScreen.Visibility = Visibility.Hidden;
            GameOverScreen.Visibility = Visibility.Hidden;
            GameUI.Visibility = Visibility.Visible;
        }

        internal void GameOver(GamePlayer gameOverPlayer)
        {
            GameOverScreen.BindPlayer(gameOverPlayer);
            StartScreen.Visibility = Visibility.Hidden;
            GameUI.Visibility = Visibility.Hidden;
            GameOverScreen.Visibility = Visibility.Visible;
        }

        public void Exit()
        {
            Close();
        }

        public void StartTest()
        {
            StartGame();
            // Run test action
            GameUI.LaunchTest();
        }

    }
}
