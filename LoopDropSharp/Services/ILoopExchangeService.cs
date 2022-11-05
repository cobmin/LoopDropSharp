using LoopDropSharp;
using LoopDropSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    public interface ILoopExchangeService
    {
        Task<LoopExchange> GetLoopPhunksData();
        
    }
}
