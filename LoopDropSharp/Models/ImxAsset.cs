using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp.Models
{
    public class Attribute
    {
        public string value { get; set; }
        public string trait_type { get; set; }
    }

    public class Collection
    {
        public string name { get; set; }
        public string icon_url { get; set; }
    }

    public class Metadata
    {
        public string aura { get; set; }
        public string body { get; set; }
        public string ears { get; set; }
        public string eyes { get; set; }
        public string hair { get; set; }
        public string name { get; set; }
        public string nose { get; set; }
        public string image { get; set; }
        public string mouth { get; set; }
        public string outfit { get; set; }
        public string rarity { get; set; }
        public string glasses { get; set; }
        public List<Attribute> attributes { get; set; }
        public string description { get; set; }
        public string bodyAccessory { get; set; }
        public string faceAccessory { get; set; }
        public string headAccessory { get; set; }
        public string backgroundColor { get; set; }
    }

    public class Result
    {
        public string token_address { get; set; }
        public string token_id { get; set; }
        public string id { get; set; }
        public string user { get; set; }
        public string status { get; set; }
        public object uri { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image_url { get; set; }
        public Metadata metadata { get; set; }
        public Collection collection { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class ImxAsset
    {
        public List<Result> result { get; set; }
        public string cursor { get; set; }
        public int remaining { get; set; }
    }

    public class AssetExcel
    {
        public string name { get; set; }
        public string user { get; set; }
    }

}
