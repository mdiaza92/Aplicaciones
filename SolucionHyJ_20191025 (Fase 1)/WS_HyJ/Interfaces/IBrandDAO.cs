using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Interfaces
{
    public interface IBrandDAO
    {
        Task<SaveBrandResponse> Add(BrandEntity model);
        Task<SaveBrandResponse> Update(BrandEntity model);
        Task<SaveBrandResponse> Delete(string id);
        Task<SaveBrandResponse> Get(string id);
        Task<IEnumerable<BrandEntity>> GetAll();
    }
}
