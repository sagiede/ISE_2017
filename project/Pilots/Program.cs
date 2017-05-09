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

        public bool runAlgo()
        {
            SetTimer();
            while (keepingProcces) { }
            return true;
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
}
