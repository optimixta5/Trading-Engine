using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public interface IOrderEntryOrderbook
    {
        OrderbookResult AddOrder(Order order);
        OrderbookResult ChangeOrder(ModifyOrder modifyOrder);
        OrderbookResult RemoveOrder(CancelOrder cancelOrder);
        OrderbookSpread GetSpread();
        bool ContainsOrder(long orderId);
        int Count { get; }
    }

    public interface IRetrievalOrderbook : IOrderEntryOrderbook
    {
        List<OrderbookEntry> GetAskOrders();
        List<OrderbookEntry> GetBuyOrders();
    }
}
