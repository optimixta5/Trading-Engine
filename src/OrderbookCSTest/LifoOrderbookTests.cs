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
    public class LifoOrderbookTests
    {
        private static readonly IOrderCore _testOrderBase = new OrderCore(0, "Test Username", 0);

        [TestMethod]
        public void LifoOrderbook_MatchTwoOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook LifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Lifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 10, true);

            LifoOrderbook.AddOrder(askOrder);
            LifoOrderbook.AddOrder(buyOrder);

            var (matchResult, obResult) = LifoOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 2);
            Assert.AreEqual(matchResult.Trades.Count, 1);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(askOrderOrderId), false);
        }

        [TestMethod]
        public void LifoOrderbook_MatchTwoOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook LifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Lifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            LifoOrderbook.AddOrder(askOrder);
            LifoOrderbook.AddOrder(buyOrder);

            var (matchResult, obResult) = LifoOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 2);
            Assert.AreEqual(matchResult.Trades.Count, 1);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(buyOrderOrderId), true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(askOrderOrderId), false);
        }

        [TestMethod]
        public void LifoOrderbook_MatchThreeOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            MatchingOrderbook LifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Lifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 20, false); // Sell side hits 2 bids
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            LifoOrderbook.AddOrder(askOrder);
            LifoOrderbook.AddOrder(buyOrder);
            LifoOrderbook.AddOrder(buyOrder2);

            // Asks wiped out.
            var (matchResult, obResult) = LifoOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 4);
            Assert.AreEqual(matchResult.Trades.Count, 2);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(buyOrderOrderId), true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(askOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
        }

        [TestMethod]
        public void LifoOrderbook_MatchThreeOrders_AskRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            const long askPrice = 10_000;
            MatchingOrderbook LifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Lifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 100, false); // Masive ask wall.
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            LifoOrderbook.AddOrder(askOrder);
            LifoOrderbook.AddOrder(buyOrder);
            LifoOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = LifoOrderbook.Match();
            var spread = LifoOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 4);
            Assert.AreEqual(matchResult.Trades.Count, 2);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(askOrderOrderId), true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(spread.Ask.Value, askPrice);
            Assert.AreEqual(spread.Bid.HasValue, false);
        }

        [TestMethod]
        public void LifoOrderbook_MatchFourOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook LifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Lifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 10, false);
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            LifoOrderbook.AddOrder(askOrder);
            LifoOrderbook.AddOrder(askOrder2);
            LifoOrderbook.AddOrder(buyOrder);
            LifoOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = LifoOrderbook.Match();
            var spread = LifoOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 6);
            Assert.AreEqual(matchResult.Trades.Count, 3);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(askOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(secondAskOrderOrderId), false);
            Assert.AreEqual(spread.Ask.HasValue, false);
            Assert.AreEqual(spread.Bid.HasValue, false);
        }

        [TestMethod]
        public void LifoOrderbook_MatchFourOrders_RestingAskIsEarliestAsk()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook LifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Lifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 15, false);
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            LifoOrderbook.AddOrder(askOrder);
            LifoOrderbook.AddOrder(askOrder2);
            LifoOrderbook.AddOrder(buyOrder);
            LifoOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = LifoOrderbook.Match();
            var spread = LifoOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 6);
            Assert.AreEqual(matchResult.Trades.Count, 3);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(askOrderOrderId), true);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(LifoOrderbook.ContainsOrder(secondAskOrderOrderId), false);
            Assert.AreEqual(spread.Ask.HasValue, true);
            Assert.AreEqual(spread.Bid.HasValue, false);
        }
    }
}
