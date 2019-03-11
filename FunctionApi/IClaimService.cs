using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApi
{
    public interface IClaimService
    {
        Task<HttpResponseMessage> GetClaims();
    }
}