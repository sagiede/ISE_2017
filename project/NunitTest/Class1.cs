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

namespace NunitTest
{
    [TestFixture]
    public class Tests
    {

        MarketClientConnection mc = new MarketClientConnection();
        int requestId;

        [Test]
        public void buyRequestTest()
        {
            requestId = mc.SendBuyRequest(1, 2, 10);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(user.requests.Contains(requestId), true);
        }
        [Test]
        public void sellRequestTest()
        {
            mc.SendSellRequest(200, 5, 1);
            requestId = mc.SendSellRequest(100, 5, 1);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(user.requests.Contains(requestId), true);
        }

       [Test]
        public void cancelRequestTest()
        {
            requestId = mc.SendBuyRequest(1, 3, 10);
            mc.SendCancelBuySellRequest(requestId);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(user.requests.Contains(requestId), false);
        }
        [Test]
        public void cancelAllRequestTest()
        {
            mc.SendBuyRequest(1, 3, 10);
            mc.SendBuyRequest(1, 4, 10);
            mc.SendSellRequest(200, 5, 1);
            mc.cancelAllRequests();
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(user.requests.Count, 0);
        }
        [Test]
        public void test1SemiPilot()
        {

            SemiPilot.runSemiPilot(5, 20, 1, true);
            Assert.AreEqual(SemiPilot.semiPilotTimer.Enabled, true);
            SemiPilot.stopSemiPilot();
            Assert.AreEqual(SemiPilot.semiPilotTimer.Enabled, false);
        }
        
        [Test]
        public void test2SemiPilot()
        {
            MarketUserData userBefore = (MarketUserData)mc.SendQueryUserRequest();
            SemiPilot.runSemiPilot(5, 17, 1, true);
            while (SemiPilot.eventsData.Equals("")) { }
            MarketUserData userAfter = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreNotEqual(userBefore.ToString(), userAfter.ToString());
        }

        [Test]
        public void testAutoPilot()
        {
            MarketUserData userData = (MarketUserData)mc.SendQueryUserRequest();
            var comm = userData.commodities;
            double myFunds = userData.funds;


            AutoPilot.runPilot();
            while (AutoPilot.actions == "") { }

            userData = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreNotEqual(myFunds, userData.funds);
        }
        [Test]
        public void test2AutoPilot()
        {
            MarketUserData userDataBefore = (MarketUserData)mc.SendQueryUserRequest();

            AutoPilot.runPilot();
            while (AutoPilot.actions == "") { }
            AutoPilot.runPilot();           //stop auto pilot

            MarketUserData userDataAfter = (MarketUserData)mc.SendQueryUserRequest();
            if (userDataAfter.commodities[AutoPilot.lastCommodity + ""] > userDataBefore.commodities[AutoPilot.lastCommodity + ""])    //we bought and havent sell the item
            {
                Assert.AreEqual(userDataAfter.requests.Count > userDataBefore.requests.Count, true);
            }
            else        //we managed to sell the stock when we bought it - then check we earned money
            {
                Assert.AreEqual(true , userDataAfter.funds > userDataBefore.funds);
            }
            
        }

    }

}
