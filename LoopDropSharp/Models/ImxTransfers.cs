using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Models
{
    public class ImxData
    {
        public string token_id { get; set; }
        public string id { get; set; }
        public string token_address { get; set; }
        public int decimals { get; set; }
        public string quantity { get; set; }
        public string quantity_with_fees { get; set; }
    }

    public class TransferResult
    {
        public int transaction_id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public string receiver { get; set; }
        public Token token { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class ImxTransfer
    {
        public List<TransferResult> result { get; set; }
        public string cursor { get; set; }
        public int remaining { get; set; }
    }

    public class ImxToken
    {
        public string type { get; set; }
        public ImxData data { get; set; }
    }

}
