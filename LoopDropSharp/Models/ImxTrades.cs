using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Models
{
    public class A
    {
        public int order_id { get; set; }
        public string token_type { get; set; }
        public string sold { get; set; }

    }
    public class B
    {
        public int order_id { get; set; }
        public string token_type { get; set; }
        public string token_id { get; set; }
        public string token_address { get; set; }
        public string sold { get; set; }

    }
    public class TradeResult
    {
        public int transaction_id { get; set; }
        public string status { get; set; }
        public A a { get; set; }
        public B b { get; set; }
        public DateTime timestamp { get; set; }

    }
    public class TradeResults
    {
        public List<TradeResult> result { get; set; }
        public string cursor { get; set; }
        public int remaining { get; set; }

    }
}
