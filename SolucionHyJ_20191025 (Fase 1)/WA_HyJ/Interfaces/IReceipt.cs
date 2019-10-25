using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WA_HyJ.Models.Request;
using WA_HyJ.Models.Response;

namespace WA_HyJ.Interfaces
{
    public interface IReceipt
    {
        Task<List<ReceiptResponse>> GetAllAsync(string token = null);

        Task<Uri> AddAsync(ReceiptRequest model, string token = null);

        Task<HttpStatusCode> DeleteAsync(string id, string token = null);

        Task<HttpStatusCode> UpdateAsync(string id, ReceiptRequest model, string token = null);
        
        /// <param name="id">ID del comprobante</param>
        /// <returns></returns>
        Task<ReceiptResponse> DetailsAsync(string id, string token = null);

        /// <param name="id">ID del comprobante</param>
        /// <param name="model">Modelo</param>
        Task<object> AddOtherPaymentMethodAsync(string id, OtherMethodPaymentRequest model, string token = null);

        /// <param name="id">ID del comprobante</param>
        /// <param name="model">Modelo del método de pago</param>
        Task<object> AddLetterPaymentMethodAsync(string id, LetterMethodPaymentRequest model, string token = null);
    }
}
