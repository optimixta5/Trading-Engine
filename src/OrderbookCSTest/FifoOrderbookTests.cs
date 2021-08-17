using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingEngineServer.Fills;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace OrderbookCSTest
{
    [TestClass]
    public class FifoOrderbookTests
    {
        private static readonly IOrderCore _testOrderBase = new OrderCore(0, "Test Username", 0);

        [TestMethod]
        public void FifoOrderbook_MatchTwoOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Fifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 10, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);

            var (matchResult, obResult) = fifoOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 2);
            Assert.AreEqual(matchResult.Trades.Count, 1);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(askOrderOrderId), false);
        }

        [TestMethod]
        public void FifoOrderbook_MatchTwoOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Fifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);

            var (matchResult, obResult) = fifoOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 2);
            Assert.AreEqual(matchResult.Trades.Count, 1);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(buyOrderOrderId), true);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(askOrderOrderId), false);
        }

        [TestMethod]
        public void FifoOrderbook_MatchThreeOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Fifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 20, false); // Sell side hits 2 bids
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);
            fifoOrderbook.AddOrder(buyOrder2);

            // Asks wiped out.
            var (matchResult, obResult) = fifoOrderbook.Match();

            Assert.AreEqual(matchResult.Fills.Count, 4);
            Assert.AreEqual(matchResult.Trades.Count, 2);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(askOrderOrderId), false);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(secondbuyOrderOrderId), true);
        }

        [TestMethod]
        public void FifoOrderbook_MatchThreeOrders_AskRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            const long askPrice = 10_000;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Fifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 100, false); // Masive ask wall.
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);
            fifoOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = fifoOrderbook.Match();
            var spread = fifoOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 4);
            Assert.AreEqual(matchResult.Trades.Count, 2);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(askOrderOrderId), true);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(spread.Ask.Value, askPrice);
            Assert.AreEqual(spread.Bid.HasValue, false);
        }

        [TestMethod]
        public void FifoOrderbook_MatchFourOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(null as Security, FillAllocationAlgorithm.Fifo);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 10, false);
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(askOrder2);
            fifoOrderbook.AddOrder(buyOrder);
            fifoOrderbook.AddOrder(buyOrder2);

            var (matchResult, obResult) = fifoOrderbook.Match();
            var spread = fifoOrderbook.GetSpread();

            Assert.AreEqual(matchResult.Fills.Count, 6);
            Assert.AreEqual(matchResult.Trades.Count, 3);
            Assert.AreEqual(obResult.HasCancelOrderStatus, true);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(buyOrderOrderId), false);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(askOrderOrderId), false);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(secondbuyOrderOrderId), false);
            Assert.AreEqual(fifoOrderbook.ContainsOrder(secondAskOrderOrderId), false);
            Assert.AreEqual(spread.Ask.HasValue, false);
            Assert.AreEqual(spread.Bid.HasValue, false);
        }
    }
}
