using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ.Interfaces
{
    public interface IOption
    {
        Task<double> GetIGVAsync(string token = null);
        Task<List<EnumModel>> GetUnidadMedida(string token = null);
        Task<List<EnumModel>> GetTipoMoneda(string token = null);
        Task<List<EnumModel>> GetTipoRecibo(string token = null);
        Task<List<EnumModel>> GetTipoPago(string token = null);
    }
}
