using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orderbook
{
    public class Limit
    {
        public long Price { get; set; }
        public OrderbookEntry Head { get; set; }
        public OrderbookEntry Tail { get; set; }
    }
}
