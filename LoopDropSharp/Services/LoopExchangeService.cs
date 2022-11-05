using LoopDropSharp;
using LoopDropSharp.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopDropSharp
{
    public class LoopExchangeService : ILoopExchangeService, IDisposable
    {
        const string _baseUrl = "https://api.loopexchange.art";

        readonly RestClient _client;

        public LoopExchangeService()
        {
            _client = new RestClient(_baseUrl);
        }


        public async Task<LoopExchange> GetLoopPhunksData()
        {
            var request = new RestRequest("web-v1/collection/public/loop-phunks");
            try
            {
                var response = await _client.GetAsync(request);
                var data = JsonConvert.DeserializeObject<LoopExchange>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                Font.SetTextToWhite($"Error getting storage id: {httpException.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }


    }
}
