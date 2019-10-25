using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Interfaces
{
    public interface IProductDAO
    {
        Task<SaveProductResponse> Add(ProductRequest model);
        Task<SaveProductResponse> Update(string id, ProductRequest model);
        Task<SaveProductResponse> Delete(string id);
        Task<SaveProductResponse> Get(string id);
        Task<IEnumerable<ProductResponse>> GetAll();
        Task<IEnumerable<ProductResponse>> GetProductsByProvider(string idProvider);
    }
}
