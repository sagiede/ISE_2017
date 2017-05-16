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
        private static Timer semiPilotTimer;

        public static void runSemiPilot(int id, int price, int amount, bool requestKind)
        {
            SemiPilot.commodity = id;
            SemiPilot.extremePrice = price;
            SemiPilot.amount = amount;
            SemiPilot.requestType = requestKind;
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
                    pc.SendBuyRequest(stockPrice, commodity, amount);
                    semiPilotTimer.Enabled = false;
                    semiPilotTimer.Stop();
                    return;
                    // comment
                }
            }
            else
            {
                int stockOffer = query.bid;
                if (stockOffer >= extremePrice)
                {
                    pc.SendSellRequest(stockOffer, commodity, amount);
                    semiPilotTimer.Enabled = false;
                    semiPilotTimer.Stop();
                    return;
                }
            }
        }

    }
  
    
    public class AutoPilot
    {
        private static Timer timer = new Timer(2000);
        private static int commodity = 0;
        private static int requestsLeft = 18;
        public static MarketClientConnection mc = new MarketClientConnection();
        private static Boolean act = false;
        public static String actions = "";
       
      

        public static void runPilot()
        {
            act = !act;
            if (act)
            {
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
            }
            timer.Enabled = act;
            // need to add stopping term from the GUI
        }
       
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            
              requestsLeft += 2;

              if (requestsLeft <= 4)
              {
                  timer.Stop();
                  System.Threading.Thread.Sleep(4000);
                  requestsLeft += 16;
                  timer.Start();
              }



                   double funds = ((MarketUserData)mc.SendQueryUserRequest()).funds;

                   IMarketCommodityOffer stockStatus = mc.SendQueryMarketRequest(commodity);
                   int ask = ((MarketCommodityOffer)stockStatus).ask;
                   int bid = ((MarketCommodityOffer)stockStatus).bid;

              if (ask < bid)
              {
                if (funds - ask > 0)
                {
                    mc.SendBuyRequest(ask, commodity, 1);
                    requestsLeft--;
                    actions += "bought " + commodity + " in " + ask;
                    mc.SendSellRequest(bid, commodity, 1);
                    requestsLeft--;
                    actions += ", sold for " + bid + "\n";

                }
                else
                {
                    actions += "there is no more money";
                    return;
                }
               }
               else
               {
                   // temporarly to 9
                   if (commodity == 9)
                      commodity = 0;
                  else commodity++;


           }
           
        }

    }

    }