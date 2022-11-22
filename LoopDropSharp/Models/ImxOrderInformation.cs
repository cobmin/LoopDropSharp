using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Models
{
    public class TradeSellData
    {
        public string token_address { get; set; }
        public int decimals { get; set; }
        public string symbol { get; set; }
        public string quantity { get; set; }
        public string quantity_with_fees { get; set; }

    }
    public class Sell
    {
        public string type { get; set; }
        public TradeSellData data { get; set; }

    }
    public class TradeCollection
    {
        public string name { get; set; }
        public string icon_url { get; set; }

    }
    public class Properties
    {
        public string name { get; set; }
        public string image_url { get; set; }
        public TradeCollection collection { get; set; }

    }
    public class TradeBuyData
    {
        public string token_id { get; set; }
        public string id { get; set; }
        public string token_address { get; set; }
        public string quantity { get; set; }
        public string quantity_with_fees { get; set; }
        public Properties properties { get; set; }

    }
    public class Buy
    {
        public string type { get; set; }
        public TradeBuyData data { get; set; }

    }
    public class OrderInformation
    {
        public int order_id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public Sell sell { get; set; }
        public Buy buy { get; set; }
        public string amount_sold { get; set; }
        public DateTime expiration_timestamp { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime updated_timestamp { get; set; }

    }









    public class BuyIMX
    {
        public string type { get; set; }
        public DataIMX data { get; set; }
    }

    public class CollectionIMX
    {
        public string name { get; set; }
        public string icon_url { get; set; }
    }

    public class DataIMX
    {
        public string token_id { get; set; }
        public string id { get; set; }
        public string token_address { get; set; }
        public string quantity { get; set; }
        public string quantity_with_fees { get; set; }
        public PropertiesIMX properties { get; set; }
        public int decimals { get; set; }
    }

    public class PropertiesIMX
    {
        public string name { get; set; }
        public string image_url { get; set; }
        public CollectionIMX collection { get; set; }
    }

    public class ResultIMX
    {
        public int order_id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public SellIMX sell { get; set; }
        public BuyIMX buy { get; set; }
        public string amount_sold { get; set; }
        public DateTime expiration_timestamp { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime updated_timestamp { get; set; }
    }

    public class RootIMX
    {
        public List<ResultIMX> result { get; set; }
        public string cursor { get; set; }
        public int remaining { get; set; }
    }

    public class SellIMX
    {
        public string type { get; set; }
        public DataIMX data { get; set; }
    }


}
