using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketClient;
using MarketClient.Utils;
using MarketClient.DataEntries;

using System.IO;


namespace LogicLayer
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
        override
        public string ToString()
        {
            string str =" type: "+type+ " commodity: " + commodity+ " amount: " + amount + " price: " + price;
            return str;
        }
        
    }
    public class QueryBuySellRequest : Request                     
    {
        public int id { get; set; }
        override
        public string ToString()
        {
            string str = "type: " + type + "id: " + id;
            return str;
        }
    }
    public class QueryUserRequest : Request {
        override
        public string ToString()
        {
            string str = "type: " + type;
            return str;
        }
    }
    public class QueryMarketRequest : Request {
        public int commodity { get; set; }
        override
        public string ToString()
        {
            string str = "type: " + type+ "commodity: "+commodity;
            return str;
        }
    }
    public class CancelBuySellRequest : Request
    {
        public int id { get; set; }
        override
        public string ToString()
        {
            string str = "type: " + type + "id: " + id;
            return str;
        }
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
    
    public class MarketUserData : IMarketUserData{
        public Dictionary <string, int> commodities { get; set; }
        public double funds { get; set; }
        public List<int> requests { get; set; }
        override
        public string ToString()
        {
            string commTostring = "";
            string reqTostring = " ";
            foreach(var tmp in commodities)
            {
                commTostring += " stock " + tmp.Key.ToString()  + ":" + tmp.Value.ToString();
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

    public class MarketCommodityOffer : IMarketCommodityOffer{
       public int ask { get; set; }
       public int bid { get; set; }

        override
        public string ToString()
        {
            return "\nMarket status: \n" + "ask: " + ask +  "\nbid: " + bid;
        }

    }
     
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

       
        private static log4net.ILog mainLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int SendBuyRequest(int price, int commodity, int amount)
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
         
            
          
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            BuySellRequest item = new BuySellRequest();
            item.type = "buy";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
            try
            {
                mainLog.Info("buying requset send to the server. information: " + item.ToString());
                string output = client.SendPostRequest<BuySellRequest>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                mainLog.Info("return answere from the server after buing request: " + output);


                if (!(checkMarketResponse(output)))
                {
                  
                    Console.WriteLine(output);
                    return -1;
                }
                int integerOutput;
                int.TryParse(output, out integerOutput);
                return integerOutput;
            }
            catch (Exception e)
            {
                mainLog.Error("the answere of the server has problem after buing request" + e.Message);
                Console.WriteLine(e.Message);
            }
            return -1;
        }

        public int SendSellRequest(int price, int commodity, int amount) {

            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new BuySellRequest();
            item.type = "sell";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
            try
            {
                mainLog.Info("selling requset send to the server. information: " + item.ToString());
                string output = client.SendPostRequest<BuySellRequest>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                mainLog.Info("return answere from the server after selling request: " + output);
                if (!(checkMarketResponse(output)))
                {
                    Console.WriteLine(output);
                    return -1;
                }
                int integerOutput;
                int.TryParse(output, out integerOutput);
                return integerOutput;
            }
            catch(Exception e)
            {
                mainLog.Error("the answere of the server has problem after selling request" + e.Message);
                Console.WriteLine(e.Message);
            }
            return -1;
        }

        public IMarketItemQuery SendQueryBuySellRequest(int id)
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new QueryBuySellRequest();
            item.type = "queryBuySell";
            item.id = id;

            try
            {
                mainLog.Info("Send Query BuySell Request to the server. information: " + item.ToString());
                MarketItemQuery output = client.SendPostRequest<QueryBuySellRequest, MarketItemQuery>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                mainLog.Info("return answere from the server after Send Query BuySell Request: " + output);
                return output;
            }
            
            catch (Exception e)
            {
                mainLog.Error("the answere of the server has problem after Send Query BuySell Request" + e.Message);
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public IMarketUserData SendQueryUserRequest()
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new QueryUserRequest();
            item.type = "queryUser";
            try
            {
                mainLog.Info("Send Query User Request to the server. information: " + item.ToString());
                MarketUserData output = client.SendPostRequest<QueryUserRequest, MarketUserData>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                mainLog.Info("return answere from the server after Send Query User Request: " + output);
                return output;
            }
          catch(Exception e)
            {
                mainLog.Error("the answere of the server has problem after Send Query User Request" + e.Message);
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public IMarketCommodityOffer SendQueryMarketRequest(int commodity){

            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new QueryMarketRequest();
            item.type = "queryMarket";
            item.commodity = commodity;
            try
            {
                mainLog.Info("Send Query Market Request to the server. information: " + item.ToString());
                var output = client.SendPostRequest<QueryMarketRequest, MarketCommodityOffer>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                mainLog.Info("return answere from the server after Send Query Market Request: " + output);
                return output;
            }
            catch(Exception e)
            {
                mainLog.Error("the answere of the server has problem after Send Query User Request" + e.Message);
                Console.WriteLine(e.Message);
            }
            
            return null;
        }

        public bool SendCancelBuySellRequest(int id){

            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new CancelBuySellRequest();
            item.type = "cancelBuySell";
            item.id = id;
            try
            {
                mainLog.Info("Send Cancel Buy Sell Request to the server. information: " + item.ToString());
                var output = client.SendPostRequest<CancelBuySellRequest>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                mainLog.Info("return answere from the server afterSend Cancel Buy Sell Request: " + output);
                if (output == "Ok")
                    return true;
                Console.WriteLine("transaction id num:" + id+ ", could not canceled correctly beacuse of the following reason: " + output);
            }
            catch(Exception e)
            {
                mainLog.Error("the answere of the server has problem after Cancel Buy Sell Request" + e.Message);
                Console.WriteLine(e.Message);
            }
            return false;
           
        }
        public bool cancelAllRequests()
        {
            mainLog.Info("Send cancel All Requests to the server.");
            MarketUserData userD = (MarketUserData) SendQueryUserRequest();
            bool allCanceled = true;
            foreach( int id in userD.requests)
            {
                bool output = SendCancelBuySellRequest(id);
                
                if (!output)
                    allCanceled = false;

            }
            mainLog.Info("All Requests canceld.");
            return allCanceled;

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

}
