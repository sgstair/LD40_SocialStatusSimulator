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
            new GameAction {
                Title = "Work on a small project", Description = "Make something small your fans will appreciate, that won't take too much time.",
                MoneyCost = 50000, TimeCost = 45,
                BenefitString = (Player, Action) => "Improve your popularity a bit.",
                CommitAction = (Player, Action) =>
                {
                    GameAction.DeductCost(Player, Action);
                    Player.ImproveSentiment(0, 0.01, ref Player.ThisRound.PublicSentiment);
                    Player.AddFans(0.005, 0.03, ref Player.ThisRound.FanCount);
                }
            },
            new GameAction
            {
                Title = "Improve public sentiment", Description = "Work on some community projects that will improve people's perception of you.",
                MoneyCost = 10000, TimeCost = 70,
                CommitAction = (Player, Action) =>
                {
                    GameAction.DeductCost(Player, Action);
                    Player.ImproveSentiment(0.03, 0.12, ref Player.ThisRound.PublicSentiment);
                }
            }



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

        public static bool CanUseBase(GamePlayer Player, GameAction Action)
        {
            return Player.ThisRound.Money >= Action.MoneyCost && Player.ThisRound.TimeRemaining > Action.TimeCost;
        }

        public static void DeductCost(GamePlayer Player, GameAction Action)
        {
            Player.ThisRound.Money -= Action.MoneyCost;
            Player.ThisRound.TimeRemaining -= Action.TimeCost;
        }
        public static string DefaultCostString(GamePlayer Player, GameAction Action)
        {
            List<string> stringParts = new List<string>();
            if (Action.MoneyCost > 0) { stringParts.Add(GameFormat.FormatMoney(Action.MoneyCost)); }
            if (Action.TimeCost > 0) { stringParts.Add(GameFormat.FormatTime(Action.TimeCost)); }
            if (stringParts.Count == 0) return "FREE!";
            return string.Join(", ", stringParts);
        }



        public string Title;
        public string Description;

        public long MoneyCost;
        public int TimeCost;

        public Func<GamePlayer, GameAction, string> CostString = DefaultCostString;
        public Func<GamePlayer, GameAction, string> RiskString;
        public Func<GamePlayer, GameAction, string> BenefitString;

        // Determine if the action can be used
        public Func<GamePlayer, GameAction, bool> CanUseAction = CanUseBase;
        public Action<GamePlayer, GameAction> CommitAction = DeductCost;
    }
}
