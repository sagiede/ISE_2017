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
                if (stockPrice <= extremePrice)
                {
                    pc.SendBuyRequest(stockPrice, commodity, 1);
                    SemiPilot.amount--;
                    eventsData += "buy request has sended : \ncommodidy: "+commodity+"\nprice: "+stockPrice+ "\namount: 1 \n";
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
                if (stockOffer >= extremePrice)
                {
                    pc.SendSellRequest(stockOffer, commodity, 1);
                    SemiPilot.amount--;
                    eventsData += "sell request has sended : \ncommodidy: " + commodity + "\nprice: " + stockOffer + "\namount: 1 \n";
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
                    mc.SendBuyRequest(ask, commodity, 1);
                    actions += "bought " + commodity + " in " + ask;
                    mc.SendSellRequest(bid, commodity, 1);
                    actions += ", sold for " + bid + "\n";
                    Console.WriteLine(commodity + " " + ask + " " + bid);
                    increaseRequests(-2);
                }
                else
                {
                    actions += "there is no more money\n";
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