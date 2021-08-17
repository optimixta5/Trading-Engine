using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TradingEngineServer.Fills;
using TradingEngineServer.Trades;

namespace TradingEngineServer.Orderbook
{
    public class MatchResult
    {
        public MatchResult()
        { }

        public void AddTradeResult(TradeResult tradeResult)
        {
            AddTrade(tradeResult.Trade);
            AddFill(tradeResult.BuyFill);
            AddFill(tradeResult.SellFill);
        }

        public void AddTrade(Trade trade)
        {
            _trades.Add(trade);
        }

        public void AddFill(Fill fill)
        {
            _fills.Add(fill);
        }

        public IReadOnlyList<Trade> Trades
        {
            get
            {
                return new ReadOnlyCollection<Trade>(_trades);
            }
        }

        public IReadOnlyList<Fill> Fills
        {
            get
            {
                return new ReadOnlyCollection<Fill>(_fills);
            }
        }

        private readonly List<Fill> _fills = new List<Fill>();
        private readonly List<Trade> _trades = new List<Trade>();
    }
}
