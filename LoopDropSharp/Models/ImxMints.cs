using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Models
{
    public class Data
    {
        public string token_id { get; set; }
        public string id { get; set; }
        public string token_address { get; set; }
        public string quantity { get; set; }
        public string quantity_with_fees { get; set; }
    }

    public class MintResult
    {
        public int transaction_id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public IMXToken token { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class ImxMints
    {
        public List<MintResult> result { get; set; }
        public string cursor { get; set; }
        public int remaining { get; set; }
    }

    public class IMXToken
    {
        public string type { get; set; }
        public Data data { get; set; }
    }


}
