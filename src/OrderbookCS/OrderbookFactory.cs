using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Fills;
using TradingEngineServer.Instrument;

namespace TradingEngineServer.Orderbook
{
    public class OrderbookFactory
    {
        public static MatchingOrderbook CreateOrderbook(IRetrievalOrderbook ob, FillAllocationAlgorithm fillAllocationAlgorithm)
        {
            return fillAllocationAlgorithm switch
            {
                FillAllocationAlgorithm.Fifo => new FifoOrderbook(ob),
                FillAllocationAlgorithm.Lifo => new LifoOrderbook(ob),
                FillAllocationAlgorithm.ProRata => new ProRataOrderbook(ob),
                _ => throw new InvalidOperationException($"Unknown FillAllocationAlgorithm ({fillAllocationAlgorithm})"),
            };
        }

        public static MatchingOrderbook CreateOrderbook(Security inst, FillAllocationAlgorithm fillAllocationAlgorithm)
        {
            var retrievalOrderbook = new Orderbook(inst);
            return CreateOrderbook(retrievalOrderbook, fillAllocationAlgorithm);
        }
    }
}
