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
    /// Interaction logic for GameOverControl.xaml
    /// </summary>
    public partial class GameOverControl : UserControl
    {
        public GameOverControl()
        {
            InitializeComponent();
        }

        internal MainWindow LinkedWindow;

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            LinkedWindow?.MainMenu();
        }

        internal void BindPlayer(GamePlayer p)
        {
            StackDetails.Children.Clear();
            if(p.Dead)
            {
                LabelHeading.Content = "You Died";
                StackDetails.Children.Add(new Label() { Content = p.DeadHowDied });
                StackDetails.Children.Add(new Label() { Content = p.DeadContext });
                if(p.Retired)
                {
                    StackDetails.Children.Add(new Label() { Content = "(It's a shame, as they were just about to retire.)" });
                }
            }
            else if(p.Retired)
            {
                LabelHeading.Content = "Game Over";
                string location = "from the industry";
                if (p.InJail) location = "in a jail cell";
                StackDetails.Children.Add(new Label() { Content = $"{p.Name} Retired {location} at the age of {Math.Floor(p.Age)}" });
            }

            StackDetails.Children.Add(new Label() { Content = $"Final Money: {GameFormat.FormatMoney(p.Values.Money)}" });
            StackDetails.Children.Add(new Label() { Content = $"Final Fans: {GameFormat.FormatFans(p.Values.FanCount)}" });
            StackDetails.Children.Add(new Label() { Content = $"Final Rank: #{p.Values.Rank}" });
            double workPercent = 0;
            if (p.TotalDays > 0) workPercent = (double)p.WorkedDays / p.TotalDays;
            StackDetails.Children.Add(new Label() { Content = $"Productivity: Worked {p.WorkedDays} / {p.TotalDays} days ({workPercent*100:n2}%)" });

            if(p.FineCount > 0)
            {
                string s = p.FineCount == 1 ? "" : "s";
                StackDetails.Children.Add(new Label() { Content = $"Fined {p.FineCount} time{s} for a total of {GameFormat.FormatMoney(p.FineTotal)}" });
            }
            
            if(p.JailCount > 0)
            {
                string s = p.JailCount == 1 ? "" : "s";
                string s2 = p.JailQuarters == 1 ? "" : "s";
                StackDetails.Children.Add(new Label() { Content = $"Jailed {p.JailCount} time{s} for a total of {p.JailQuarters} Quarter{s2}" });
            }


        }
    }
}
