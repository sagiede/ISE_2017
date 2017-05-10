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
    public class SemiPilot
    {

        private int commodity;
        private int extremePrice;
        private int amount;
        private bool buyRequest;               // true - buy request. false - sell request
        private bool keepingProcces;
        private static Timer aTimer;

        public SemiPilot(int id, int price, int amount, bool requestKind)
        {
            this.commodity = id;
            this.extremePrice = price;
            this.amount = amount;
            this.buyRequest = requestKind;
            this.keepingProcces = true;
        }

        public void runAlgo()
        {
            SetTimer();
            while (keepingProcces) { }
            return;
        }
        public void stopSemiPilot()
        {
            this.keepingProcces = false;
        }

        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += checkPrice;
            aTimer.AutoReset = true;
            aTimer.Start();
        }
        private void checkPrice(Object source, ElapsedEventArgs e)
        {
            MarketClientConnection pc = new MarketClientConnection();
            MarketCommodityOffer query = (MarketCommodityOffer)pc.SendQueryMarketRequest(commodity);
            if (buyRequest)            //buy when price of ask of commidity id is lower then extremePrice
            {
                int stockPrice = query.ask;
                if (stockPrice <= extremePrice)
                {
                    pc.SendBuyRequest(stockPrice, commodity, amount);
                    aTimer.Enabled = false;
                    keepingProcces = false;
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
                    aTimer.Enabled = false;
                    keepingProcces = false;
                    return;
                }
            }
        }

    }
    public static class AutoPilot
    {
        private static Timer timer = new Timer(2000);
        private static int commodity = 0;
        private static int requestsLeft = 18;
        private static Boolean keeapOnBuying = true;
        private static MarketClientConnection mc = new MarketClientConnection();
        private static Boolean act = false;

        public static void runPilot()
        {
            act = !act;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = act;
            // need to add stopping term from the GUI
            while (act)
            {
                while (keeapOnBuying) { }
                timer.Stop();
                System.Threading.Thread.Sleep(4000);
                requestsLeft += 16;
                keeapOnBuying = true;
                timer.Start();
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            requestsLeft += 2;

            if (requestsLeft <= 4)
                keeapOnBuying = false;

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
                    mc.SendSellRequest(bid, commodity, 1);
                    requestsLeft--;
                }
                else return;
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