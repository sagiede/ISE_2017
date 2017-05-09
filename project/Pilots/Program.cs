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
    class SemiPilot {

        private int commodity;
        private int extremePrice;
        private int amount;
        private bool request;               // true - buy request. false - sell request
        private static Timer aTimer;

        public SemiPilot(int id, int price, int amount, bool requestKind)
        {
            this.commodity = id;
            this.extremePrice = price;
            this.amount = amount;
            this.request = requestKind;
            SetTimer();

        }

        public bool runAlgo()
        {
            aTimer.Enabled = true;
            return true;
        }
        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += checkPrice;
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
        }
        private void checkPrice(Object source, ElapsedEventArgs e)
        {
            MarketClientConnection pc = new MarketClientConnection();
            MarketCommodityOffer query = (MarketCommodityOffer)pc.SendQueryMarketRequest(commodity);
            if (request)            //buy when price of ask of commidity id is lower then extremePrice
            {
                int stockPrice = query.ask;
                if (stockPrice <= extremePrice)
                {
                    pc.SendBuyRequest(stockPrice, commodity, amount);
                    aTimer.Enabled = false;
                    return;
                }
            }
            else
            {
                int stockOffer = query.bid;
                if (stockOffer >= extremePrice)
                {
                    pc.SendSellRequest(stockOffer, commodity, amount);
                    aTimer.Enabled = false;
                    return;
                }
            }
        }
            
    }
}
