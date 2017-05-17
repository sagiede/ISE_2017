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
            mc.cancelAllRequests();
            requestId = mc.SendBuyRequest(1, 2, 10);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(user.requests.Contains(requestId), true);
        }
        [Test]
        public void cancelRequestTest()
        {
            mc.SendCancelBuySellRequest(requestId);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(0, user.requests.Count);
        }
        [Test]
        public void sellRequestTest()
        {
            mc.cancelAllRequests();
            mc.SendSellRequest(5, 5, 1);
            requestId = mc.SendSellRequest(100, 5, 1);
            MarketUserData user = (MarketUserData)mc.SendQueryUserRequest();
            Assert.AreEqual(user.requests.Contains(requestId), true);
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
            SemiPilot.runSemiPilot(5, 10, 1, true);
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
                Assert.AreEqual(userDataAfter.funds > userDataBefore.funds, true);
            }
            
        }


    }

}