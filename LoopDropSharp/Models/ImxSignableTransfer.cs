using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Models
{
    public class SignableTransfer
    {
        public string amount { get; set; }
        public string asset_id { get; set; }
        public int expiration_timestamp { get; set; }
        public int nonce { get; set; }
        public string payload_hash { get; set; }
        public string receiver_stark_key { get; set; }
        public int receiver_vault_id { get; set; }
        public string sender_stark_key { get; set; }
        public int sender_vault_id { get; set; }
        public string signable_message { get; set; }
    }

    public class DataNft
    {
        public string token_address { get; set; }
        public string token_id { get; set; }
    }

    public class SignableTransferRequest
    {
        public string amount { get; set; }
        public string receiver { get; set; }
        public string sender { get; set; }
        public ImxTokenNft token { get; set; }
    }

    public class ImxTokenNft
    {
        public string type { get; set; }
        public DataNft data { get; set; }
    }


}
