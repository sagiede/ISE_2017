using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer;
using MarketItems;
using MarketClient;
using MarketClient.Utils;
using MarketClient.DataEntries;
using System.Timers;


namespace Pilots
{
    //class that responsibale to operate the semi auto pilot
    public static class SemiPilot
    {

        private static int commodity;
        private static int extremePrice;
        private static int amount;
        private static bool requestType;               // true - buy request. false - sell request
        public static Timer semiPilotTimer;
        public static String eventsData;                //describing the events - what we bought and what we sold

        //function that runs the semi auto pilot according to the user conditions
        public static void runSemiPilot(int id, int price, int amount, bool requestKind)
        {
            SemiPilot.commodity = id;
            SemiPilot.extremePrice = price;
            SemiPilot.amount = amount;
            SemiPilot.requestType = requestKind;
            SemiPilot.eventsData = "";
            SetTimer();
        }
        //function to stop the semi auto pilot
        public static void stopSemiPilot()
        {
            semiPilotTimer.Stop();
        }
        //set a timer to check the market every seconed
        private static void SetTimer()
        {
            semiPilotTimer = new System.Timers.Timer(1000);
            semiPilotTimer.Elapsed += checkPrice;
            semiPilotTimer.AutoReset = true;
            semiPilotTimer.Start();
        }
        //semi auto pilot function that summoned every seconed
        private static void checkPrice(Object source, ElapsedEventArgs e)
        {
            try
            {
                MarketClientConnection pc = new MarketClientConnection();
                MarketCommodityOffer query = (MarketCommodityOffer)pc.SendQueryMarketRequest(commodity);
                if (requestType)            //buy when price of ask of commidity id is lower then extremePrice
                {
                    int stockPrice = query.ask;
                    if (stockPrice <= extremePrice & amount > 0)
                    {
                        pc.SendBuyRequest(stockPrice, commodity, 1);
                        SemiPilot.amount = amount - 1;                  //reduce the amount we need to buy
                        eventsData += "Sent a request to buy " + amount + " shares of commodity number "
                                        + commodity + " for " + stockPrice + " per share";
                        if (amount == 0)                                //we accompliced to buy all of the asked amount
                        {
                            semiPilotTimer.Stop();                      //stop semi pilot           
                        }
                        return;
                    }
                }
                else
                {
                    int stockOffer = query.bid;
                    if (stockOffer >= extremePrice & amount > 0)
                    {
                        pc.SendSellRequest(stockOffer, commodity, 1);
                        SemiPilot.amount--;                             //reduce the amount we need to sell
                        eventsData += "Sent a request to sell " + amount + " shares of commodity number "
                                            + commodity + " for " + stockOffer + " per share";
                        if (amount == 0)                                //we accompliced to sell all of the asked amount
                            semiPilotTimer.Stop();                      //stop semi pilot  
                        return;
                    }
                }
            }
            catch (Exception e2)
            {
                eventsData += e2.Message;

            }
        }
    }
    //class that responsibale to operate the semi auto pilot
    public class AutoPilot
    {
        private static Timer timer = new Timer(2000);
        private static int requestsLeft = 18;               //count requests for not getting ban from server
        public static MarketClientConnection mc = new MarketClientConnection();
        private static Boolean act = false;
        private static Boolean activated = false;
        public static String actions = "";
        public static int lastCommodity = -1;

        public static void runPilot()
        {
            actions = "";
            if (!activated)
            {
                activated = true;
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
            }
            act = !act;
            timer.Enabled = act;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            increaseRequests(2);                    //seconed elpased so update the requests amount we can do
                         double funds = ((MarketUserData)mc.SendQueryUserRequest()).funds;
            LinkedList<Commodities> stockStatus = mc.SendQueryAllMarketRequest();

            int ask = 0;
            int bid = 0;
            int commodity = 0;

            for (commodity = 4; commodity <= 9; commodity++)
            {
                ask = stockStatus.ElementAt<Commodities>(commodity).info.ask;
                bid = stockStatus.ElementAt<Commodities>(commodity).info.bid;
                if (ask < bid)
                    break;                                //stop at commodity with bid higher then ask price

            }
            if (ask < bid)
                if (funds - ask > 0)                    //if we have enaugh money to buy, buy and sell
                {
                    String current = "";
                    mc.SendBuyRequest(ask, commodity, 5);
                    current += "Sent a request to buy commodity number " + commodity + " for "
                        + ask + " per share";
                    mc.SendSellRequest(bid, commodity, 5);
                    current += " and requested to sell it for " + bid + "\n";
                    if (lastCommodity == -1)
                        lastCommodity = commodity;
                    actions += current;                 //update string
                    increaseRequests(-2);               //decrease requests amount
                }
                else
                {
                    actions += "There is no more money\n";
                    return;
                }
        }
        
        private static void increaseRequests(int i)
        {
            if (requestsLeft + i <= 4)
            {
                timer.Stop();
                System.Threading.Thread.Sleep(2000);
                increaseRequests(4);
                timer.Start();
            }
            else if (requestsLeft + i > 20)
                requestsLeft = 20;
            else
                requestsLeft += i;
        }
    }

    public static class NewAutoPilot
    {
        private static Timer timer = new Timer(20000);
        public static MarketClientConnection mc = new MarketClientConnection();
        private static Boolean act = false;
        private static Boolean activated = false;
        public static String actions = "";
        private static int bestCommodity = 4;
        private static float bestProfit = 0;
        private static float[] profits;
        private static float[] buyingPrices;
        private static float[] sellingPrices;
        
        public static void runPilot()
        {
            actions = "";
            if (!activated)
            {
                profits = new float[10];
                buyingPrices = new float[10];
                sellingPrices = new float[10];
                activated = true;
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
                for (int i = 4; i < 10; i++)
                {
                    sellingPrices[i] = LogicLayer.History.getTodaysRecommendedSellPrice(i);
                    buyingPrices[i] = LogicLayer.History.getTodaysRecommendedBuyPrice(i);
                    float difference = sellingPrices[i] - buyingPrices[i];
                    profits[i] = (difference / buyingPrices[i]) * 100;
                    if (profits[i] > bestProfit)
                    {
                        bestCommodity = i;
                        bestProfit = profits[i];
                    }       
                }
            }
            act = !act;
            timer.Enabled = act;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            double funds = ((MarketUserData)mc.SendQueryUserRequest()).funds;
            for (int i = 4; i < 10; i++)
            {
                int amount = ((MarketUserData)mc.SendQueryUserRequest()).commodities.ElementAt(i).Value;
                if (amount != 0)
                {
                    mc.SendSellRequest((int)sellingPrices[i], i, amount);
                    actions += "Sold " + amount + " of commodity "
                        + i + " for " + (int)sellingPrices[i] + " per share, approximate profit: "
                        + profits[i] + "%\n";
                }
            }
            mc.SendBuyRequest((int)buyingPrices[bestCommodity], bestCommodity, (int)(funds / 20 / buyingPrices[bestCommodity]));
            actions += "Bought " + (int)(funds / 10 / buyingPrices[bestCommodity]) + " of commodity "
                        + bestCommodity + " for " + buyingPrices[bestCommodity] + " per share\n";
        }
    }
}

