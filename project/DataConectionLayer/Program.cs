﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketClient;
using MarketClient.Utils;
using MarketClient.DataEntries;


namespace LogicLayer
{
    public abstract class Request                               // הפכתי לאבסטרקט
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
    public class queryBuySellRequest : Request                     
    {
        public int id { get; set; }
    }
    public class queryUserRequest : Request { }
   
    public class MarketItemQuery : IMarketItemQuery 
    {
        public object user { get; set; }
        public string type { get; set; }
        public int commodity { get; set; }
        public int amount { get; set; }
        public int price { get; set; }

    }

    public class MarketUserData : IMarketUserData
    {
        public Dictionary <string, int> commodities { get; set; }
        public int funds { get; set; }
        public List<int> requests { get; set; }                               

    }
    class PipeConection : IMarketClient
    {
      public static int SendBuyRequest(int price, int commodity, int amount)
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string key= @"-----BEGIN RSA PRIVATE KEY-----
                        MIICXgIBAAKBgQDTBpVK3vFeDFOW6bGIGg3Uu7Gv6sYhLqhxAVwhyV87huH2cCg5
                        ZdWdHfgPB5XlwdPQuAJhgM2KtldAtzBUzUUMHj6uvSrUQ6ovPMYPmUq0CraKlBfr
                        AgTqBkzOZUL0m72acwZThDtUxWWJ9tfgej758Gx23IwQMcWdjfmi1vMkXwIDAQAB
                        AoGBAJ6H6YwPpGA0 / m73LQnEGPPh9YDk7Odst9n9XYt8TnDXydVrOKy2Fh4sr3gL
                        CM9MJ6Y7Nn6tVryIc1AIzuRGOUGdz / IpxsTLFvgTSqMXUazVy39hoUUBGloWhUQ7
                        w + Uicbixt5v4SGTEi69 + EBFMPplSrV1k5eKc4Q / 6n5fuPWTBAkEA32g + 9wyEDX3A
                        WpVXuD7IDsr / LQHQ4dckoxUF9hzo2EAXT2QyQH8ELpmFCWj0Jy2iGnfzH / e0O90c
                        N5DydVerLwJBAPHP6pdeXsLQry5h5Qh8uO / C9dV2GEh4Frn7lnhx53 / b6 / echr3f
                        qsGzwee58ngHB3vnY + nR / 57 + Dvz / ceyBDdECQQCADCUCvpa1kNz + TljPzpQl / m3R
                        oxfRSdnC61rWXG2M / PcfVwOCegqwludspE5EYmBmIVgle3k / UpVIt / hwD0abAkB1
                        7irA69tXM6NcAY5Ll1gyRmjSVCf / n + GljpeR4is + 5iist//WtjB3C43zz3H7K6Jw
                        wSavMCV0iv8QUBxldYMhAkEAkYx4UBaYXMr / byar4UYkdTYuxag + iXFifBSqIYY4
                        sybKv1Ahjdz9bcvIYbauBzJPjL7n1u68fGPXcaKYDzjo3w ==
                        -----END RSA PRIVATE KEY---- - ";

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

            SimpleHTTPClient client = new SimpleHTTPClient();
            string key = "123";
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new queryBuySellRequest;
            item.type = "queryBuySell";
            item.id = id;
            item.authentication = new Dictionary<string, string>() { { "token", token }, { "user", "user52" } };

            MarketItemQuery output = client.SendPostRequest<queryBuySellRequest,MarketItemQuery>("http://ise172.ise.bgu.ac.il", "user52", token, item);

            return output;
        }
        public IMarketUserData SendQueryUserRequest()
        {
            SimpleHTTPClient client = new SimpleHTTPClient();
            string key = "123";
            string token = SimpleCtyptoLibrary.CreateToken("user52", key);
            var item = new queryUserRequest();
            item.type = "queryUser";
            item.authentication = new Dictionary<string, string>() { { "token", token }, { "user", "user52" } };

            MarketUserData output = client.SendPostRequest<queryUserRequest, MarketUserData>("http://ise172.ise.bgu.ac.il", "user52", token, item);

            return output;

        }
        public IMarketCommodityOffer SendQueryMarketRequest(int commodity){ }
        public bool SendCancelBuySellRequest(int id){ }
    }
}
