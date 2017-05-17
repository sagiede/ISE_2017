using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketClient;
using MarketClient.Utils;
using MarketClient.DataEntries;

namespace MarketItems
{
    public abstract class Request
    {
        public object authentication { get; set; }
        public string type { get; set; }
    }
    public class BuySellRequest : Request
    {
        public int commodity { get; set; }
        public int amount { get; set; }
        public int price { get; set; }

    }
    public class QueryBuySellRequest : Request
    {
        public int id { get; set; }
    }
    public class QueryUserRequest : Request { }
    public class QueryMarketRequest : Request
    {
        public int commodity { get; set; }
    }
    public class CancelBuySellRequest : Request
    {
        public int id { get; set; }
    }

    public class MarketItemQuery : IMarketItemQuery
    {
        public object user { get; set; }
        public string type { get; set; }
        public int commodity { get; set; }
        public int amount { get; set; }
        public int price { get; set; }

        override
        public string ToString()
        {
            return "\nTransaction status: \n" + "User: " + user +
                                  "\nType: " + type +
                                  "\nCommodity: " + commodity +
                                  "\nAmmount: " + amount +
                                  "\nPrice: " + price;
        }
    }

    public class MarketUserData : IMarketUserData
    {
        public Dictionary<string, int> commodities { get; set; }
        public double funds { get; set; }
        public List<int> requests { get; set; }
        override    
        public string ToString()
        {
            string commTostring = "";
            string reqTostring = " ";
            foreach (var tmp in commodities)
            {
                commTostring += " stock " + tmp.Key.ToString() + ":" + tmp.Value.ToString();
            }
            foreach (var tmp in requests)
            {
                reqTostring += tmp + ", ";
            }
            if (reqTostring.Length > 1)
                reqTostring = reqTostring.Substring(0, reqTostring.Length - 2);

            return "User status: \n" + "User: " + "user52" +
                                  "\ncommodities: " + commTostring +
                                  "\nfunds: " + funds +
                                  "\nopen requests: " + reqTostring;
        }

    }

    public class MarketCommodityOffer : IMarketCommodityOffer
    {
        public int ask { get; set; }
        public int bid { get; set; }

        override
        public string ToString()
        {
            return "\nMarket status: \n" + "ask: " + ask + "\nbid: " + bid;
        }
    }

    public class CommodityOffer
    {
        public MarketCommodityOffer info { get; set; }

        override
        public string ToString()
        {
            return info.ToString();
        }
    }

    public class UserRequests
    {
        public int id { get; set; }
        public MarketItemQuery request { get; set; }

        override
        public string ToString()
        {
            return id + ": " + request;
        }
    }

    public class Commodities
    {
        public int id { get; set; }
        public MarketCommodityOffer info { get; set; }

        override
        public string ToString()
        {
            return "Commodity " + id + " " + info;
        }
    }
}
