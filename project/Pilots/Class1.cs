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

            }
        }
    }
    //class that responsibale to operate the semi auto pilot
    public class AutoPilot
    {
        private static Timer timer = new Timer(2000);
        private static int requestsLeft = 18;               //count requests for not getting ban from server
        public static MarketClientConnection mc = new MarketClientConnection();
        private static Boolean act = false;                 //tells the auto pilot to make actions or not              
        private static Boolean activated = false;           //tellsif the auto pilot runs or not
        public static String actions = "";                  //represent what the auto pilot have done so far
        public static int lastCommodity = -1;               //last commodity we did action on (initiate with false value)

        //function that runs the auto pilot
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
            timer.Enabled = act;                            //tell the timer to run according to act
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {

                increaseRequests(2);                    //seconed elpased so update the requests amount we can do

                double funds = ((MarketUserData)mc.SendQueryUserRequest()).funds;
                LinkedList<Commodities> stockStatus = mc.SendQueryAllMarketRequest();

                int ask = 0;
                int bid = 0;
                int commodity = 0;
                
                for (commodity = 0; commodity <= 9; commodity++)            //run oncommodities list
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
                        mc.SendBuyRequest(ask, commodity, 1);
                        current += "Sent a request to buy commodity number " + commodity + " for "
                            + ask + " per share";
                        mc.SendSellRequest(bid, commodity, 1);
                        current += " and requested to sell it for " + bid + "\n";
                        if(lastCommodity == -1)             //update the first commodity case
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
            catch (Exception e2){}
        }
        //private functions that control timer and updating the requests amount we allow tp make
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

