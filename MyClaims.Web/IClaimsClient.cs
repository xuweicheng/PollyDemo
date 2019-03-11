using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyClaims.Web
{
    public interface IClaimsClient
    {
        string BaseUrl { get; set; }

        Task<ICollection<MyClaim>> GetAllAsync();
        Task<ICollection<MyClaim>> GetAllAsync(CancellationToken cancellationToken);
        Task<MyClaim> GetAsync(string id);
        Task<MyClaim> GetAsync(string id, CancellationToken cancellationToken);
        Task<string> PostAsync(MyClaim claim);
        Task<string> PostAsync(MyClaim claim, CancellationToken cancellationToken);
    }
}