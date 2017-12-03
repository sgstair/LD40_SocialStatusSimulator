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
    /// Interaction logic for GameUI.xaml
    /// </summary>
    public partial class GameUI : UserControl
    {
        public GameUI()
        {
            InitializeComponent();
        }

        Engine GameEngine;
        GamePlayer HumanPlayer;

        public void StartNewGame()
        {
            GameEngine = new Engine();
            HumanPlayer = GameEngine.PrepareSinglePlayerGame();

            GameEngine.BeginGame();
            GameEngine.BeginRound();
            UpdateUIForQuarter();
        }

        void UpdateUIForQuarter()
        {
            RebuildNewsItems();
            RebuildActionItems();
            RebuildStatus();
        }


        void RebuildNewsItems()
        {

        }
        void RebuildActionItems()
        {

        }
        void RebuildStatus()
        {
            // Top status area
            LabelName.Content = HumanPlayer.Name;
            LabelPlayerPosition.Content = "(#?)"; // Todo: compute
            LabelAge.Content = string.Format("Age: {0}", (int)Math.Floor(HumanPlayer.Age));
            LabelTime.Content = string.Format("Q{0} {1}", GameEngine.Quarter, GameEngine.CurrentYear);
            LabelMoney.Content = GameFormat.FormatMoney(HumanPlayer.ThisRound.Money);
            double moneyPercent = 0;
            if (HumanPlayer.Values.Money != 0) moneyPercent = (double)HumanPlayer.ThisRound.Money / HumanPlayer.Values.Money - 1;
            LabelMoneyChange.Content = string.Format("({0})", GameFormat.FormatPercent(moneyPercent));
            LabelMoneyChange.Foreground = moneyPercent < 0 ? Brushes.DarkRed : Brushes.DarkGreen;

            LabelFans.Content = GameFormat.FormatFans(HumanPlayer.ThisRound.FanCount);
            double fanPercent = 0;
            if (HumanPlayer.Values.FanCount != 0) fanPercent = (double)HumanPlayer.ThisRound.FanCount / HumanPlayer.Values.FanCount - 1;
            LabelFansChange.Content = string.Format("({0})", GameFormat.FormatPercent(fanPercent));
            LabelFansChange.Foreground = fanPercent < 0 ? Brushes.DarkRed : Brushes.DarkGreen;

            // Side panel status
            LabelTimeRemaining.Content = string.Format("Time Remaining ({0})", GameFormat.FormatTime(HumanPlayer.ThisRound.TimeRemaining));
        }
    }
}
