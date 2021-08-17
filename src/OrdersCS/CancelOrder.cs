namespace TradingEngineServer.Orders
{
    public class CancelOrder : IOrderCore
    {
        public CancelOrder(IOrderCore orderBase)
        {
            _orderBase = orderBase;
        }

        public long OrderId => _orderBase.OrderId;
        public string Username => _orderBase.Username;
        public int SecurityId => _orderBase.SecurityId;

        private readonly IOrderCore _orderBase;
    }
}