using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Fills
{
    public enum FillAllocationAlgorithm
    {
        Unknown,
        Fifo,
        Lifo,
        ProRata,
    }
}
