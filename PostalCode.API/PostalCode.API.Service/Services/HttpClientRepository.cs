using PostalCode.API.Service.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace PostalCode.API.Service
{
    public class HttpClientRepository : IHttpClientRepository
    {
        private readonly HttpClient _client;

        public HttpClientRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await _client.GetAsync(requestUri);
        }
    }

}
