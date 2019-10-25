using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Responses;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Interfaces
{
    public interface IProviderDAO
    {
        Task<SaveProviderResponse> Add(ProviderEntity model);
        Task<SaveProviderResponse> Update(ProviderEntity model);
        Task<SaveProviderResponse> Delete(string id);
        Task<SaveProviderResponse> GetProvider(string id);
        Task<IEnumerable<ProviderEntity>> GetProviders();
    }
}
