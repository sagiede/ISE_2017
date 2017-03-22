using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketClient;
using MarketClient.Utils;


namespace LogicLayer
{
    public class Request
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
    public class MarketItemQuery : IMarketItemQuery 
    {
        public object user { get; set; }
        public string type { get; set; }
        public int commodity { get; set; }
        public int amount { get; set; }
        public int price { get; set; }

    }
    class PipeConection : IMarketClient
    {
      public static int SendBuyRequest(int price, int commodity, int amount)
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string key= "123";

            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new BuySellRequest();
            item.type = "buy";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
            item.authentication = new Dictionary<string,string>(){ { "token", token }, { "user", "user52" } };

            string output= client.SendPostRequest<BuySellRequest> ("http://ise172.ise.bgu.ac.il", "user52", token,item);
            int integerOutput; 
            int.TryParse(output,out integerOutput);
            return integerOutput;

        }

       public int SendSellRequest(int price, int commodity, int amount) {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string key = "123";
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new BuySellRequest();
            item.type = "sell";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
            item.authentication = new Dictionary<string, string>() { { "token", token }, { "user", "user52" } };

            string output = client.SendPostRequest<BuySellRequest>("http://ise172.ise.bgu.ac.il", "user52", token, item);
            int integerOutput;
            int.TryParse(output, out integerOutput);
            return integerOutput;

        }
        public IMarketItemQuery SendQueryBuySellRequest(int id) {

        }
        public IMarketUserData SendQueryUserRequest(){ }
        public IMarketCommodityOffer SendQueryMarketRequest(int commodity){ }
        public bool SendCancelBuySellRequest(int id){ }
    }
}
