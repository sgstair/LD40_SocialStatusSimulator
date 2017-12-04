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
                    Player.AddFlavorHeadline(
                        "Another releas by {0} hits the shelves\nThis project has been in progress for quite some time, we can't wait to see how it holds up"
                       );
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
                    Player.AddFlavorHeadline(
                        "Another release by {0} hits the shelves\nWe're hearing good things about this one"
                       );
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
                    Player.AddFlavorHeadline(
                        "{0} has been working on this for over a year\nExcited fans have flooded local retailers to see {0}'s materpiece"
                       );
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
                    Player.AddFlavorHeadline(
                        "The new blockbuster of our era\nRave reviews for {0}'s latest release"
                       );

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
                    Player.AddFlavorHeadline(
                        "Crowds seeking {0}'s release continue\nWeeks after the release, fans are still trying to get their hands on a copy"
                       );
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
                    // Smear campaign has its own media interactions
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
                    Player.ImproveAffinity(0.12, 0.35, ref Player.ThisRound.AffinityProfessional);
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
                    Player.AddFlavorHeadline(
                        "Media industry event attracts pros from near and far\nParticipants from the other side of the continent declare it a smashing success"

                        );
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
                    Player.ImproveAffinity(0.12, 0.35, ref Player.ThisRound.AffinitySocial);
                    Player.AddFlavorHeadline(
                        "House party organized by {0} attracts crowd\nPatrons have nothing but good things to say"
                        );
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
                        Player.SetResult("Success: Criminal standing improved", true);
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
                        Player.SetResult("Success: The boss accepts your generous gift", true);
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
                    Player.ImproveAffinity(0.005, 0.02, ref Player.ThisRound.AffinityMedia);

                    Player.AddVanityHeadline(
                        "World Record attempt by {0}\n{0} failed miserably today to set the world record for number of consecutive backflips",
                        "{0} takes us skydiving\nThe record for this year's most ridiculous publicity stunt goes to {0}",
                        "{0} gives lecture on popsicles\nOnly 7 people showed up for {0}'s hour long lecture on frozen treats"
                        );
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Debt reduction actions
            new GameAction
            {
                Title = "File for Bankruptcy", Description = "Lose the crippling debt, but also most of your connections and fans",
                MoneyCost = 0, TimeCost = 80,
                CanUseAction = (Player, Action, Media) => Player.ThisRound.Money < 0 && Player.ThisRound.TimeRemaining >= Action.TimeCost,
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

                    Player.AddFlavorHeadline(
                        "{0} unexpectedly filed for bankruptcy protection today\nThe lavish lifestyle was hiding a now apparent lack of liquidity"
                        );
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
                        Player.SetResult("Success: This party sure is lively", true);
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
                    // This path has its own news generation
                }
            },


#if false
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Some test actions for making sure the game works correctly.

            new GameAction {
                Title = "Go Directly to Jail", Description = "Do not pass go, Do not collect $200",
                MoneyCost = 0, TimeCost = 0,
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.GoToJail(4);
                }
            },


            new GameAction {
                Title = "Fine, Fine", Description = "I'll give you a fine.",
                MoneyCost = 0, TimeCost = 0,
                CostString = (Player, Action) => "$100M Fine, whether you have it or not",
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ApplyFine(100000000,"Bewildered police officer gives record fine\n{0} asked to be fined, we still don't unserstand why. Their publicist has not returned our calls.");
                }
            },

            new GameAction {
                Title = "Gambling addiction", Description = "Lose a bunch of money, for no good reason.",
                MoneyCost = 0, TimeCost = 0,
                CostString = (Player, Action) => "$100M, whether you have it or not",
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.ThisRound.Money -= 100000000;
                }
            },

            new GameAction {
                Title = "Status test", Description = "Post a status",
                MoneyCost = 0, TimeCost = 0,
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                    Player.SetResult("Here is a result!", true);
                }
            },

#endif


        };

        public static GameAction[] MediaActions = new GameAction[]
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Respond to media contact
            new GameAction
            {
                Title = "Respond in a Dignified fashion", Description = "Do some research and consultation to make a good impression",
                MoneyCost = 50000, TimeCost = 25, 
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    GameAction.RequireAffinityMedia(Player, 0.20) && GameAction.RequireAffinitySocial(Player, 0.20) &&
                    (Media.EventType == MediaEventType.Smear || Media.EventType == MediaEventType.Vanity),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.MediaAffectPopularity(0.15, 0.15);
                }
            },

            new GameAction
            {
                Title = "Engage with the media, and be yourself", Description = "Is casual you a good fit for your audience? At least there isn't much preparation",
                MoneyCost = 5000, TimeCost = 3,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    (Media.EventType == MediaEventType.Smear || Media.EventType == MediaEventType.Vanity),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.MediaAffectPopularity(0.1, 0.15);
                }
            },

            new GameAction
            {
                Title = "React controversially to the media", Description = "Nobody can guess how this is going to end...",
                MoneyCost = 80000, TimeCost = 15,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    GameAction.RequireAffinityMedia(Player, 0.10) && GameAction.RequireAffinitySocial(Player, 0.10) &&
                    (Media.EventType == MediaEventType.Smear || Media.EventType == MediaEventType.Vanity),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.MediaAffectPopularity(0.03, 0.45);
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Respond to criminal reporting

            new GameAction
            {
                Title = "Respond in a Dignified fashion", Description = "Explain your case and try to get people on your side",
                MoneyCost = 50000, TimeCost = 25,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    GameAction.RequireAffinityMedia(Player, 0.20) && GameAction.RequireAffinitySocial(Player, 0.20) &&
                    (Media.EventType == MediaEventType.Criminal),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.MediaAffectPopularity(0.05, 0.10);
                }
            },

            new GameAction
            {
                Title = "React controversially to the media", Description = "Yeah man! Those automatic weapons should totally be legal!",
                MoneyCost = 80000, TimeCost = 15,
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    GameAction.RequireAffinityMedia(Player, 0.10) && GameAction.RequireAffinitySocial(Player, 0.10) &&
                    (Media.EventType == MediaEventType.Criminal),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);

                    Player.MediaAffectPopularity(-.1, 0.5);
                }
            },

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Respond to failed assassination attempts


            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Option to sue the origin of assasination / smear campaign plots


            new GameAction
            {
                Title = "Sue the originator", Description = "We have a rock solid case to extract some added benefits from this plot.",
                MoneyCost = 200000, TimeCost = 15, NumQuarters = 3,
                BenefitString = (Player, Action) => "$50M on completion of the lawsuit",
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    GameAction.RequireAffinitySocial(Player, 0.10) &&
                    (Media.EventType == MediaEventType.Victim && Media.InstigatingPlayer != null),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                },
                FinalCompletion = (Player, Action, Media) =>
                {
                    long settlementamount = 50000000;
                    Media.InstigatingPlayer.ThisRound.Money -= settlementamount;
                    Player.ThisRound.Money += settlementamount;
                    
                    // Add headlines to both parties to indicate what's going on.
                    string mediaText = $"{Player.Name}'s lawsuit against {Media.InstigatingPlayer.Name} concludes\nDefendant to pay $50M in damages for the attempt on {Player.Name}'s life";
                    Player.QueueNews(mediaText);
                    Media.InstigatingPlayer.QueueNews(mediaText, highlight:true);
                }
            },
            new GameAction
            {
                Title = "Sue the originator", Description = "We have a rock solid case to extract some added benefits from this plot.",
                MoneyCost = 100000, TimeCost = 10, NumQuarters = 2,
                BenefitString = (Player, Action) => "$5M on completion of the lawsuit",
                CanUseAction = (Player, Action, Media) => GameAction.CanUseBase(Player, Action, Media) &&
                    GameAction.RequireAffinitySocial(Player, 0.10) &&
                    (Media.EventType == MediaEventType.Smear && Media.InstigatingPlayer != null),
                CommitAction = (Player, Action, Media) =>
                {
                    GameAction.DeductCost(Player, Action, Media);
                },
                FinalCompletion = (Player, Action, Media) =>
                {
                    long settlementamount = 5000000;
                    Media.InstigatingPlayer.ThisRound.Money -= settlementamount;
                    Player.ThisRound.Money += settlementamount;

                    // Add headlines to both parties to indicate what's going on.
                    string mediaText = $"{Player.Name}'s lawsuit against {Media.InstigatingPlayer.Name} concludes\nDefendant settled to pay $5M for defamantion";
                    Player.QueueNews(mediaText);
                    Media.InstigatingPlayer.QueueNews(mediaText, highlight:true);
                }
            },
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
