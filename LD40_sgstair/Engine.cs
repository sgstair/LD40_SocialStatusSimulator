using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40_sgstair
{
    class Engine
    {
        Random r;
        int RandomSeed;

        public Engine(int seed)
        {
            r = new Random(seed);
            RandomSeed = seed;
        }
        public Engine() : this((new Random()).Next())
        {
        }

        public static readonly long WorldPopulation = 10000000000; // 10 billion

        public List<GamePlayer> Players = new List<GamePlayer>();
        public Dictionary<GamePlayer, List<MediaEvent>> RoundNews = new Dictionary<GamePlayer, List<MediaEvent>>();
        public List<MediaEvent> ThisRoundNews = new List<MediaEvent>();
        public List<MediaEvent> NextRoundNews = new List<MediaEvent>();



        public int Quarter = 1;
        public int Year = 0;
        public int YearBase = 2017;
        public int CurrentYear {  get { return YearBase + Year; } }


        public GamePlayer PrepareSinglePlayerGame(string name = null)
        {
            if (name == null) name = GenerateName();
            GamePlayer human = new GamePlayer(this, name, true);
            Players.Add(human);

            // Initial values for human.
            human.Values.Money = 10000000;
            human.Values.FanCount = 100000; // 100k fans
            human.Values.PublicSentiment = 0; // Neutral public sentiment
            human.Values.AffinityProfessional = 0; // Neutral affinities
            human.Values.AffinityMedia = 0; 
            human.Values.AffinitySocial = 0; 
            human.Values.AffinityCriminal = 0; 

            for (int i=0;i<500;i++)
            {
                GamePlayer cpu = new GamePlayer(this, GenerateName(), false);
                Players.Add(cpu);

                cpu.BaseAge = 20 + r.NextDouble() * 40;
                cpu.LifeExpectancy = 50 + r.NextDouble() * 40;

                cpu.Values.Money = (long)(100000 * i * (r.NextDouble() + 0.8));
                cpu.Values.FanCount = (int)(20000 * i * (r.NextDouble() + 0.8));
                cpu.Values.PublicSentiment = r.Next(-0x5000, 0x5000);
                cpu.Values.AffinityProfessional = r.Next(0, 0x5000);
                cpu.Values.AffinityMedia = r.Next(0, 0x5000);
                cpu.Values.AffinitySocial = r.Next(0, 0x5000);
                cpu.Values.AffinityCriminal = r.Next(0, 0x5000);
            }

            // Queue some intro news for the human.
            MediaEvent me = new MediaEvent(human, "New star strikes out into the spotlight!", $"After {name}'s smash success last week, everyone is talking about it...", null, false);
            NextRoundNews.Add(me);

            return human;
        }

        HashSet<string> UsedNames = new HashSet<string>();

        string GenerateName()
        {
            // There should be 200*1000 names in the generator, we will not exhaust that many.
            string use = null;
            for (int i=0;i<1000;i++)
            {
                use = NameGenerator.GenerateName(r.Next(), r.Next());
                if(!UsedNames.Contains(use))
                {
                    UsedNames.Add(use);
                    return use;
                }
            }
            // Give up. May produce duplicates if an extreme number of users are generated.
            return use;
        }


        void AddCpuPlayer()
        {
            GamePlayer cpu = new GamePlayer(this, GenerateName(), false);
            Players.Add(cpu);

            long maxFans = Players[49].Values.FanCount;
            long maxMoney = 20000000;

            cpu.BaseAge = 20 + r.NextDouble() * 40 - (Year + Quarter*0.25); // Players starting late need to be adjusted back to the starting time.
            cpu.LifeExpectancy = 50 + r.NextDouble() * 40;

            cpu.Values.Money = (long)Math.Round(maxMoney * r.NextDouble());
            cpu.Values.FanCount = (long)Math.Round(maxFans * r.NextDouble());
            cpu.Values.PublicSentiment = r.Next(-0x5000, 0x5000);
            cpu.Values.AffinityProfessional = r.Next(0, 0x5000);
            cpu.Values.AffinityMedia = r.Next(0, 0x5000);
            cpu.Values.AffinitySocial = r.Next(0, 0x5000);
            cpu.Values.AffinityCriminal = r.Next(0, 0x5000);
        }


        public void BeginGame()
        {

        }
        public void BeginRound()
        {
            // Compute rank for all the players
            Players.Sort((a, b) => Math.Sign(b.Values.FanCount - a.Values.FanCount));
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].Values.Rank = i + 1;
                if(Players[i].ThisRound != null)  Players[i].ThisRound.Rank = i + 1; // Hax
            }
            // remove players who are dead and rank below the top 50
            Players.RemoveAll((p) => p.Dead && p.Values.Rank > 50 && p.Human == false);

            // Add players to get back to the 501 mark
            while (Players.Count < 501)
                AddCpuPlayer();

            // Prepare players for this round
            foreach (GamePlayer p in Players)
            {
                p.BeginRound();                
            }

            // Advance buffered media fallout
            // Build a per-user list of the news to assist with cpu cost of browsing it.
            RoundNews = new Dictionary<GamePlayer, List<MediaEvent>>();
            foreach(MediaEvent e in NextRoundNews)
            {
                AddNews(e.AffectedPlayer, e);
                AddNews(e.InstigatingPlayer, e);
            }
            ThisRoundNews = NextRoundNews;
            NextRoundNews = new List<MediaEvent>();

        }

        void AddNews(GamePlayer p, MediaEvent e)
        {
            if (p == null) return;
            List<MediaEvent> playerNews;
            if(!RoundNews.TryGetValue(p, out playerNews))
            {
                playerNews = new List<MediaEvent>();
                RoundNews.Add(p, playerNews);
            }
            playerNews.Add(e);
        }


        public void CompleteRound()
        {
            // Play all the AI players
            foreach (GamePlayer p in Players)
            {
                if(!p.Human)
                    p.PlayAI();
            }

            // Commit changes from this round
            foreach (GamePlayer p in Players)
            {
                p.CompleteRound();
            }

            // Advance to next quarter
            Quarter++;
            if(Quarter > 4)
            {
                Quarter = 1;
                Year++;
            }

            // Check for player death.
            foreach (GamePlayer p in Players)
            {
                if(p.Age > p.LifeExpectancy)
                {
                    p.Kill("{0} died of natural causes", "{0} Lived to the old age of {1}");
                }
            }
        }


        // Randomnews helpers
        public double NextPercent(double minPercent, double maxPercent)
        {
            if (minPercent >= maxPercent) return minPercent;
            return minPercent + (maxPercent - minPercent) * r.NextDouble();
        }

        public bool PercentChance(double percent)
        {
            return percent <= r.NextDouble();
        }

        public int RandomNext(int min, int max)
        {
            return r.Next(min, max);
        }
    }

    class PlayerValues : ICloneable
    {
        public object Clone()
        {
            PlayerValues newValues = (PlayerValues)this.MemberwiseClone();
            return newValues;
        }

        public int Rank;

        public long Money;
        public int TimeRemaining;
        public long FanCount;
        // For sentiment, affinities Working range is -0x10000 to +0x10000. Avoiding floating point for no particular reason.
        public int PublicSentiment; 
        public int AffinityProfessional;
        public int AffinityMedia;
        public int AffinitySocial;
        public int AffinityCriminal;
    }

    class PartialProject
    {
        public GameAction Project;
        public MediaEvent Media;
        public int QuartersRemaining;
        public bool JailTime;
    }

    class GamePlayer
    {
        public Engine Parent;

        public string Name;
        public bool Human;

        public double Age {  get { return Parent.Year + BaseAge + (Parent.Quarter-1) * 0.25; } }
        public double BaseAge = 25;
        public double LifeExpectancy = 70;
        public int FinalAge = 0;
        public bool Dead = false;
        public bool InJail = false;
        public bool Retired = false;

        public string DeadHowDied = "not dead";
        public string DeadContext = "really not dead";

        public long BestMoney = 0;
        public long BestFans = 0;
        public int BestRank = 5000;
        public int FineCount;
        public long FineTotal;
        public int JailCount;
        public int JailQuarters;
        public int AvoidedCriminalDetectionCount = 0;
        public int SmearCampaigns = 0;
        public int FailedSmearCampaigns = 0;

        public int WorkedDays = 0;
        public int LazyDays = 0;
        public int TotalDays {  get { return WorkedDays + LazyDays; } }

        public PlayerValues Values = new PlayerValues(); // Master value list for player
        public PlayerValues ThisRound; // Accumulated values as player takes actions, but not fully applied yet.
        public PlayerValues LastRound;

        public List<PartialProject> PartialProjects = new List<PartialProject>();
        public List<MediaEvent> OldNews = new List<MediaEvent>();

        public GamePlayer(Engine parentEngine, string playerName, bool playerHuman = false)
        {
            Parent = parentEngine;
            Name = playerName;
            Human = playerHuman;
        }


        public override string ToString()
        {
            return $"GamePlayer('{Name}',{Math.Floor(Age)}/{Math.Floor(LifeExpectancy)}, #{Values.Rank}, {GameFormat.FormatMoney(Values.Money)}, {GameFormat.FormatFans(Values.FanCount)}";
        }


        public void BeginRound()
        {
            if (LastRound == null)
            {
                LastRound = (PlayerValues)Values.Clone(); // First round only, as these aren't prepared
                ThisRound = (PlayerValues)Values.Clone();
            }
            ThisRound.TimeRemaining = 90; // 90 days per quarter, roughly
            

            // Public perception wanders a bit
            ImproveSentiment(-0.01, 0.01, ref ThisRound.PublicSentiment);

            // Add fans based on public perception and existing fanbase
            double sentimentPercent = 0.05 * AffinityAsPercent(ThisRound.PublicSentiment);
            AddFans(sentimentPercent, sentimentPercent + 0.04, ref ThisRound.FanCount);

            // Make some money based on the size of the fanbase.
            double moneyPerFan = Parent.NextPercent(0.08, 0.28);
            long addMoney = (long)Math.Round(ThisRound.FanCount * moneyPerFan);
            ThisRound.Money += addMoney;

            // Add some decay to the number of fans
            double decayRate = 0.01;
            if (Dead) decayRate = 0.04;
            long lostFans = (long)Math.Round(ThisRound.FanCount * decayRate);
            ThisRound.FanCount -= lostFans;

            // If dead, decay sentiment
            if(Dead)
            {
                ThisRound.PublicSentiment = (int)Math.Round(ThisRound.PublicSentiment * 0.95);
            }

            // Deduct time for partial projects that are in progress.
            foreach(PartialProject p in PartialProjects)
            {
                if (p.Project != null)
                {
                    int days = p.Project.TimeCost;
                    ThisRound.TimeRemaining -= days;
                }
            }
        }

        public void CompleteRound()
        {
            // Update tracking values
            LastRound = (PlayerValues)Values.Clone(); // LastRound = Values at the start of the previous round
            Values = (PlayerValues)ThisRound.Clone(); // Values = Values at the start of the current round. (only true during the round)

            // Track peak money, fans
            BestMoney = Math.Max(Values.Money, BestMoney);
            BestFans = Math.Max(Values.FanCount, BestFans);
            BestRank = Math.Min(Values.Rank, BestRank);

            WorkedDays += 90 - Values.TimeRemaining;
            LazyDays += Values.TimeRemaining;

            // Future: Update life expectancy based on stress.


            // Prepare for next round - Do this here, so we capture the partial project completion in the changes for next quarter.
            ThisRound = (PlayerValues)Values.Clone();

            // Complete partial projects that are done.
            foreach (PartialProject p in PartialProjects)
            {
                if(p.JailTime)
                {
                    JailQuarters++;
                }

                p.QuartersRemaining--;
                if(p.QuartersRemaining == 0)
                {
                    // Complete the project
                    if (p.JailTime)
                    {
                        // Get out of jail!
                        InJail = false;
                    }
                    else
                    {
                        p.Project.FinalCompletion(this, p.Project, p.Media);
                    }
                }
            }
            PartialProjects.RemoveAll((p) => p.QuartersRemaining == 0);

            // Keep track of old news for human players
            if(Human)
            {
                OldNews.InsertRange(0,GetPlayerNews());
                if(OldNews.Count > 50)
                {
                    OldNews.RemoveRange(50, OldNews.Count - 50);
                }
            }
        }


        public List<RoundAction> PossibleActions()
        {
            List<RoundAction> actions = new List<RoundAction>();
            if (Dead) return actions;

            // Add cancel actions for any partial projects. Number these options as -1, -2, -3... for tracking.
            int index = 1;
            foreach(PartialProject p in PartialProjects)
            {
                if (p.JailTime)
                {
                    string action_s = p.QuartersRemaining == 1 ? "" : "s";
                    GameAction cancelAction = new GameAction()
                    {
                        Title = $"In Jail",
                        Description = $"You're stuck here for another {p.QuartersRemaining} Quarter{action_s}",
                        CostString = (Player, Action) => null,
                        CommitAction = (Player, Action, Media) => { }
                    };
                    actions.Add(new RoundAction(this, cancelAction, -index));
                    return actions; // There can be no other actions in jail.
                }
                else
                {
                    string action_s = p.QuartersRemaining == 1 ? "" : "s";
                    string remaining = $"{p.QuartersRemaining} Quarter{action_s} remaining. ";
                    if (p.QuartersRemaining == 1)
                    {
                        remaining = "Completes this quarter. ";
                    }
                    GameAction cancelAction = new GameAction()
                    {
                        Title = $"Cancel '{p.Project.Title}'",
                        Description = remaining + "Recover your time but lose all investment.",
                        TimeCost = -p.Project.TimeCost,
                        CancelTaskFor = p,
                        CostString = (Player, Action) => null,
                        BenefitString = (Player, Action) => $"Recover {GameFormat.FormatTime(p.Project.TimeCost)}",
                        CommitAction = (Player, Action, Media) =>
                        {
                            GameAction.DeductCost(Player, Action, Media); // Adds back the time because of TimeCost field manipulation above
                                                                          // Also remove this project from the partial project list
                        Player.PartialProjects.Remove(Action.CancelTaskFor);
                        }
                    };
                    actions.Add(new RoundAction(this, cancelAction, -index));
                }
                index++;
            }

            for (int i=0;i<EngineData.Actions.Length;i++)
            {
                GameAction a = EngineData.Actions[i];
                if(a.CanUseAction(this,a, null))
                {
                    actions.Add(new RoundAction(this, a, i));
                }
            }
            return actions;
        }

        RoundAction CurrentRoundAction;

        public void SetResult(string result, bool positive)
        {
            CurrentRoundAction.ResultString = result;
            CurrentRoundAction.ResultPositive = positive;
        }

        public void CommitAction(RoundAction a)
        {
            if (InJail || Dead) return;

            CurrentRoundAction = a;
            a.Action.CommitAction(this, a.Action, a.Media);
            if(a.Media != null)
            {
                a.Media.Reacted = true;
            }

            if (a.Action.NumQuarters > 1)
            {
                PartialProject p = new PartialProject() { Project = a.Action, QuartersRemaining = a.Action.NumQuarters, Media = a.Media };
                PartialProjects.Add(p);
            }
        }

        public List<RoundAction> MediaActions(MediaEvent e, int mediaIndex)
        {
            List<RoundAction> actions = new List<RoundAction>();
            if (Dead || InJail) return actions;
            if (!e.Important || e.Reacted) return actions;

            // Find what media actions can be applied to this event.
            for (int i = 0; i < EngineData.MediaActions.Length; i++)
            {
                GameAction a = EngineData.MediaActions[i];
                if (a.CanUseAction(this, a, e))
                {
                    actions.Add(new RoundAction(this, a, i, mediaIndex) { Media = e });
                }
            }

            return actions;
        }

        /// <summary>
        /// Kill("{0}died of natural causes", "{0} Lived to the old age of {1}");
        /// </summary>
        public void Kill(string howDied, string context="")
        {
            FinalAge = (int)Math.Floor(Age);
            Dead = true;
            DeadHowDied = string.Format(howDied, Name, FinalAge);
            DeadContext = string.Format(context, Name, FinalAge);
        }



        // AI

        public void PlayAI()
        {
            if(Dead || InJail) return;

            while(true)
            {
                if (Parent.PercentChance(0.05)) return; // 1 in 20 chance to just stop here.

                // Enumerate actions
                List<RoundAction> actions = PossibleActions();

                // Also get actions from media items
                List<MediaEvent> media = GetPlayerNews();
                int index = 0;
                foreach(MediaEvent e in media)
                {
                    if(e.Important)
                    {
                        List<RoundAction> mediaActions = MediaActions(e, index);
                        actions.AddRange(mediaActions);
                    }
                    index++;
                }
                if (actions.Count == 0) return;

                // Pick one
                index = Parent.RandomNext(0, actions.Count);
                CommitAction(actions[index]);
            }
        }

        // News
        public void QueueNews(string blurb, string subBlurb, string outcome, bool highlight = false, MediaEventType eventType = MediaEventType.Mundane)
        {
            MediaEvent me = new MediaEvent(this, blurb, subBlurb, outcome, highlight);
            me.EventType = eventType;
            Parent.NextRoundNews.Add(me);
        }

        public void QueueNews(string newsConcentrate, string outcome = null, bool highlight = false, MediaEventType eventType = MediaEventType.Mundane)
        {
            string formattedReason = string.Format(newsConcentrate, this.Name);
            string[] parts = formattedReason.Split('\n');
            if (parts.Length < 2) parts = new string[] { formattedReason, "Ha, the editor is on vacation and I can say whatever I want" };
            QueueNews(parts[0], parts[1], outcome, highlight, eventType);
        }

        /// <summary>
        /// Meaningless news - just exists to make the headlines a little more interesting.
        /// </summary>
        public void AddFlavorHeadline(params string[] Reasons)
        {
            string Reason = Reasons[Parent.RandomNext(0, Reasons.Length)];
            QueueNews(Reason);
        }

        public void AddVanityHeadline(params string[] Reasons)
        {
            string Reason = Reasons[Parent.RandomNext(0, Reasons.Length)];
            QueueNews(Reason, null, true, MediaEventType.Vanity);
        }

        public List<MediaEvent> GetPlayerNews()
        {
            List<MediaEvent> importantNews = new List<MediaEvent>();
            List<MediaEvent> otherNews = new List<MediaEvent>();
            List<MediaEvent> myNews;
            if (Parent.RoundNews.TryGetValue(this, out myNews))
            {
                foreach (MediaEvent me in myNews)
                {
                    if (me.AffectedPlayer == this || me.InstigatingPlayer == this)
                    {
                        if (me.Important) { importantNews.Add(me); } else { otherNews.Add(me); }
                    }
                }
                importantNews.AddRange(otherNews);
            }
            return importantNews;
        }


        // Crminal handling
        public void ApplyFine(long fineAmount, string fineReason)
        {
            string outcome = string.Format("You were fined {0}", GameFormat.FormatMoney(fineAmount));
            ThisRound.Money -= fineAmount;

            FineCount++;
            FineTotal += fineAmount;

            SetResult("Failed: " + outcome, false);
            QueueNews(fineReason, outcome, true, MediaEventType.Criminal);
        }

        public void GoToJail(int Quarters)
        {
            // Cancel all ongoing projects
            PartialProjects.Clear();

            // Penalty to affinities and fans (Criminal affinity not affected.)
            ThisRound.AffinityProfessional /= 3;
            ThisRound.AffinitySocial /= 3;
            ThisRound.AffinityMedia /= 3;

            if (ThisRound.PublicSentiment > 0)
                ThisRound.PublicSentiment /= 3;

            ThisRound.FanCount /= 2;

            // Create jail partial project
            PartialProject p = new PartialProject();
            p.QuartersRemaining = Quarters;
            p.JailTime = true;

            InJail = true;
            JailCount++;

            SetResult("Failed: The police threw you in jail.", false);

            PartialProjects.Add(p);

        }
        public bool DidEvadeCriminalRisk(double percentRisk)
        {
            double outcome = Parent.NextPercent(0, 1);
            if(outcome <= percentRisk)
            {
                // Risk bit you. Now we decide how to punish the player, depending on the risk and outcome %
                // What was the player even doing? Decide.

                string Reason = CriminalHeadlines[Parent.RandomNext(0, CriminalHeadlines.Length)];

                if( percentRisk <= 0.05 )
                {
                    // Outcome: Fined. Fines in this range are pretty negligible, and generally < $10k
                    int baseFine = (int)Math.Round(percentRisk * 100 * 1000);
                    int actualFine = (int)Math.Round(baseFine * percentRisk * 100);
                    ApplyFine(actualFine, Reason);
                }
                else
                {
                    // Outcome: Fined or jailed (based on outcome)
                    if (outcome <= (percentRisk / 5))
                    {
                        // Jailed
                        int maxJailTerm = (int)Math.Round(percentRisk * 1000); // 15% chance = 150 quarters max sentence (~38 years)
                        int jailTerm = (int)Math.Round(outcome * maxJailTerm / (percentRisk / 5));

                        GoToJail(jailTerm);
                        // Todo: consider bail mechanic.
                    }
                    else
                    {
                        // Fined. Fines in this range are a little more severe, but cap at around $10M (50% risk, 50% outcome = 25M, but most high risks are ~25-35%)
                        long baseFine = (long)Math.Round(percentRisk * 1000 * 1000 * 100);
                        long actualFine = (long)Math.Round(baseFine * outcome);
                        ApplyFine(actualFine, Reason);
                    }
                }

                return false;
            }
            else
            {
                AvoidedCriminalDetectionCount++;
            }
            return true;
        }

        string[] CriminalHeadlines;
        public void SetCriminalHeadlines(params string[] Reasons)
        {
            CriminalHeadlines = Reasons;
        }

        public GamePlayer SelectRandomTarget()
        {
            // Only select targets that aren't in jail or dead
            for (int i = 0; i < 20; i++)
            {
                GamePlayer selected;
                if (ThisRound.Rank > 1)
                {
                    int pickIndex = Parent.RandomNext(1, ThisRound.Rank) - 1;
                    selected = Parent.Players[pickIndex];
                }
                else
                {
                    int pickIndex = Parent.RandomNext(1, 50);
                    selected = Parent.Players[pickIndex];
                }
                if (!(selected.Dead || selected.InJail)) return selected;
            }
            // If we can't find one, just find the first player that isn't us that isn't dead or in jail.
            foreach(GamePlayer p in Parent.Players)
            {
                if (p.Dead || p.InJail || p == this) continue;
                return p;
            }
            return null; // This should never happen.
        }

        public void AttemptAssasination(double risk, GamePlayer specificTarget = null)
        {
            string[] failedOutcomes = new string[] {
                "Police nab would-be Assassin\nThe plot was ultimately traced back to {0}, the prosecuting attorney is hoping for a serious penalty in this case"
            };

            if(specificTarget == null)
            {
                specificTarget = SelectRandomTarget();
            }

            SetCriminalHeadlines(failedOutcomes);
            if(DidEvadeCriminalRisk(risk))
            {
                // Success! Kill the target!
                specificTarget.Kill("{0} was assasinated by " + Name + " at the age of {1}", "...But nobody ever knew. It was covered up very well");

                // Add "Mysterious death" news
                QueueNews($"Mysterious death of {specificTarget.Name}\nAuthorities say there are no signs of foul play, for this unusual death");

                SetResult($"Operation was successful, {specificTarget.Name} is no more.", true);
            }
            else
            {
                string[] failedAttempts = new string[]
                {
                    "The police informed {0} they captured an assassin on their property\nApparently the would-be murderer fell victim to a number of aggressive dogs in the area",
                    "Local bar-goer narrowly evaded death\n{0} noticed his drink was a very unusual color, a toxicologist who happened to be at the bar identified the poison by scent and alerted the police"
                };

                string failure = failedAttempts[Parent.RandomNext(0, failedAttempts.Length)];
                string formattedReason = string.Format(failure, this.Name);
                string[] parts = formattedReason.Split('\n');
                if (parts.Length < 2) parts = new string[] { formattedReason, "Ha, the editor is on vacation and I can say whatever I want" };

                // Spectacular failure! Inform the target that they narrowly evaded death
                MediaEvent me = new MediaEvent(specificTarget, parts[0], parts[1], "We found out who was behind this attempt...", true);
                me.InstigatingPlayer = this;
                me.EventType = MediaEventType.Victim;
                Parent.NextRoundNews.Add(me);
            }
        }


        public void SmearCampaign(double effectiveness, double risk, GamePlayer specificTarget = null)
        {
            double outcome = Parent.NextPercent(0, 1);
            GamePlayer Instigator = null;
            string[] Smears = new string[]
            {
                "Anonymous sources allege {0} has a drug problem\nOf course we can't reveal our sources, but they're probably right in our estimation",
                "{0} is a stalker, sources say\nBlurry footage shows {0} following someone in the dark, you can tell by the hat",
                "Does {0} have cancer, and are they hiding it?\nWeb denizen diagnoses {0}, claiming he can tell by \"the pixels\""
            };
            string outcomeString;
            string myoutcome = "";

            if (specificTarget == null)
            {
                // Pick a random target with higher rank, or in the top 50 if we are #1
                specificTarget = SelectRandomTarget();
            }

            if (outcome < risk)
            {
                // Caught! Target will know it was you trying to smear them.
                Instigator = this;
                outcomeString = "We found out who was behind these false allegations...";
                myoutcome = "Details of your involvement got out, there may be retaliation...";
                FailedSmearCampaigns++;
                SetResult($"Oh no! {specificTarget.Name} found out we were behind the smear campaign!", false);
            }
            else
            {
                // Not caught
                outcomeString = "The motive for this smear campaign is unknown";
                myoutcome = "The campaign was successful!";
                SmearCampaigns++;
                SetResult($"Success! {specificTarget.Name}'s good name has been rubbed in the dirt.", true);
            }


            string smear = Smears[Parent.RandomNext(0, Smears.Length)];
            string formattedReason = string.Format(smear, this.Name);
            string[] parts = formattedReason.Split('\n');
            if (parts.Length < 2) parts = new string[] { formattedReason, "Ha, the editor is on vacation and I can say whatever I want" };

            specificTarget.MediaAffectPopularity(-effectiveness, 0.1);

            MediaEvent me = new MediaEvent(specificTarget, parts[0], parts[1], outcomeString, true);
            me.InstigatingPlayer = Instigator;
            me.EventType = MediaEventType.Smear;
            Parent.NextRoundNews.Add(me);

            // Also queue the smear to this player's feed, for reference.
            me = new MediaEvent(this, parts[0], parts[1], myoutcome, false);
            Parent.NextRoundNews.Add(me);

        }


        public void MediaAffectPopularity(double effect, double variability)
        {
            ImproveSentiment(-effect - variability, -effect + variability, ref ThisRound.PublicSentiment);
        }


        // Helpers for computing outcomes

        public bool AffinityGreaterThan(int affinity, double percent)
        {
            return AffinityAsPercent(affinity) >= percent;
        }
        public double AffinityAsPercent(int affinity)
        {
            return (double)affinity / 0x10000;
        }
        public void ImproveAffinity(double minPercent, double maxPercent, ref int affinity)
        {
            if (minPercent < -.8) minPercent = -.8;
            if (maxPercent > .8) maxPercent = .8;

            double start = AffinityAsPercent(affinity);
            double percent = Parent.NextPercent(minPercent, maxPercent);
            double remain = 1 - start;
            if(percent < 0)
            {
                remain = start + 1; // Distance to -1;
            }
            double newAffinity = start + remain * percent; // Percentage of the remaining distance to 1.0 (diminishing returns)
            affinity = (int)Math.Round(newAffinity * 0x10000);
        }
        public void ImproveSentiment(double minPercent, double maxPercent, ref int percent)
        {
            ImproveAffinity(minPercent, maxPercent, ref percent);
        }

        public void AddFans(double minPercent, double maxPercent, ref long fans)
        {
            // Add either a % of the current fans, or a % of the remaining people in the world, whichever is lower. (for positive growth)
            long percentSource = fans;
            long remainingPeople = Engine.WorldPopulation - fans;

            double percent = Parent.NextPercent(minPercent, maxPercent);
            if (percent > 0 && remainingPeople < percentSource) percentSource = remainingPeople;
            // Prevent things from getting stuck when there are no fans.
            if (percentSource < 10000) percentSource = 10000;

            long addFans = (long)Math.Round(percentSource * percent);

            fans += addFans;
            if (fans < 0) fans = 0;
        }

    }

    class RoundAction
    {
        public RoundAction(GamePlayer p, GameAction a, int index, int mediaIndex = -1)
        {
            Action = a;
            ActionIndex = index;
            ActionMediaIndex = mediaIndex;
            if (a.CostString != null) CostString = a.CostString(p, a);
            if (a.RiskString != null) RiskString = a.RiskString(p, a);
            if (a.BenefitString != null) BenefitString = a.BenefitString(p, a);
        }

        public RoundAction(string Name, string Description, string Cost)
        {
            // Not actually an action related to the game, but this action is used as a helper in the UI.
            Action = new GameAction() { Title = Name, Description = Description };
            ActionIndex = -1;
            CostString = Cost;
        }

        public MediaEvent Media;
        public GameAction Action;
        public int ActionIndex;
        public int ActionMediaIndex = -1; // Only set for media actions.

        public string CostString, RiskString, BenefitString;

        public string ResultString;
        public bool ResultPositive;
    }

    // Deal with media stuff after getting core game loop working.
    class MediaEvent
    {
        public MediaEvent(GamePlayer affected, string _heading, string _subheading, string _outcome, bool _important)
        {
            Heading = _heading;
            SubHeading = _subheading;
            OutcomeText = _outcome;
            Important = _important;
            AffectedPlayer = affected;
        }

        public string Heading;
        public string SubHeading;
        public string OutcomeText;

        public GamePlayer AffectedPlayer;
        public GamePlayer InstigatingPlayer; // If a media risk is failed, the instigating player will be known. todo: figure out how to manage retalitation actions.

        public bool Important;
        public bool Reacted;

        public MediaEventType EventType;
    }

    enum MediaEventType
    {
        Mundane, // Not meaningful
        Smear, // Received a smear from another player
        Vanity, // Current player's media hijinx, prompting for further engagement
        Criminal, // Current player failed to do something criminal
        Victim // Current player was nearly the victim of another player's criminal activity
    }
}
