using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders.OrderStatuses;

namespace TradingEngineServer.Orders
{
    public sealed class OrderStatusCreator
    {
        public static CancelOrderStatus GenerateOrderCancelStatus(CancelOrder obe)
        {
            return new CancelOrderStatus
            {

            };
        }
        public static NewOrderStatus GenerateNewOrderStatus(Order obe)
        {
            return new NewOrderStatus
            {

            };
        }
        public static ModifyOrderStatus GenerateModifyOrderStatus(ModifyOrder obe)
        {
            return new ModifyOrderStatus
            {

            };
        }
    }
}
