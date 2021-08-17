using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orderbook.MatchingAlgorithm
{
    public interface IMatchingAlgorithm
    {
        MatchResult Match(IEnumerable<OrderbookEntry> bids, IEnumerable<OrderbookEntry> asks);
    }
}
