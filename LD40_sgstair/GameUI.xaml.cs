using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            AnimStatus = new Storyboard();
        }

        internal MainWindow LinkedWindow;

        Storyboard AnimStatus;

        Engine GameEngine;
        GamePlayer HumanPlayer;
        RoundAction EndRoundAction = new RoundAction("End Round", "Relax for the rest of this quarter and advance to the next one.", "Cost: ");
        RoundAction RetireAction = new RoundAction("Retire", "You've had your fun, get out of this crazy rat race here.", null);


        public void StartNewGame()
        {
            GameEngine = new Engine();
            HumanPlayer = GameEngine.PrepareSinglePlayerGame();

            GameEngine.BeginGame();
            GameEngine.BeginRound();
            UpdateUIForQuarter();
        }

        public void LaunchTest()
        {
            TestRound();
        }

        void TestRound(int index = 0)
        {
            if (index >= 1000) return;

            ThreadPool.QueueUserWorkItem((context) =>
            {
                Dispatcher.Invoke(() =>
                {
                    // Run a quarter of simulation
                    GameEngine.CompleteRound();
                    GameEngine.BeginRound();
                    UpdateUIForQuarter();
                });
                // Recursively call self (to queue again)
                Thread.Sleep(15);
                TestRound(index + 1);
            });
        }

        void UpdateUIForQuarter()
        {
            RebuildNewsItems();
            RebuildActionItems();
            RebuildTop50Items();
            RebuildStatus();
        }

        void SetStatusText(string text, Brush color)
        {
            DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromPercent(0)));
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromPercent(0.9)));
            anim.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromPercent(1)));
            anim.Duration = TimeSpan.FromSeconds(7);

            LabelStatus.Content = text;
            LabelStatus.Foreground = color;

            AnimStatus.Children.Clear();
            AnimStatus.Children.Add(anim);
            Storyboard.SetTargetName(anim, LabelStatus.Name);
            Storyboard.SetTargetProperty(anim, new PropertyPath(Label.OpacityProperty));

            AnimStatus.Begin(this);
        }


        void RebuildNewsItems(bool initialRebuild = false)
        {
            NewsScroll.Children.Clear();
            List<MediaEvent> newsEvents = HumanPlayer.GetPlayerNews();
            bool HasImportantNews = false;
            int index = 0;
            foreach (MediaEvent e in newsEvents)
            {
                NewsItemControl c = new NewsItemControl();
                c.BindNews(e);
                NewsScroll.Children.Add(c);
                if (e.Important) HasImportantNews = true;
                // If this media event has reactions, add them to the list
                if (e.Important)
                {
                    List<RoundAction> actions = HumanPlayer.MediaActions(e, index);
                    if(actions.Count > 0)
                    {
                        // Build a container to hold these items indented
                        StackPanel p = new StackPanel() { Margin = new Thickness(10, 0, 0, 0) };
                        NewsScroll.Children.Add(p);

                        foreach (RoundAction action in actions)
                        {
                            ActionItemControl ac = new ActionItemControl();
                            ac.BindAction(action);
                            ac.ActionClicked += ActionClicked;
                            p.Children.Add(ac);
                        }
                    }
                }
                index++;
            }

            if (HumanPlayer.OldNews.Count > 0)
            {
                NewsScroll.Children.Add(new Label() { Content = "Older News:", FontSize = 18, Foreground = Brushes.White });
            }

            foreach(MediaEvent e in HumanPlayer.OldNews)
            {
                NewsItemControl c = new NewsItemControl();
                c.BindNews(e);
                NewsScroll.Children.Add(c);
            }

            if (HasImportantNews && initialRebuild)
            {
                // Switch to the news tab.
                tabControl.SelectedIndex = 0;
            }
        }
        void RebuildActionItems()
        {
            ActionScroll.Children.Clear();
            List<RoundAction> actions = HumanPlayer.PossibleActions();
            ActionItemControl c;

            // If the player has hit the top 50, provide the "Retire" option
            if (HumanPlayer.BestRank <= 50)
            {
                c = new ActionItemControl();
                c.BindAction(RetireAction);
                c.ActionClicked += ActionClicked;
                ActionScroll.Children.Add(c);
            }

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
        void RebuildTop50Items()
        {
            TopScroll.Children.Clear();

            for(int i=0;i<50;i++)
            {
                GamePlayer p = GameEngine.Players[i];

                TopPlayerControl c = new TopPlayerControl();
                c.BindPlayer(p);
                TopScroll.Children.Add(c);
            }
        }

        private void ActionClicked(RoundAction obj)
        {
            if (obj == EndRoundAction)
            {
                // Start the next round
                GameEngine.CompleteRound();
                GameEngine.BeginRound();
                UpdateUIForQuarter();

                // Did player die? If so, we're going to game over.
                if (HumanPlayer.Dead)
                {
                    LinkedWindow.GameOver(HumanPlayer);
                }

            }
            else if (obj == RetireAction)
            {
                GameEngine.CompleteRound();
                HumanPlayer.Retired = true;
                LinkedWindow.GameOver(HumanPlayer);
            }
            else
            {
                // Commit the action, and rebuild the UI to reflect the changes.
                HumanPlayer.CommitAction(obj);

                // Find result from RoundAction object.
                if (obj.ResultString != null)
                {
                    SetStatusText(obj.ResultString, obj.ResultPositive ? Brushes.DarkGreen : Brushes.DarkRed);
                }

                RebuildStatus();
                RebuildNewsItems();
                RebuildActionItems();
            }
        }

        void RebuildStatus()
        {
            // Top status area
            LabelName.Content = HumanPlayer.Name;
            LabelPlayerPosition.Content = $"(#{HumanPlayer.Values.Rank})"; // Todo: compute
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
            double TimeRemainingPercent = HumanPlayer.ThisRound.TimeRemaining / 90.0;
            BarTimeRemaining.SetPercent(TimeRemainingPercent, TimeRemainingPercent, false);

            double sentimentPercent = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.PublicSentiment);
            LabelPublicSentiment.Content = string.Format("Public Sentiment ({0})", GameFormat.FormatPercent(sentimentPercent));
            BarPublicSentiment.SetPercent(sentimentPercent, sentimentPercent, true);

            double affinityProfessional = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinityProfessional);
            LabelProfessionalAffinity.Content = string.Format("Professional Affinity ({0})", GameFormat.FormatPercent(affinityProfessional));
            BarProfessionalAffinity.SetPercent(affinityProfessional, affinityProfessional, false);

            double affinityMedia = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinityMedia);
            LabelMediaAffinity.Content = string.Format("Media Affinity ({0})", GameFormat.FormatPercent(affinityMedia));
            BarMediaAffinity.SetPercent(affinityMedia, affinityMedia, false);

            double affinitySocial = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinitySocial);
            LabelSocialAffinity.Content = string.Format("Social Affinity ({0})", GameFormat.FormatPercent(affinitySocial));
            BarSocialAffinity.SetPercent(affinitySocial, affinitySocial, false);

            double affinityCriminal = HumanPlayer.AffinityAsPercent(HumanPlayer.ThisRound.AffinityCriminal);
            LabelCriminalAffinity.Content = string.Format("Criminal Affinity ({0})", GameFormat.FormatPercent(affinityCriminal));
            BarCriminalAffinity.SetPercent(affinityCriminal, affinityCriminal, false);

        }
    }
}
