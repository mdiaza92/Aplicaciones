using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;

namespace BackEnd_HyJ.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProviderRequest, ProviderEntity>();
            CreateMap<ProductRequest, ProductEntity>();
            CreateMap<KardexRequest, KardexEntity>();
        }
    }
}
