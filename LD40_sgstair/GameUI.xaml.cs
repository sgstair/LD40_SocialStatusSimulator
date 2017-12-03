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
        RoundAction EndRoundAction = new RoundAction("End Round", "Relax for the rest of this quarter and advance to the next one.", "Cost: ");


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
            ActionScroll.Children.Clear();
            List<RoundAction> actions = HumanPlayer.PossibleActions();
            ActionItemControl c;

            foreach (RoundAction action in actions)
            {
                c = new ActionItemControl();
                c.BindAction(action);
                c.ActionClicked += ActionClicked;
                ActionScroll.Children.Add(c);
            }

            // Add a special action at the end of the list to end the quarter and advance the game.
            c = new ActionItemControl();
            EndRoundAction.CostString = GameFormat.FormatTime(HumanPlayer.ThisRound.TimeRemaining);
            c.BindAction(EndRoundAction);
            c.ActionClicked += ActionClicked;
            ActionScroll.Children.Add(c);
        }


        private void ActionClicked(RoundAction obj)
        {
            if (obj == EndRoundAction)
            {
                // Start the next round
                GameEngine.CompleteRound();
                GameEngine.BeginRound();
                UpdateUIForQuarter();


            }
            else
            {
                // Commit the action, and rebuild the UI to reflect the changes.
                HumanPlayer.CommitAction(obj);
                RebuildStatus();
                RebuildActionItems();
            }
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

            double sentimentPercent = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.PublicSentiment);
            LabelPublicSentiment.Content = string.Format("Public Sentiment ({0})", GameFormat.FormatPercent(sentimentPercent));

            double affinityProfessional = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinityProfessional);
            LabelProfessionalAffinity.Content = string.Format("Professional Affinity ({0})", GameFormat.FormatPercent(affinityProfessional));
            double affinityMedia = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinityMedia);
            LabelMediaAffinity.Content = string.Format("Media Affinity ({0})", GameFormat.FormatPercent(affinityMedia));
            double affinitySocial = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinitySocial);
            LabelSocialAffinity.Content = string.Format("Social Affinity ({0})", GameFormat.FormatPercent(affinitySocial));
            double affinityCriminal = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinityCriminal);
            LabelCriminalAffinity.Content = string.Format("Criminal Affinity ({0})", GameFormat.FormatPercent(affinityCriminal));

        }
    }
}
