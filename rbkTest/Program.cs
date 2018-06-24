using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkTest
{
    static class someConst
    {
        public const int traderPerType = 10;
        public const int yearCount = 60;
        public const int rebuildCount = 10;
        public const int notIndludeMy = 0; // 0 or 1
        public const bool largeLog = false;
        public const bool veryLargeLog = largeLog&&true;

        public static string getName(TraderTypes type)
        {
            string res;
            switch (type)
            {
                case TraderTypes.Cunning:
                    res = "Хитрец";
                    break;
                case TraderTypes.Quirky:
                    res = "Ушлый";
                    break;
                case TraderTypes.Rancorous:
                    res = "Злопамятный";
                    break;
                case TraderTypes.Threw:
                    res = "Кидала";
                    break;
                case TraderTypes.Unpredictable:
                    res = "Непредсказуемый";
                    break;
                case TraderTypes.My:
                    res = "Мой";
                    break;
                default:
                    res = "Альтруист";
                    break;
            }
            return res;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            bool loop = true;
            Dictionary<string, int> typesSuccessful = new Dictionary<string, int>();
            for (int i = 0; i < (int)TraderTypes.Length; i++)
            {
                typesSuccessful.Add(someConst.getName((TraderTypes)i), 0);
            }

            Console.Write(" --- Start program --- \r\n\r\n");

            while (loop)
            {
                for (int k = 0; k < someConst.rebuildCount; k++)
                {
                    if(someConst.largeLog)Console.Write("build #" + (k + 1).ToString() + " ");
                    Guild guild = new Guild();
                    guild.Init();
                    for (int i = 0; i < someConst.yearCount - 1; i++)
                    {
                        guild.Trade();
                        if(someConst.veryLargeLog)guild.PrintYearResult();
                        guild.NewYearInit();
                        
                        if (someConst.largeLog)
                        {
                            if (i % 10 == 0) Console.Write("*");
                        }
                    }
                    guild.Trade();
                    if (someConst.largeLog)
                    {
                        guild.PrintYearResult();
                    }
                    else
                    {
                        Console.Write("*");
                    }
                    for (int i = 0; i < typesSuccessful.Count; i++)
                    {
                        typesSuccessful[someConst.getName((TraderTypes)i)] += guild.getTraderCount((TraderTypes)i);
                    }
                }
                Console.Write("\r\n");
                int totalCount = 0;
                for (int i = 0; i < typesSuccessful.Count; i++)
                {
                    totalCount += typesSuccessful[someConst.getName((TraderTypes)i)];
                }

                foreach (var type in typesSuccessful.OrderByDescending(type => type.Value))
                {
                    string tab = "\t\t";
                    if (type.Key.Length > 6) tab = "\t";
                    Console.Write(type.Key + tab + ((float)type.Value / totalCount).ToString() + "\r\n");
                }
                for (int i = 0; i < typesSuccessful.Count; i++)
                {
                    typesSuccessful[someConst.getName((TraderTypes)i)] = 0;
                }
                Console.Write("\r\n Restart any/n? \r\n");
                var key = Console.ReadKey();
                Console.Write("\r\n");
                if (key.KeyChar == 'n')
                {
                    loop = false;
                }
            }
        }
    }

    enum TraderTypes
    {
        Altruist,
        Threw,
        Cunning,
        Unpredictable,
        Rancorous,
        Quirky,
        My,
        Length
    }

    class Guild
    {
        List<Trader> traders;

        public void Init()
        {
            traders = new List<Trader>();
            for(int i = 0; i < (int)TraderTypes.Length - someConst.notIndludeMy; i++)
            {
                for(int k = 0; k < someConst.traderPerType; k++)
                {
                    var trader = TraderCreator((TraderTypes)i);
                    trader.Init();
                    traders.Add(trader);
                }
            }
        }

        public void Trade()
        {
            for (int i = 0; i < traders.Count; i++)
            {
                var trader = traders[i];
                //Console.Write("Trader #" + i.ToString() + " trade with " + (i + 1).ToString() + " - " + traders.Count.ToString() + " traders \r\n");
                for (int k = i + 1; k < traders.Count; k++)
                {
                    int tradeCount = myRand.getTradeCount();
                    int myGold = trader.gold;
                    int otherGold = traders[k].gold;
                    while (tradeCount-- > 0)
                    {
                        trader.Trade(traders[k]);
                    }
                    if (someConst.veryLargeLog)
                    {
                        Console.Write(trader.type.ToString() + " vs " + traders[k].type.ToString() + " " +
                        (trader.gold - myGold).ToString() + " - " + (traders[k].gold - otherGold).ToString() + "\r\n");
                    }
                    trader.ReInit();
                    traders[k].ReInit();
                }
            }
        }

        public void PrintYearResult()
        {
            int count = 1;
            Console.Write("\r\n");
            foreach (Trader trader in traders.OrderByDescending(trader => trader.gold))
            {
                Console.Write(count++.ToString() + ". gold = " + trader.gold + " type - " + trader.type.ToString() + "\r\n");
            }
        }

        public void NewYearInit()
        {
            List<Trader> newTraders = new List<Trader>();
            int count = 0;
            int traderMax = someConst.traderPerType * (int)TraderTypes.Length;
            int traderPart = traderMax / 5;
            foreach (Trader trader in traders.OrderByDescending(trader => trader.gold))
            {
                trader.Init();
                if (count == traderMax - traderPart)
                {
                    break;
                }
                if (count < traderPart)
                {
                    var _trader = TraderCreator(trader.type);
                    _trader.Init();
                    newTraders.Add(_trader);
                }
                newTraders.Add(trader);
                count++;
            }
            traders = newTraders;
        }

        Trader TraderCreator(TraderTypes type)
        {
            Trader trader;
            switch (type)
            {
                case TraderTypes.Cunning:
                    trader = new CunningTrader();
                    break;
                case TraderTypes.Quirky:
                    trader = new QuirkyTrader();
                    break;
                case TraderTypes.Rancorous:
                    trader = new RancorousTrader();
                    break;
                case TraderTypes.Threw:
                    trader = new ThrewTrader();
                    break;
                case TraderTypes.Unpredictable:
                    trader = new UnpredictableTrader();
                    break;
                case TraderTypes.My:
                    trader = new MyTrader();
                    break;
                default:
                    trader = new AltruistTrader();
                    type = TraderTypes.Altruist;
                    break;
            }
            trader.type = type;
            return trader;
        }

        public int getTraderCount(TraderTypes type)
        {
            int res = 0;
            foreach (Trader trader in traders.OrderByDescending(trader => trader.gold))
            {
                if(trader.type == type)
                {
                    res++;
                }
            }
            return res;
        }

    }

    class Trader
    {
        public int gold = 0;
        public TraderTypes type;

        public bool lastDecision;

        public void Init()
        {
            gold = 0;
            lastDecision = true;
        }

        public int Trade(Trader trader)
        {
            var myDecision = GetDecision();
            var otherDecision = trader.GetDecision();
            lastDecision = otherDecision;
            trader.lastDecision = myDecision;

            if (myDecision && otherDecision)
            {
                trader.gold += 4;
                gold += 4;
            }
            else
            {
                if(!myDecision && !otherDecision)
                {
                    trader.gold += 2;
                    gold += 2;
                }
                else
                {
                    if(myDecision)
                    {
                        trader.gold += 5;
                        gold += 1;
                    }
                    if(otherDecision)
                    {
                        trader.gold += 1;
                        gold += 5;
                    }
                }
            }

            return 0;
        }

        public virtual bool GetDecision()
        {
            return true;
        }
        public virtual void ReInit(){}
    }

    class AltruistTrader : Trader
    {
        public override bool GetDecision()
        {
            return myRand.CheckRandomError(true);
        }
    }

    class ThrewTrader : Trader
    {
        public override bool GetDecision()
        {
            return myRand.CheckRandomError(false);
        }
    }

    class CunningTrader : Trader
    {
        public override bool GetDecision()
        {
            return myRand.CheckRandomError(lastDecision);
        }
        public override void ReInit()
        {
            lastDecision = true;
        }
    }

    class UnpredictableTrader : Trader
    {
        Random random = new Random();
        public override bool GetDecision()
        {
            return myRand.CheckRandomError(random.Next(2) == 1);
        }
    }

    class RancorousTrader : Trader
    {
        private bool memory = false;
        public override bool GetDecision()
        {
            if (!memory)
            {
                if(!lastDecision)
                {
                    memory = true;
                }
                return myRand.CheckRandomError(true);
            }
            return myRand.CheckRandomError(false);
        }
        public override void ReInit()
        {
            memory = false;
            lastDecision = true;
        }
    }

    class QuirkyTrader : Trader
    {
        private bool memory = false;
        private int decisionIndex = 0;
        private bool[] decisionWay = { true, false, true, true };

        public override bool GetDecision()
        {
            bool res = true;
            if (decisionIndex < 4)
            {
                if (!lastDecision)
                {
                    memory = true;
                }
                res = decisionWay[decisionIndex++];
            }
            else
            {
                if (!memory)
                {
                    res = lastDecision;
                }
                else
                {
                    res = false;
                }
            }
            return myRand.CheckRandomError(res);
        }

        public override void ReInit()
        {
            memory = false;
            decisionIndex = 0;
            lastDecision = true;
        }
    }

    class MyTrader : Trader
    {
        private bool memory = false;
        private int loseCount = 0;
        private int tradeCount = 0;
        private bool loseAfterOk = false;

        public override bool GetDecision()
        {
            if (!memory)
            {
                if (!lastDecision)
                {
                    loseCount++;
                    if (loseCount > 1)
                    {
                        memory = true;
                    }
                }
                return myRand.CheckRandomError(true);
            }
            return myRand.CheckRandomError(false);
        }
        //public override bool GetDecision()
        //{
        //    bool res = false;
        //    if (!loseAfterOk)
        //    {
        //        if (tradeCount > 0)
        //        {
        //            if (lastDecision && tradeCount == 1) loseCount = 1;
        //            if (lastDecision && tradeCount == 3) loseCount = 0;
        //            if (lastDecision && tradeCount == 4 && loseCount == 1)
        //            {
        //                //Console.Write(" --------------------- loseAfterOk = true \r\n");
        //                loseAfterOk = true;
        //            }
        //        }
        //        tradeCount++;
        //        return myRand.CheckRandomError(res);
        //    }
        //    return myRand.CheckRandomError(true);
        //}
        public override void ReInit()
        {
            memory = false;
            lastDecision = true;
            loseAfterOk = false;
            loseCount = 0;
            tradeCount = 0;
        }
    }

    static class myRand
    {
        static Random random = new Random();
        public static bool CheckRandomError(bool value)
        {
            if(random.Next(101) <= 5)
            {
                return !value;
            }
            return value;
        }
        public static int getTradeCount()
        {
            return random.Next(5, 11);
        }
    }
}
