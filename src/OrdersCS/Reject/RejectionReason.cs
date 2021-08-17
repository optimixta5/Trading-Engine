using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Rejects
{
    public enum RejectionReason
    {
        Unknown,
        OrderNotFound,
        InstrumentNotFound,
        AttemptingToModifyWrongSide,
    }
}
