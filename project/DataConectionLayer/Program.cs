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
            var item = new MarketItems.BuySellRequest();
            item.type = "buy";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
                mainLog.Debug("buying requset send to the server. information: " + item.ToString());
                string output = client.SendPostRequest< MarketItems.BuySellRequest> ("http://ise172.ise.bgu.ac.il", "user52", token, item);
                mainLog.Debug("return answere from the server after buing request: " + output);


                if (!(checkMarketResponse(output)))
                    throw new ApplicationException(output);
                int integerOutput;
                int.TryParse(output, out integerOutput);
                return integerOutput;
        }

        public int SendSellRequest(int price, int commodity, int amount) {

            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new MarketItems.BuySellRequest();
            item.type = "sell";
            item.price = price;
            item.commodity = commodity;
            item.amount = amount;
                string output = client.SendPostRequest< MarketItems.BuySellRequest> ("http://ise172.ise.bgu.ac.il", "user52", token, item);
                if (!(checkMarketResponse(output)))
                    throw new ApplicationException(output);
                int integerOutput;
                int.TryParse(output, out integerOutput);
                return integerOutput;

        }

        public IMarketItemQuery SendQueryBuySellRequest(int id)
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new MarketItems.QueryBuySellRequest();
            item.type = "queryBuySell";
            item.id = id;

                MarketItems.MarketItemQuery output = client.SendPostRequest<MarketItems.QueryBuySellRequest, MarketItems.MarketItemQuery>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                return output;
        }

        public IMarketUserData SendQueryUserRequest()
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new MarketItems.QueryUserRequest();
            item.type = "queryUser";
                MarketItems.MarketUserData output = client.SendPostRequest<MarketItems.QueryUserRequest, MarketItems.MarketUserData>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                return output;
        }

        public IMarketCommodityOffer SendQueryMarketRequest(int commodity){

            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new MarketItems.QueryMarketRequest();
            item.type = "queryMarket";
            item.commodity = commodity;
                var output = client.SendPostRequest<MarketItems.QueryMarketRequest, MarketItems.MarketCommodityOffer>("http://ise172.ise.bgu.ac.il", "user52", token, item);
                return output;
        }

        public bool SendCancelBuySellRequest(int id){

            SimpleHTTPClient client = new SimpleHTTPClient();
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new MarketItems.CancelBuySellRequest();
            item.type = "cancelBuySell";
            item.id = id;
                var output = client.SendPostRequest<MarketItems.CancelBuySellRequest>("http://ise172.ise.bgu.ac.il", "user52", token, item);

                if (output == "Ok")
                    return true;
                throw new Exception(output); 
        }
        public bool cancelAllRequests()
        {
            MarketItems.MarketUserData userD = (MarketItems.MarketUserData) SendQueryUserRequest();
            bool allCanceled = true;
            foreach( int id in userD.requests)
            {
                bool output = SendCancelBuySellRequest(id);
                if (!output)
                    allCanceled = false;
            }
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
