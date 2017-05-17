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

    public static class SemiPilot
    {

        private static int commodity;
        private static int extremePrice;
        private static int amount;
        private static bool requestType;               // true - buy request. false - sell request
        public static Timer semiPilotTimer;
        public static String eventsData;

        public static void runSemiPilot(int id, int price, int amount, bool requestKind)
        {
            SemiPilot.commodity = id;
            SemiPilot.extremePrice = price;
            SemiPilot.amount = amount;
            SemiPilot.requestType = requestKind;
            SemiPilot.eventsData = "";
            SetTimer();
        }
        public static void stopSemiPilot()
       {
            semiPilotTimer.Stop();
       }

        private static void SetTimer()
        {
            semiPilotTimer = new System.Timers.Timer(1000);
            semiPilotTimer.Elapsed += checkPrice;
            semiPilotTimer.AutoReset = true;
            semiPilotTimer.Start();
        }
        private static void checkPrice(Object source, ElapsedEventArgs e)
        {
            MarketClientConnection pc = new MarketClientConnection();
            MarketCommodityOffer query = (MarketCommodityOffer)pc.SendQueryMarketRequest(commodity);
            if (requestType)            //buy when price of ask of commidity id is lower then extremePrice
            {
                int stockPrice = query.ask;
                if (stockPrice <= extremePrice & amount>0)
                {
                        pc.SendBuyRequest(stockPrice, commodity, 1);
                        SemiPilot.amount = amount - 1;
                        eventsData += "Sent a request to buy " + amount + " shares of commodity number " 
                                        + commodity + " for " + stockPrice + " per share";
                        if (amount == 0)
                    {
                        semiPilotTimer.Stop();
                    }
                    return;
                }
            }
            else
            {
                int stockOffer = query.bid;
                if (stockOffer >= extremePrice & amount>0)
                {
                    pc.SendSellRequest(stockOffer, commodity, 1);
                    SemiPilot.amount--;
                    eventsData += "Sent a request to sell " + amount + " shares of commodity number "
                                        + commodity + " for " + stockOffer + " per share";
                    if (amount ==0)
                    semiPilotTimer.Stop();
                    return;
                }
            }
        }

    }
  
    
    public class AutoPilot
    {
        private static Timer timer = new Timer(2000);
        private static int requestsLeft = 18;
        public static MarketClientConnection mc = new MarketClientConnection();
        private static Boolean act = false;
        private static Boolean activated = false;
        public static String actions = "";
        public static int lastCommodity;
     
        public static void runPilot()
        {
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
            increaseRequests(2);

            double funds = ((MarketUserData)mc.SendQueryUserRequest()).funds;

            LinkedList<Commodities> stockStatus = mc.SendQueryAllMarketRequest();

            int ask = 0;
            int bid = 0;
            int commodity = 0;

            for (commodity = 0; commodity <= 9; commodity++)
            {
                ask = stockStatus.ElementAt<Commodities>(commodity).info.ask;
                bid = stockStatus.ElementAt<Commodities>(commodity).info.bid;
                if (ask < bid)
                    break;
            }

            if (ask < bid)
            {
                if (funds - ask > 0)
                {
                    String current = "";
                    mc.SendBuyRequest(ask, commodity, 1);
                    current += "Sent a request to buy commodity number " + commodity + " for " 
                        + ask + " per share";
                    mc.SendSellRequest(bid, commodity, 1);
                    current += " and requested to sell it for " + bid + "\n";
                    lastCommodity = commodity;
                    actions += current;
                    increaseRequests(-2);
                }
                else
                {
                    actions += "There is no more money\n";
                    return;
                }
            }
            else
            {
                if (commodity == 9)
                    commodity = 0;
                else commodity++;
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
}