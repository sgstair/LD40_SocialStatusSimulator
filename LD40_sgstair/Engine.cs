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
        public List<MediaEvent> NextRoundNews = new List<MediaEvent>();



        public int Quarter = 1;
        public int Year = 0;
        public int YearBase = 2017;
        public int CurrentYear {  get { return YearBase + Year; } }


        public GamePlayer PrepareSinglePlayerGame()
        {
            GamePlayer human = new GamePlayer(this, "human", true);
            Players.Add(human);

            // Initial values for human.
            human.Values.Money = 1000000;
            human.Values.FanCount = 50000; // 50k fans
            human.Values.PublicSentiment = 0; // Neutral public sentiment
            human.Values.AffinityProfessional = 0; // Neutral affinities
            human.Values.AffinityMedia = 0; 
            human.Values.AffinitySocial = 0; 
            human.Values.AffinityCriminal = 0; 

            for (int i=0;i<500;i++)
            {
                GamePlayer cpu = new GamePlayer(this, i.ToString(), false);
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

            return human;
        }

        void AddCpuPlayer()
        {
            // future, when we get to replacing dead cpu players.
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
            }

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
            NextRoundNews.Clear();

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
                    p.Dead = true;
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

    class GamePlayer
    {
        public Engine Parent;

        public string Name;
        public bool Human;

        public double Age {  get { return Parent.Year + BaseAge + (Parent.Quarter-1) * 0.25; } }
        public double BaseAge = 25;
        public double LifeExpectancy = 70;
        public bool Dead = false;

        public PlayerValues Values = new PlayerValues(); // Master value list for player
        public PlayerValues ThisRound; // Accumulated values as player takes actions, but not fully applied yet.
        public PlayerValues LastRound;

        public GamePlayer(Engine parentEngine, string playerName, bool playerHuman = false)
        {
            Parent = parentEngine;
            Name = playerName;
            Human = playerHuman;
        }

        List<RoundAction> RoundActions = new List<RoundAction>();


        public override string ToString()
        {
            return $"GamePlayer('{Name}',{Math.Floor(Age)}/{Math.Floor(LifeExpectancy)}, #{Values.Rank}, {GameFormat.FormatMoney(Values.Money)}, {GameFormat.FormatFans(Values.FanCount)}";
        }


        public void BeginRound()
        {
            Values.TimeRemaining = 90; // 90 days per quarter, roughly
            if(LastRound == null) LastRound = (PlayerValues)Values.Clone(); // First round only.
            ThisRound = (PlayerValues)Values.Clone();

            // Public perception wanders a bit
            ImproveSentiment(-0.01, 0.01, ref ThisRound.PublicSentiment);

            // Add fans based on public perception and existing fanbase
            double sentimentPercent = 0.05 * AffinityAsPercent(ThisRound.PublicSentiment);
            AddFans(sentimentPercent, sentimentPercent + 0.02, ref ThisRound.FanCount);

            // Make some money based on the size of the fanbase.
            double moneyPerFan = Parent.NextPercent(0.03, 0.18);
            long addMoney = (long)Math.Round(ThisRound.FanCount * moneyPerFan);
            ThisRound.Money += addMoney;
        }

        public void CompleteRound()
        {
            LastRound = (PlayerValues)Values.Clone();
            Values = (PlayerValues)ThisRound.Clone();
        }


        public List<RoundAction> PossibleActions()
        {
            List<RoundAction> actions = new List<RoundAction>();
            if (Dead) return actions;

            for(int i=0;i<EngineData.Actions.Length;i++)
            {
                GameAction a = EngineData.Actions[i];
                if(a.CanUseAction(this,a))
                {
                    actions.Add(new RoundAction(this, a, i));
                }
            }
            return actions;
        }

        public void CommitAction(RoundAction a)
        {
            a.Action.CommitAction(this, a.Action);
            RoundActions.Add(a);
        }


        // AI

        public void PlayAI()
        {
            while(true)
            {
                if (Parent.PercentChance(0.05)) return; // 1 in 20 chance to just stop here.

                // Enumerate actions
                List<RoundAction> actions = PossibleActions();
                if (actions.Count == 0) return;

                // Pick one
                int index = Parent.RandomNext(0, actions.Count);
                CommitAction(actions[index]);
            }
        }

        // News
        public void QueueNews(string blurb, string subBlurb, string outcome, bool highlight = false)
        {
            MediaEvent me = new MediaEvent(this, blurb, subBlurb, outcome, highlight);
            Parent.NextRoundNews.Add(me);
        }

        public void QueueNews(string newsConcentrate, string outcome = null, bool highlight = false)
        {
            string formattedReason = string.Format(newsConcentrate, this.Name);
            string[] parts = formattedReason.Split('\n');
            if (parts.Length < 2) parts = new string[] { formattedReason, "Ha, the editor is on vacation and I can say whatever I want" };
            QueueNews(parts[0], parts[1], outcome, highlight);
        }

        /// <summary>
        /// Meaningless news - just exists to make the headlines a little more interesting.
        /// </summary>
        public void AddFlavorHeadline(params string[] Reasons)
        {
            string Reason = Reasons[Parent.RandomNext(0, Reasons.Length)];
            QueueNews(Reason);
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

            QueueNews(fineReason, outcome, true);
        }


        public bool DidEvadeCriminalRisk(double percentRisk)
        {
            double outcome = Parent.NextPercent(0, 1);
            if(outcome <= percentRisk)
            {
                // Risk bit you. Now we decide how to punish the player, depending on the risk and outcome %
                // What was the player even doing? Decide.

                string Reason = CriminalHeadlines[Parent.RandomNext(0, CriminalHeadlines.Length)];

                if( percentRisk <= 0.5 )
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

                        // Todo: figure this part out.
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
            return true;
        }

        string[] CriminalHeadlines;
        public void SetCriminalHeadlines(params string[] Reasons)
        {
            CriminalHeadlines = Reasons;
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

            if(outcome < risk)
            {
                // Caught! Target will know it was you trying to smear them.
                Instigator = this;
                outcomeString = "We found out who was behind these false allegations...";
            }
            else
            {
                // Not caught
                outcomeString = "The motive for this smear campaign is unknown";
            }

            if(specificTarget == null)
            {
                // Pick a random target with higher rank, or in the top 50 if we are #1
                if(ThisRound.Rank > 1)
                {
                    int pickIndex = Parent.RandomNext(1, ThisRound.Rank) - 1;
                    specificTarget = Parent.Players[pickIndex];
                }
                else
                {
                    int pickIndex = Parent.RandomNext(1, 50);
                    specificTarget = Parent.Players[pickIndex];
                }
            }

            string smear = Smears[Parent.RandomNext(0, Smears.Length)];
            string formattedReason = string.Format(smear, this.Name);
            string[] parts = formattedReason.Split('\n');
            if (parts.Length < 2) parts = new string[] { formattedReason, "Ha, the editor is on vacation and I can say whatever I want" };

            MediaEvent me = new MediaEvent(specificTarget, parts[0], parts[1], outcomeString, true);
            me.InstigatingPlayer = Instigator;
            Parent.NextRoundNews.Add(me);
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
        public RoundAction(GamePlayer p, GameAction a, int index)
        {
            Action = a;
            ActionIndex = index;
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

        public GameAction Action;
        public int ActionIndex;

        public string CostString, RiskString, BenefitString;
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
        public bool CanRespond; // If this or InstigatingPlayer is set, options exist to respond to the news story.
    }
}
