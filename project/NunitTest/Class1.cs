using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using LogicLayer;
using Pilots;
using MarketClient;
using MarketItems;
using System.IO;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace NunitTest
{
    [TestFixture]
    public class Tests
    {

        MarketClientConnection mc = new MarketClientConnection();
        int requestId;

       /* [Test]
        public void buyRequestTest()
        {
            requestId = mc.SendBuyRequest(1, 2, 10);            //send buy request (with low price)
            System.Threading.Thread.Sleep(2000);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(true, user.requests.Contains(requestId));       //check that the request had sended
        }
        [Test]
        public void sellRequestTest()
        {
            requestId = mc.SendSellRequest(100, 5, 1);      //send buy request  (with high price)
            System.Threading.Thread.Sleep(2000);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(true, user.requests.Contains(requestId));       //check that the request had sended
        }

        [Test]
        public void cancelRequestTest()
        {
            requestId = mc.SendBuyRequest(1, 3, 10);
            mc.SendCancelBuySellRequest(requestId);
            System.Threading.Thread.Sleep(2000);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(false, user.requests.Contains(requestId));      //check the request had canceled
        }
        [Test]
        public void cancelAllRequestTest()
        {
            mc.SendBuyRequest(1, 3, 10);
            mc.SendBuyRequest(1, 4, 10);
            mc.SendSellRequest(200, 5, 1);
            mc.cancelAllRequests();
            System.Threading.Thread.Sleep(2000);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(user.requests.Count, 0);        //check all requests had canceled
        }
        //test that the semi pilot begins and stops when telling him to
        [Test]
        public void test1SemiPilot()
        {
            SemiPilot.runSemiPilot(5, 20, 1, true);
            Assert.AreEqual(true, SemiPilot.semiPilotTimer.Enabled);
            SemiPilot.stopSemiPilot();
            Assert.AreEqual(false, SemiPilot.semiPilotTimer.Enabled);
        }
        //run semi pilot and check that he bought/sell when he told he did
        [Test]
        public void test2SemiPilot()
        {
            MarketUserData userBefore = (MarketUserData)mc.SendQueryUserRequest();
            SemiPilot.runSemiPilot(5, 17, 1, true);
            while (SemiPilot.eventsData.Equals("")) { }
            System.Threading.Thread.Sleep(2000);
            MarketUserData userAfter = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreNotEqual(userBefore.ToString(), userAfter.ToString());
            //if the user data diffrenet - its either there is more requests waiting (buy/sell) or the funds/commodity is different
        }
        //run auto pilot and after making a purchase check that he funds changed
        //either we bought and the funds lower/we bought and sold and earned money
        [Test]
        public void testAutoPilot()
        {
            MarketUserData userData = (MarketUserData)mc.SendQueryUserRequest();
            var comm = userData.commodities;
            double myFunds = userData.funds;

            AutoPilot.runPilot();
            while (AutoPilot.actions == "") { }
            System.Threading.Thread.Sleep(2000);
            userData = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreNotEqual(myFunds, userData.funds);
        }
        */
        [Test]
        public void testBuyHistoryByDate()
        {
            DateTime start = new DateTime(2017, 06, 10);
            DateTime end = new DateTime(2017, 06, 11);
            LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
            IQueryable<LogicLayer.item> i1 = mc1.getBuyHistoryByDate(start, end);
            int x = i1.Count();
            Assert.AreEqual(x, 2452);
        }
        [Test]
        public void testSellHistoryByDate()
        {
            DateTime start = new DateTime(2017, 06, 10);
            DateTime end = new DateTime(2017, 06, 11);
            LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
            IQueryable<LogicLayer.item> i1 = mc1.getSellHistoryByDate(start, end);
            int x = i1.Count();
            Assert.AreEqual(x, 3253);
            
        }
        [Test]
        public void testBuyHistory()
        { 
            LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
            IQueryable<LogicLayer.item> i1 = mc1.getBuyHistory();
            int x = i1.Count();
            Boolean check = x > 10;
            Assert.AreEqual(true, check);
        }
        [Test]
        public void testSellBuyHistory()
        {
            LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
            IQueryable<LogicLayer.item> i1 = mc1.getSellHistory();
            int x = i1.Count();
            Boolean check = x > 10;
            Assert.AreEqual(true, check);

        }
    }
}
