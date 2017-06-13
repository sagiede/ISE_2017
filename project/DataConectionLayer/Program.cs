using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketItems;
using MarketClient;
using MarketClient.Utils;
using MarketClient.DataEntries;
using log4net;
using System.IO;
using System.Timers;
using log4net.Appender;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace LogicLayer
{

    public class MarketClientConnection : IMarketClient //the class that send/get information from the server
    {// key
        private static string key = @"-----BEGIN RSA PRIVATE KEY-----
MIICXgIBAAKBgQDTBpVK3vFeDFOW6bGIGg3Uu7Gv6sYhLqhxAVwhyV87huH2cCg5
ZdWdHfgPB5XlwdPQuAJhgM2KtldAtzBUzUUMHj6uvSrUQ6ovPMYPmUq0CraKlBfr
AgTqBkzOZUL0m72acwZThDtUxWWJ9tfgej758Gx23IwQMcWdjfmi1vMkXwIDAQAB
AoGBAJ6H6YwPpGA0/m73LQnEGPPh9YDk7Odst9n9XYt8TnDXydVrOKy2Fh4sr3gL
CM9MJ6Y7Nn6tVryIc1AIzuRGOUGdz/IpxsTLFvgTSqMXUazVy39hoUUBGloWhUQ7
w+Uicbixt5v4SGTEi69+EBFMPplSrV1k5eKc4Q/6n5fuPWTBAkEA32g+9wyEDX3A
WpVXuD7IDsr/LQHQ4dckoxUF9hzo2EAXT2QyQH8ELpmFCWj0Jy2iGnfzH/e0O90c
N5DydVerLwJBAPHP6pdeXsLQry5h5Qh8uO/C9dV2GEh4Frn7lnhx53/b6/echr3f
qsGzwee58ngHB3vnY+nR/57+Dvz/ceyBDdECQQCADCUCvpa1kNz+TljPzpQl/m3R
oxfRSdnC61rWXG2M/PcfVwOCegqwludspE5EYmBmIVgle3k/UpVIt/hwD0abAkB1
7irA69tXM6NcAY5Ll1gyRmjSVCf/n+GljpeR4is+5iist//WtjB3C43zz3H7K6Jw
wSavMCV0iv8QUBxldYMhAkEAkYx4UBaYXMr/byar4UYkdTYuxag+iXFifBSqIYY4
sybKv1Ahjdz9bcvIYbauBzJPjL7n1u68fGPXcaKYDzjo3w==
-----END RSA PRIVATE KEY-----";
       
        public static log4net.ILog mainLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static  log4net.ILog buyingLog = log4net.LogManager.GetLogger("buyingLogger");
        public static log4net.ILog sellingLog = log4net.LogManager.GetLogger("sellingLogger");
        public static log4net.ILog cancelLog = log4net.LogManager.GetLogger("cancelLogger");
        public static int nonce=0;
     //   public static SqlConnection conn = new SqlConnection(windo);
        public int SendBuyRequest(int price, int commodity, int amount)
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.BuySellRequest();
            item.type = "buy";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
                mainLog.Debug("Buying requset sent to the server. information: " + item.ToString());
                string output = client.SendPostRequest< MarketItems.BuySellRequest> ("http://ise172.ise.bgu.ac.il", "user52",nonce,key, token, item);
                mainLog.Debug("Returned answer from the server after buying request: " + output);


                if (!(checkMarketResponse(output)))
                    throw new ApplicationException(output);
                int integerOutput;
                int.TryParse(output, out integerOutput);
            buyingLog.Info("Request for buying " + amount + " shares of commodity number " 
                            + commodity + " for " + price + " dollars per share has sent, " 
                            + " transaction id: " + output);
           
         
            return integerOutput;
        }
        public IQueryable<item> getBuyHistory()
        {
            HistoryDataContext buyHistory = new HistoryDataContext();
            byte b= Byte.Parse("52");
            IQueryable<item> histo = from item in buyHistory.items where item.buyer.Equals(b) select item;
            
            return histo;    
        }
        public IQueryable<item> getSellHistory()
        {
            HistoryDataContext sellHistory = new HistoryDataContext();
            byte b = Byte.Parse("52");
            IQueryable<item> histo = from item in sellHistory.items where item.seller.Equals(b) select item;
          
            return histo;
        }
        public IQueryable<item> getCancelHistory()
        {
            HistoryDataContext sellHistory = new HistoryDataContext();
            byte b = Byte.Parse("52");
            IQueryable<item> histo = from item in sellHistory.items where item.seller.Equals(b) select item;
            
            return histo;
        }
        public IQueryable<item> getBuyHistoryByDate(DateTime start, DateTime end)
        {
            HistoryDataContext buyHistory = new HistoryDataContext();
            byte b = Byte.Parse("52");
            IQueryable<item> histo = from item in buyHistory.items where item.buyer.Equals(b) & item.timestamp.Date.CompareTo(start)>=0  & item.timestamp.Date.CompareTo(end) <= 0  select item;
            return histo;
        }
        public IQueryable<item> getSellHistoryByDate(DateTime start, DateTime end)
        {
            HistoryDataContext sellHistory = new HistoryDataContext();
            byte b = Byte.Parse("52");
            IQueryable<item> histo = from item in sellHistory.items where item.seller.Equals(b) & item.timestamp.Date.CompareTo(start) >= 0 & item.timestamp.Date.CompareTo(end) <= 0 select item;
            return histo;
        }
        
        public int SendSellRequest(int price, int commodity, int amount) {

            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.BuySellRequest();
            item.type = "sell";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
            mainLog.Info("Selling requset sent to the server. information: " + item.ToString());
                string output = client.SendPostRequest< MarketItems.BuySellRequest> ("http://ise172.ise.bgu.ac.il", "user52", nonce, key, token, item);
                mainLog.Info("Returned answer from the server after selling request: " + output);
                if (!(checkMarketResponse(output)))
                    throw new ApplicationException(output);
                int integerOutput;
                int.TryParse(output, out integerOutput);
            sellingLog.Info("Request for selling " + amount + " shares of commodity number "
                               + commodity + " for " + price + " dollars per share has sent, "
                               + " transaction id: " + output);

            return integerOutput;
        }

        public IMarketItemQuery SendQueryBuySellRequest(int id)
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.QueryBuySellRequest();
            item.type = "queryBuySell";
            item.id = id;

            mainLog.Info("Sent Query BuySell request to the server. information: " + item.ToString());
            MarketItemQuery output = client.SendPostRequest<QueryBuySellRequest, MarketItemQuery> ("http://ise172.ise.bgu.ac.il", "user52",nonce,key, token, item);
            mainLog.Info("Returned answer from the server after Send Query BuySell Request: " + output);
            return output;
        }

        public IMarketUserData SendQueryUserRequest()
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.QueryUserRequest();
            item.type = "queryUser";
            mainLog.Info("Sent Query User request to the server. information: " + item.ToString());
            MarketUserData output = client.SendPostRequest<QueryUserRequest, MarketUserData> ("http://ise172.ise.bgu.ac.il", "user52",nonce,key, token, item);
            mainLog.Info("Returned answer from the server after sent Query User request: " + output);
            return output;
        }

        public IMarketCommodityOffer SendQueryMarketRequest(int commodity){

            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.QueryMarketRequest();
            item.type = "queryMarket";
            item.commodity = commodity;
            mainLog.Info("Sent Query Market request to the server. information: " + item.ToString());
            var output = client.SendPostRequest<QueryMarketRequest, MarketCommodityOffer> ("http://ise172.ise.bgu.ac.il", "user52",nonce,key, token, item);
            mainLog.Info("returned answer from the server after sent Query Market request: " + output);
            return output;
        }

        public bool SendCancelBuySellRequest(int id){

            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.CancelBuySellRequest();
            item.type = "cancelBuySell";
            item.id = id;
                mainLog.Info("Sent Cancel Buy Sell request to the server. information: " + item.ToString());
                var output = client.SendPostRequest<CancelBuySellRequest> ("http://ise172.ise.bgu.ac.il", "user52",nonce,key, token, item);
                mainLog.Info("Returned answer from the server after sent Cancel Buy Sell request: " + output);

            if (output == "Ok")
            {
                cancelLog.Info("Canceling request for transaction: " + id + " has sent");
               
                return true;
            }
            throw new Exception(output);
        }

        public bool cancelAllRequests()
        {
            int counter = 0;
       		mainLog.Info("Sent cancel All Requests request to the server.");
            MarketItems.MarketUserData userD = (MarketItems.MarketUserData) SendQueryUserRequest();
            bool allCanceled = true;
            foreach( int id in userD.requests)
            {
                if(counter==2)
                {
                    counter = 0;
                    System.Threading.Thread.Sleep(2000);
                }
                bool output = SendCancelBuySellRequest(id);
                counter++;
                if (!output)
                    allCanceled = false;
            }
            mainLog.Info("All Requests have been canceled.");
            return allCanceled;
        }

        public LinkedList<Commodities> SendQueryAllMarketRequest()
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.QueryMarketRequest();
            item.type = "queryAllMarket";
            mainLog.Info("Sent Query All Market request to the server. information: " + item.ToString());
            var output = client.SendPostRequest<QueryMarketRequest, LinkedList<Commodities>>("http://ise172.ise.bgu.ac.il", "user52", nonce, key, token, item);
            mainLog.Info("Returned answer from the server after sent Query Market request: " + output);
            return output;
        }

        public LinkedList<UserRequests> SendQueryUserRequestsRequest()
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            nonce++;
            string token = SimpleCtyptoLibrary.CreateToken("user52_" + nonce, key);
            var item = new MarketItems.QueryUserRequest();
            item.type = "queryUserRequests";
            mainLog.Info("Sent Query User Request to the server. information: " + item.ToString());
            var output = client.SendPostRequest<QueryUserRequest, LinkedList<UserRequests>>("http://ise172.ise.bgu.ac.il", "user52", nonce, key, token, item);
            mainLog.Info("Returned answer from the server after sent Query User request: " + output);
            return output;
        }

        // check the answere that the server give back. if it isnt number return false
        private static bool checkMarketResponse(string s1)
        {
            foreach (Char c in s1)
            {
                if (c < '0' | c > '9')
                    return false;
            }
            return true;
        }
       

    }

    public static class History
    {
        public static IQueryable<float> getLastHourCommodityHistoryOrderedByDate(int commodity)
        {
            HistoryDataContext dbContext = new HistoryDataContext();
            IQueryable<float> list = from item in dbContext.items
                                     where item.commodity == commodity
       & item.timestamp.Day == DateTime.Today.Day - 1
       & item.timestamp.Month == DateTime.Today.Month
                                     //& item.timestamp.Hour >= DateTime.Today.Hour
                                     orderby item.timestamp
                                     select item.price;
            
            return list;
        }

        public static float getTodaysRecommendedBuyPrice(int commodity)
        {
            HistoryDataContext dbContext = new HistoryDataContext();
            var list = from item in dbContext.items
                       where item.commodity == commodity
                       & item.timestamp.Day == DateTime.Today.Day
                       & item.timestamp.Month == DateTime.Today.Month
                       group item by item.price into tmp
                       orderby tmp.Key
                       select new
                       {
                           num = tmp.Count(),
                           price = tmp.Key
                       };
            int quarter = list.Count() / 5, i = 0, maxNum = 0;
            float bestPrice = 0;

            foreach (var tmp in list)
                if (i > quarter)
                    break;
                else if (tmp.num > maxNum)
                {
                    maxNum = tmp.num;
                    bestPrice = tmp.price;
                    i++;
                }

            return bestPrice;         
        }

        public static float getTodaysRecommendedSellPrice(int commodity)
        {
            HistoryDataContext dbContext = new HistoryDataContext();
            var list = from item in dbContext.items
                       where item.commodity == commodity
                       & item.timestamp.Day == DateTime.Today.Day
                       & item.timestamp.Month == DateTime.Today.Month
                       group item by item.price into tmp
                       orderby tmp.Key descending
                       select new
                       {
                           num = tmp.Count(),
                           price = tmp.Key
                       };
            int quarter = list.Count() / 5, i = 0, maxNum = 0;
            float bestPrice = 0;

            foreach (var tmp in list)
                if (i > quarter)
                    break;
                else if (tmp.num > maxNum)
                {
                    maxNum = tmp.num;
                    bestPrice = tmp.price;
                    i++;
                }

            return bestPrice;
        }
    }
}


