using System.Net.Http;
using System.Threading.Tasks;

namespace PostalCode.API.Service.Interfaces
{
    public interface IHttpClientRepository
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}
