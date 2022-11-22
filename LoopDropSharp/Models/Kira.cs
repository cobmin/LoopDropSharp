using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Models
{
    public class MinterAndAmounts
    {
        public string walletAddress  { get; set; }
        public int totalMints  { get; set; }
    }

    public class Minter
    {
        public string tokenId { get; set; }
        public string walletAddress { get; set; }
    }
    public class MinterInformation
    {
        public int transaction_id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public string type { get; set; }
        public string token_id { get; set; }
        public string id { get; set; }
        public string token_address { get; set; }
        public string quantity { get; set; }
        public string quantity_with_fees { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class MinterInformations
    {
        public int order_id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public string sell { get; set; }
        public string token_address { get; set; }
        public int decimals { get; set; }
        public string symbol { get; set; }
        public string quantity { get; set; }
        public string quantity_with_fees { get; set; }
        public string buy { get; set; }
        public string token_id { get; set; }
        public string id { get; set; }
        public string token_addresss { get; set; }
        public string quantitys { get; set; }
        public string quantity_with_feess { get; set; }
        public string name { get; set; }
        public string image_url { get; set; }
        public string names { get; set; }
        public string icon_url { get; set; }
        public string amount_sold { get; set; }
        public DateTime expiration_timestamp { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime updated_timestamp { get; set; }
    }

    public class CollectionInformation
    {
        public int order_id { get; set; }
        public string status { get; set; }
        public string user { get; set; }
        public string sell { get; set; }
        public string token_id { get; set; }
        public DateTime expiration_timestamp { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime updated_timestamp { get; set; }
    }

    public class MinterAndCollectionInformation
    {
        public string status { get; set; }
        public string token_id { get; set; }
        public string user { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class MinterAndAmountsAndPass
    {
        public string walletAddress { get; set; }
        public int totalMints { get; set; }
        public string pass { get; set; }
    }
}

