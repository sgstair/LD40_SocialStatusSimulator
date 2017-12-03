using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40_sgstair
{
    class EngineData
    {
        public static GameAction[] Actions = new GameAction[]
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Productive actions
            new GameAction {
                Title = "Work on a small project", Description = "Make something small your fans will appreciate, that won't take too much time.",
                MoneyCost = 30000, TimeCost = 45,
                BenefitString = (Player, Action) => "Improve your popularity a bit.",
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveSentiment(0, 0.01, ref Player.ThisRound.PublicSentiment);
                    Player.AddFans(0.005, 0.03, ref Player.ThisRound.FanCount);
                    Player.AddFlavorHeadline(
                        "{0} Released another micro-video today\nWeather at 11...",
                        "New release by {0}\nThere's increasing evidence that {0} does care about their fans.",
                        "The kids today are talking about {0}\nMore on how we're not irrelevant at 7");
                }
            },
            new GameAction {
                Title = "A medium size project", Description = "This will take some time, but could boost your ratings a bit.",
                MoneyCost = 110000, TimeCost = 30, NumQuarters = 3,
                BenefitString = (Player, Action) => "Improve your popularity.",
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                },
                FinalCompletion = (Player, Action, Media) => 
                {
                    Player.ImproveSentiment(0, 0.05, ref Player.ThisRound.PublicSentiment);
                    Player.AddFans(0.04, 0.13, ref Player.ThisRound.FanCount);
                    //Todo: headlines
                }
            },
            new GameAction {
                Title = "A spectacular medium project", Description = "The new professionals you've met are really good at what they do.",
                MoneyCost = 650000, TimeCost = 40, NumQuarters = 4,
                BenefitString = (Player, Action) => "Improve your popularity.",
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityProfessional(Player, 0.2),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                },
                FinalCompletion = (Player, Action, Media) =>
                {
                    Player.ImproveSentiment(0, 0.05, ref Player.ThisRound.PublicSentiment);
                    Player.AddFans(0.06, 0.18, ref Player.ThisRound.FanCount);
                    //Todo: headlines
                }
            },

            new GameAction {
                Title = "A large project", Description = "This is going to be, maybe the most ambitious thing you've ever tried.",
                MoneyCost = 780000, TimeCost = 40, NumQuarters = 6,
                BenefitString = (Player, Action) => "Improve your popularity.",
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                },
                FinalCompletion = (Player, Action, Media) =>
                {
                    Player.ImproveSentiment(0, 0.05, ref Player.ThisRound.PublicSentiment);
                    Player.AddFans(0.03, 0.36, ref Player.ThisRound.FanCount);
                    //Todo: headlines
                }
            },

            new GameAction {
                Title = "A spectacular large project", Description = "Huge ambition, and the right people collected here to pull it off. This one can't fail",
                MoneyCost = 2300000, TimeCost = 45, NumQuarters = 8,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityProfessional(Player, 0.35),
                BenefitString = (Player, Action) => "Improve your popularity a lot.",
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                },
                FinalCompletion = (Player, Action, Media) =>
                {
                    Player.ImproveSentiment(0, 0.05, ref Player.ThisRound.PublicSentiment);
                    Player.AddFans(0.29, 0.45, ref Player.ThisRound.FanCount);
                    //Todo: headlines
                }
            },

            new GameAction {
                Title = "The best project ever", Description = "This will be a blockbuster, without a doubt",
                MoneyCost = 8500000, TimeCost = 45, NumQuarters = 11,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityProfessional(Player, 0.65),
                BenefitString = (Player, Action) => "Improve your popularity a lot.",
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                },
                FinalCompletion = (Player, Action, Media) =>
                {
                    Player.ImproveSentiment(0, 0.05, ref Player.ThisRound.PublicSentiment);
                    Player.AddFans(0.39, 0.65, ref Player.ThisRound.FanCount);
                    //Todo: headlines
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Public sentiment actions
            new GameAction
            {
                Title = "Improve public sentiment", Description = "Work on some community projects that will improve people's perception of you.",
                MoneyCost = 10000, TimeCost = 70,
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveSentiment(0.03, 0.08, ref Player.ThisRound.PublicSentiment);
                    // The whole point of this is publicity, so it makes sense that news would be involved.
                    Player.AddFlavorHeadline(
                        "{0} was seen today at the animal shelter\nApparently {0} spends a lot of time there helping the animals",
                        "We bumped into {0} at a trash picking event\n{0} says the community looks best clean");
                }
            },
            new GameAction
            {
                Title = "Donate to charity", Description = "Donate a lot of money to helping the developing world",
                MoneyCost = 30000000, TimeCost = 10,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequirePublicSentiment(Player, 0.1),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveSentiment(0.06, 0.12, ref Player.ThisRound.PublicSentiment);
                    Player.AddFlavorHeadline(
                        "{0} is feeding kids in Africa\nWe just heard of a large donation made by {0} to the ...",
                        "Science education for the third world\nSurprisingly we just got news that {0} donated a large sum to help promote science education in the third world...");

                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Smear campaign actions
            new GameAction
            {
                Title = "Smear Campaign", Description = "Publish false allegations about someone doing better than you",
                MoneyCost = 135000, TimeCost = 15, RiskPercent = 0.03,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityMedia(Player, 0.2),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.ImproveAffinity(0.005, 0.001, ref Player.ThisRound.AffinityMedia);
                    Player.SmearCampaign(0.08, 0.03);
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Dig up dirt actions


            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Networking actions
            new GameAction
            {
                Title = "Professional Networking", Description = "Connect with people who might help you with your work",
                MoneyCost = 5000, TimeCost = 5,
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveAffinity(0.005, 0.02, ref Player.ThisRound.AffinityProfessional);
                    // No news from these minor events
                }
            },
            new GameAction
            {
                Title = "Professional Networking II", Description = "Host an event to get the experts in your community together",
                MoneyCost = 1000000, TimeCost = 55,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityProfessional(Player, 0.3),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveAffinity(0.012, 0.35, ref Player.ThisRound.AffinityProfessional);
                    Player.AddFlavorHeadline(
                        "Wildly successful professional conference attracts experts\n{0}, who organized the event, said the outcome was better than expected",
                        "New workshop for experts turning heads\nThis fantastic opportunity was organized by {0}, who has been incredibly active in the space...");
                }
            },
            new GameAction
            {
                Title = "Media Networking", Description = "Get to know reporters in your area",
                MoneyCost = 10000, TimeCost = 5,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityProfessional(Player, 0.08),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveAffinity(0.005, 0.02, ref Player.ThisRound.AffinityMedia);
                }
            },
            new GameAction
            {
                Title = "Media Networking II", Description = "Sponsor a media event to build relationships with the media",
                MoneyCost = 5000000, TimeCost = 60,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityMedia(Player, 0.25),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveAffinity(0.13, 0.36, ref Player.ThisRound.AffinityMedia);
                    //Todo: add headlines - this is really a polish work item.
                }
            },
            new GameAction
            {
                Title = "Social Networking", Description = "Find the places where all the popular people hang out",
                MoneyCost = 24000, TimeCost = 5,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityProfessional(Player, 0.11),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveAffinity(0.005, 0.02, ref Player.ThisRound.AffinitySocial);
                }
            },
            new GameAction
            {
                Title = "Social Networking II", Description = "Throw an elaborate party for the rich and famous",
                MoneyCost = 10000000, TimeCost = 35,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinitySocial(Player, 0.35),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveAffinity(0.012, 0.035, ref Player.ThisRound.AffinitySocial);
                }
            },
            new GameAction
            {
                Title = "Criminal Networking", Description = "You've seen the seedy underbelly of this place, explore deeper",
                MoneyCost = 30000, TimeCost = 5, RiskPercent = 0.02,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityProfessional(Player, 0.15) &&
                    GameAction.RequireAffinityMedia(Player, 0.15) && GameAction.RequireAffinitySocial(Player, 0.20),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    // Tell game engine about what kind of criminal activity might be taking place
                    Player.SetCriminalHeadlines(
                        "Police raid of brothel nets {0}\nPolice were surprised to see {0} while executing their sting operation on...",
                        "{0} Nabbed in illegal firearm bust\n{0} claimed to just be \"holding it for a friend\"...");

                    if(GameAction.DidEvadeCriminalRisk(Player, Action))
                    {
                        Player.ImproveAffinity(0.005, 0.02, ref Player.ThisRound.AffinityCriminal);
                    }
                }
            },
            new GameAction
            {
                Title = "Criminal Networking II", Description = "Bribe some local crime bosses to increase your clout",
                MoneyCost = 5000000, TimeCost = 10, RiskPercent = 0.08,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityCriminal(Player, 0.25),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.SetCriminalHeadlines(
                        "Police raid of brothel nets {0}\nPolice were surprised to see {0} while executing their sting operation on...",
                        "{0} Nabbed in illegal firearm bust\n{0} claimed to just be \"holding it for a friend\"..."
                        );
                    if(GameAction.DidEvadeCriminalRisk(Player, Action))
                    {
                        Player.ImproveAffinity(0.12, 0.25, ref Player.ThisRound.AffinityCriminal);
                    }
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Newsworthy actions

            new GameAction
            {
                Title = "Engage the Media", Description = "Let's go do something crazy and be on the news! Could be good or bad.",
                MoneyCost = 20000, TimeCost = 25,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && GameAction.RequireAffinityMedia(Player, 0.20),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ImproveAffinity(0.005, 0.02, ref Player.ThisRound.AffinityProfessional);
                    // No news from these minor events
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Debt reduction actions
            new GameAction
            {
                Title = "File for Bankruptcy", Description = "Lose the crippling debt, but also most of your connections and fans",
                MoneyCost = 0, TimeCost = 80,
                CanUseAction = (Player, Action, Media) => Player.ThisRound.Money < 0,
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ThisRound.Money = 100000;
                    if(Player.ThisRound.PublicSentiment > 0)
                    {
                        Player.ThisRound.PublicSentiment /= 5;
                    }
                    Player.ThisRound.FanCount /= 2;
                    Player.ThisRound.AffinityCriminal /= 5;
                    Player.ThisRound.AffinitySocial /= 5;
                    Player.ThisRound.AffinityMedia /= 5;
                    Player.ThisRound.AffinityProfessional /= 5;
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Criminal actions

            new GameAction
            {
                Title = "Deal Drugs to the Elite", Description = "So many of these stars love their drugs, maybe we can get some money to work with",
                MoneyCost = 200000, TimeCost = 35, RiskPercent = 0.08,
                BenefitString = (Player, Action) => "$5M if not caught",
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) && 
                    GameAction.RequireAffinityCriminal(Player, 0.13) && GameAction.RequireAffinitySocial(Player, 0.20),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.SetCriminalHeadlines(
                        "Undercover sting targets celebrities\n{0} was among the names released by the police department related to their recent raid..."
                        );
                    if(GameAction.DidEvadeCriminalRisk(Player, Action))
                    {
                        Player.ThisRound.Money += 5000000;
                        Player.ImproveAffinity(0.005, 0.05, ref Player.ThisRound.AffinityCriminal);
                        Player.ImproveAffinity(0.005, 0.03, ref Player.ThisRound.AffinitySocial);
                    }
                }
            },

            new GameAction
            {
                Title = "Assasinate a Rival", Description = "So many problems, if only they would just... Disappear...",
                MoneyCost = 8000000, TimeCost = 25, RiskPercent = 0.35,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    GameAction.RequireAffinityCriminal(Player, 0.40) && GameAction.RequireAffinitySocial(Player, 0.20),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.AttemptAssasination(Action.RiskPercent);
                }
            },

        };

        public static GameAction[] MediaActions = new GameAction[]
        {

        };

    }


    class GameFormat
    {
        public static string FormatMoney(long moneyValue)
        {
            long printValue = moneyValue;
            string suffix = "";

            if (moneyValue > 15000000000L)
            {
                printValue = (long)Math.Round(moneyValue / 1000000000.0);
                suffix = "B";
            }
            else if (moneyValue > 15000000)
            {
                printValue = (long)Math.Round(moneyValue / 1000000.0);
                suffix = "M";
            }
            else if (moneyValue > 15000)
            {
                printValue = (long)Math.Round(moneyValue / 1000.0);
                suffix = "k";
            }
            return string.Format("${0:N0}{1}", printValue, suffix);
        }
        public static string FormatTime(int days)
        {
            return string.Format("{0} day{1}", days, days == 1 ? "" : "s");
        }
        public static string FormatPercent(double percent)
        {
            if(Math.Abs(percent)> 0.08)
            {
                return string.Format("{0}%", Math.Round(percent * 100));
            }
            else
            {
                int percentpart = (int)Math.Round(percent * 100 * 100);
                if (percentpart == 0) return "0%";
                return string.Format("{0:n2}%", percentpart / 100.0);
            }
        }

        public static string FormatFans(long fans)
        {
            if (fans == 1) return "1 fan";
            string suffix = "";
            // There are only 10 billion fans in the simulated world.
            if(fans > 15000000)
            {
                suffix = " million";
                fans = (long)Math.Round(fans / 1000000.0);
            }
            else if(fans > 15000)
            {
                suffix = " thousand";
                fans = (long)Math.Round(fans / 1000.0);
            }
            return string.Format("{0:N0}{1} fans", fans, suffix);
        }
    }

    class GameAction
    {

        public static bool CanUseBase(GamePlayer Player, GameAction Action, MediaEvent Media)
        {
            return Player.ThisRound.Money >= Action.MoneyCost && Player.ThisRound.TimeRemaining >= Action.TimeCost;
        }
        public static bool RequirePublicSentiment(GamePlayer Player, double Requirement)
        {
            return Player.AffinityAsPercent(Player.ThisRound.PublicSentiment) >= Requirement;
        }
        public static bool RequireAffinityProfessional(GamePlayer Player, double Requirement)
        {
            return Player.AffinityAsPercent(Player.ThisRound.AffinityProfessional) >= Requirement;
        }
        public static bool RequireAffinityMedia(GamePlayer Player, double Requirement)
        {
            return Player.AffinityAsPercent(Player.ThisRound.AffinityMedia) >= Requirement;
        }
        public static bool RequireAffinitySocial(GamePlayer Player, double Requirement)
        {
            return Player.AffinityAsPercent(Player.ThisRound.AffinitySocial) >= Requirement;
        }
        public static bool RequireAffinityCriminal(GamePlayer Player, double Requirement)
        {
            return Player.AffinityAsPercent(Player.ThisRound.AffinityCriminal) >= Requirement;
        }
        public static bool DidEvadeCriminalRisk(GamePlayer Player, GameAction Action)
        {
            return Player.DidEvadeCriminalRisk(Action.RiskPercent);
        }

        public static void DeductCost(GamePlayer Player, GameAction Action, MediaEvent Media)
        {
            Player.ThisRound.Money -= Action.MoneyCost;
            Player.ThisRound.TimeRemaining -= Action.TimeCost;
        }
        public static string DefaultCostString(GamePlayer Player, GameAction Action)
        {
            List<string> stringParts = new List<string>();
            if (Action.MoneyCost > 0) { stringParts.Add(GameFormat.FormatMoney(Action.MoneyCost)); }
            if (Action.TimeCost != 0)
            {
                stringParts.Add(GameFormat.FormatTime(Action.TimeCost) + (Action.NumQuarters > 1 ? $" for {Action.NumQuarters} Quarters" : ""));
            }
            if (stringParts.Count == 0) return null;
            return string.Join(", ", stringParts);
        }
        public static string DefaultRiskString(GamePlayer Player, GameAction Action)
        {
            if (Action.RiskPercent == 0) return null;
            return GameFormat.FormatPercent(Action.RiskPercent);
        }


        public string Title;
        public string Description;

        public long MoneyCost;
        public int TimeCost;
        public int NumQuarters = 1; // Some actions will require time for multiple consecutive quarters to complete.
        public double RiskPercent;

        public PartialProject CancelTaskFor; // Link back to the partial project this is canceling (if it is canceling a partial project)

        public Func<GamePlayer, GameAction, string> CostString = DefaultCostString;
        public Func<GamePlayer, GameAction, string> RiskString = DefaultRiskString;
        public Func<GamePlayer, GameAction, string> BenefitString;

        // Determine if the action can be used
        public Func<GamePlayer, GameAction, MediaEvent, bool> CanUseAction = CanUseBase;
        public Action<GamePlayer, GameAction, MediaEvent> CommitAction = DeductCost;
        public Action<GamePlayer, GameAction, MediaEvent> FinalCompletion; // Final completion of actions that take multiple quarters of time. Only used when NumQuarters > 1
    }
}
