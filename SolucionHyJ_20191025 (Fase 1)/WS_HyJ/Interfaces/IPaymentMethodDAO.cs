using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses.Receipt;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Interfaces
{
    public interface IPaymentMethodDAO
    {
        /// <summary>
        /// Agrega los campos de método de pago del comprobante
        /// </summary>
        /// <param name="id">Id del comprobante</param>
        /// <param name="model">Datos por el método de pago</param>
        /// <returns></returns>
        Task<SaveReceiptResponse> AddLetterPaymentMethod(string id, LetterMethodPaymentRequest model);

        /// <summary>
        /// Obtener el tipo de pago del comprobante
        /// </summary>
        /// <param name="idRecibo">ID del comprobante</param>
        /// <returns></returns>
        Task<EPaymentType?> GetPaymentTypeByReceipt(string idRecibo);

        /// <summary>
        /// Agrega y/o actualiza la imagen del método de pago en base al comprobante
        /// </summary>
        /// <param name="id">ID del comprobante</param>
        /// <param name="model">Modelo asociado a los métodos de pago para los que no son letras</param>
        Task<SaveReceiptResponse> AddOtherPaymentMethod(string id, OtherMethodPaymentRequest model);

        Task<IEnumerable<PaymentMethodEntity>> GetAll();
    }
}
