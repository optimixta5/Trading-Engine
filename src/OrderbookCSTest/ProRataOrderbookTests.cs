using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Fills;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace OrderbookCSTest
{
    [TestClass]
    public class ProRataOrderbookTests
    {
        [TestMethod]
        public void ProRataOrderbook_MatchTwoOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.ProRata);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 10, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);

            var (matchResult, obResult) = prorataOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 2);
            Assert.AreEqual(matchResult.Trades.Count, 1);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(askOrderOrderId), false);
        }

        [TestMethod]
        public void ProrataOrderbook_MatchTwoOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.ProRata);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);

            var (matchResult, obResult) = prorataOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 2);
            Assert.AreEqual(matchResult.Trades.Count, 1);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(buyOrderOrderId), true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(askOrderOrderId), false);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchThreeOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.ProRata);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            // Later buy order matched first because it is on the same price level as the previous and has a higher quantity
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 16, true); 

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            // Asks wiped out.
            var (matchResult, obResult) = prorataOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 4);
            Assert.AreEqual(matchResult.Trades.Count, 2);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(buyOrderOrderId), true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(askOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchThreeOrders_AskRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            const long askPrice = 10_000;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.ProRata);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 100, false); // Masive ask wall.
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 4);
            Assert.AreEqual(matchResult.Trades.Count, 2);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(askOrderOrderId), true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(spread.Ask.Value, askPrice);
            Assert.AreEqual(spread.Bid.HasValue, false);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchThreeOrders_SameQuantityTimeOrderPreserved()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.ProRata);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 16, false);
            // Two buy orders have the same quantity, relative ordering is preserved and first buyOrder is matched first.
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 4);
            Assert.AreEqual(matchResult.Trades.Count, 2);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(askOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(secondbuyOrderOrderId), true);
            Assert.AreEqual(spread.Bid.Value, bidPrice);
            Assert.AreEqual(spread.Ask.HasValue, false);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchFourOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.ProRata);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 10, false);
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(askOrder2);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 6);
            Assert.AreEqual(matchResult.Trades.Count, 3);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(askOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(secondAskOrderOrderId), false);
            Assert.AreEqual(spread.Ask.HasValue, false);
            Assert.AreEqual(spread.Bid.HasValue, false);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchFourOrders_FirstAskRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long thirdBuyOrderId = 4; // 
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.ProRata);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 10, false);
            // Second ask order has higher quantity that the total of the resting bids. It gets fully filled
            // and the first askOrder remains resting with a quantity of 1.
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 21, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder3 = new Order(new OrderCore(thirdBuyOrderId, string.Empty, 0), askPrice - 1, 1, true); // This order does not get matched.

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(askOrder2);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);
            prorataOrderbook.AddOrder(buyOrder3);

            var (matchResult, obResult) = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 6);
            Assert.AreEqual(matchResult.Trades.Count, 3);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(askOrderOrderId), true);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(secondAskOrderOrderId), false);
            Assert.AreEqual(prorataOrderbook.ContainsOrder(thirdBuyOrderId), true);
            Assert.AreEqual(spread.Ask.HasValue, true);
            Assert.AreEqual(spread.Bid.HasValue, true);
            Assert.AreEqual(spread.Spread.Value, 1);
        }
    }
}
