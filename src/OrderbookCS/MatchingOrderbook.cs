using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingEngineServer.Orderbook.MatchingAlgorithm;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public class MatchingOrderbook : IMatchingOrderbook
    {
        private readonly IMatchingAlgorithm _matchingAlgorithm;
        private readonly IRetrievalOrderbook _orderbook;
        private readonly object _lock = new object();

        public MatchingOrderbook(IRetrievalOrderbook orderbook, IMatchingAlgorithm matchingAlgorithm)
        {
            _orderbook = orderbook;
            _matchingAlgorithm = matchingAlgorithm;
        }

        public int Count
        {
            get
            {
                lock (_lock)
                    return _orderbook.Count;
            }
        }

        public OrderbookResult AddOrder(Order order)
        {
            lock (_lock)
                return _orderbook.AddOrder(order);
        }

        public OrderbookResult ChangeOrder(ModifyOrder modifyOrder)
        {
            lock (_lock)
                return _orderbook.ChangeOrder(modifyOrder);
        }

        public bool ContainsOrder(long orderId)
        {
            lock (_lock)
                return _orderbook.ContainsOrder(orderId);
        }

        public OrderbookSpread GetSpread()
        {
            lock (_lock)
                return _orderbook.GetSpread();
        }

        public OrderbookResult RemoveOrder(CancelOrder cancelOrder)
        {
            lock (_lock)
                return _orderbook.RemoveOrder(cancelOrder);
        }

        public (MatchResult MatchResult, OrderbookResult OrderbookResult) Match()
        {
            OrderbookResult obr = new OrderbookResult();
            lock (_lock)
            {
                var bids = _orderbook.GetBuyOrders();
                var asks = _orderbook.GetAskOrders();
                var matchResult = _matchingAlgorithm.Match(bids, asks);

                // Remove all fully filled orders from the book.
                var fullyFilledOrders = matchResult.Fills.Where(f => f.IsCompleteFill);
                foreach (var fullyFilledOrder in fullyFilledOrders)
                    obr.Merge(_orderbook.RemoveOrder(new CancelOrder(fullyFilledOrder.OrderBase)));

                return (matchResult, obr);
            }
        }
    }
}
