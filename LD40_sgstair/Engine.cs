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

                cpu.Values.Money = 100000 * i + r.Next(0, 10000000);
                cpu.Values.FanCount = 10000 * i + r.Next(0, 100000);
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
            // Prepare players for this round
            foreach(GamePlayer p in Players)
            {
                p.BeginRound();                
            }

            // Advance buffered media fallout
        }

        public void CompleteRound()
        {
            // Play all the AI players
            foreach (GamePlayer p in Players)
            {

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

        public GamePlayer(Engine parentEngine, string playerName, bool playerHuman = false)
        {
            Parent = parentEngine;
            Name = playerName;
            Human = playerHuman;
        }

        List<RoundAction> RoundActions = new List<RoundAction>();

        


        public void BeginRound()
        {
            Values.TimeRemaining = 90; // 90 days per quarter, roughly
            ThisRound = (PlayerValues)Values.Clone();

            // Public perception wanders a bit
            ImproveSentiment(-0.01, 0.01, ref ThisRound.PublicSentiment);

            // Add fans based on public perception and existing fanbase
            double sentimentPercent = 0.05 * AffinityAsPercent(ThisRound.PublicSentiment);
            AddFans(sentimentPercent, sentimentPercent + 0.02, ref ThisRound.FanCount);
        }

        public void CompleteRound()
        {
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

    }
}
