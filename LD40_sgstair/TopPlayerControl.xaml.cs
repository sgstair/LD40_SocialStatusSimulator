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
    /// Interaction logic for TopPlayerControl.xaml
    /// </summary>
    public partial class TopPlayerControl : UserControl
    {
        public TopPlayerControl()
        {
            InitializeComponent();
        }

        GamePlayer Player;

        internal void BindPlayer(GamePlayer gamePlayer)
        {
            Player = gamePlayer;

            LabelName.Content = Player.Name;
            LabelRank.Content = Player.Values.Rank;
            int dRank = Player.Values.Rank - Player.LastRound.Rank;

            if(dRank == 0)
            {
                LabelArrow.Content = "="; // circle in webdings
                LabelArrow.Foreground = Brushes.DarkGoldenrod;
            }
            else if(dRank > 0)
            {
                LabelArrow.Content = "5"; // Up arrow
                LabelArrow.Foreground = Brushes.DarkGreen;
            }
            else
            {
                LabelArrow.Content = "6"; // Down arrow
                LabelArrow.Foreground = Brushes.DarkRed;
            }

            string description = "Age: " + (int)Math.Floor(Player.Age);
            if(Player.Dead) { description = $"Deceased"; }

            LabelDescription.Content = description;

            LabelFans.Content = GameFormat.FormatFans(Player.Values.FanCount);
            LabelMoney.Content = GameFormat.FormatMoney(Player.Values.Money);

        }
    }
}
