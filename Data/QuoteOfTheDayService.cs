using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace QuoteOfTheDay.Data
{
    public class QuoteOfTheDayService
    {
        private HttpClient _client;
        private IMemoryCache _cache;

        public QuoteOfTheDayService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _client = httpClientFactory.CreateClient();
            _cache = cache;
        }

        public async Task<Quote> GetQuoteOfTheDayAsync()
        {
            var quote = CacheGet();
            if (quote != null)
            {
                return quote;
            }

            var response = await _client.GetAsync("https://quotes.rest/qod?language=en");
            quote = new Quote
            {
                quote = "A million people may call the mountains a fiction, yet it need not trouble you as you stand atop them.",
                author = "Randal Monroe"
            };
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var quoteResponse = JsonConvert.DeserializeObject<QuoteResponse>(body);
                quote.quote = quoteResponse.contents.quotes.First().quote;
                quote.author = quoteResponse.contents.quotes.First().author;
                CacheSet(quote);
            }
            return quote;
        }

        private void CacheSet(Quote quote)
        {
            var serializedItem = JsonConvert.SerializeObject(quote, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            DateTimeOffset today = DateTime.Today;
            DateTimeOffset midNight = today.AddDays(1).AddSeconds(-1);
            _cache.Set($"quote", serializedItem, midNight);
        }

        private Quote CacheGet()
        {
            var item = (string)_cache.Get($"quote");
            if (item != null)
            {
                return JsonConvert.DeserializeObject<Quote>(item, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            return null;
        }
    }
}