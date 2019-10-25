using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WS_HyJ.Helpers;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Enum;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses.Receipt;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Repository.DAO
{
    public class PaymentMethodDAO : IPaymentMethodDAO
    {
        private readonly MongoDbContext _context = null;

        public PaymentMethodDAO(IOptions<Settings> settings)
        {
            _context = new MongoDbContext(settings);
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<SaveReceiptResponse> AddLetterPaymentMethod(string id, LetterMethodPaymentRequest model)
        {
            List<LetterDetailEntity> letterDetails = null;

            //Si el tipo de pago es una letra, debe de cargar el detalle de la letra
            if (model.TipoPago != EPaymentType.Letra)
                return new SaveReceiptResponse("El método actual sólo soporta la carga de letras.");

            letterDetails = new List<LetterDetailEntity>();

            foreach (var item in model.DetalleLetra)
            {
                letterDetails.Add(new LetterDetailEntity()
                {
                    Fecha = item.Fecha,
                    Dias = item.Dias,
                    Monto = item.Monto
                });
            }

            PaymentMethodEntity payment = new PaymentMethodEntity()
            {
                IdReceipt = id,
                CantidadLetra = model.CantidadLetra,
                TipoPago = model.TipoPago,
                DetalleLetra = letterDetails,
                EfectivoCheque = null,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };

            try
            {
                await _context.PaymentMethod.InsertOneAsync(payment);

                return new SaveReceiptResponse(new ReceiptResponse() { Id = id });
            }
            catch (AppException ex)
            {
                return new SaveReceiptResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private byte[] ConvertImageToByte(IFormFile imagen)
        {
            byte[] p1 = null;
            using (var fs1 = imagen.OpenReadStream())
            using (var ms1 = new MemoryStream())
            {
                fs1.CopyTo(ms1);
                p1 = ms1.ToArray();
            }

            return p1;
        }

        public async Task<EPaymentType?> GetPaymentTypeByReceipt(string idcomprobante)
        {
            var model = await _context.PaymentMethod
                            .Find(_ => _.IdReceipt == idcomprobante)
                            .FirstOrDefaultAsync();

            if (model != null)
                return model.TipoPago;
            else
                return null;
        }

        public async Task<IEnumerable<PaymentMethodEntity>> GetAll()
        {
            try
            {
                List<PaymentMethodEntity> response = new List<PaymentMethodEntity>();

                var list = await _context.PaymentMethod.Find(_ => true).ToListAsync();

                foreach (var item in list)
                {
                    response.Add(new PaymentMethodEntity()
                    {
                        CantidadLetra = item.CantidadLetra,
                        CreatedOn = item.CreatedOn,
                        DetalleLetra = item.DetalleLetra,
                        EfectivoCheque = item.EfectivoCheque,
                        Id = item.Id,
                        IdReceipt = item.IdReceipt,
                        InternalId = item.InternalId,
                        TipoPago = item.TipoPago,
                        UpdatedOn = item.UpdatedOn
                    });
                }

                return response;
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        /// <summary>
        /// Agrega y/o actualiza la imagen del método de pago en base al comprobante
        /// </summary>
        /// <param name="id">ID del comprobante</param>
        /// <param name="model">Modelo</param>
        public async Task<SaveReceiptResponse> AddOtherPaymentMethod(string id, OtherMethodPaymentRequest model)
        {
            EfectivoChequeEntity ecentity = null;

            if (model.TipoPago == EPaymentType.Letra)
                return new SaveReceiptResponse("El método actual no soporta la carga de letras.");

            var exist = await _context.Receipt
                                .Find(_ => _.Id == id)
                                .FirstOrDefaultAsync();

            if (exist == null) return new SaveReceiptResponse($"No se encontró el comprobante.");

            if (model.Imagen != null)
            {
                if (model.Imagen.Length > 0)
                {
                    //Convert Image to byte
                    byte[] p1 = ConvertImageToByte(model.Imagen);

                    ecentity = new EfectivoChequeEntity()
                    {
                        NombreArchivo = model.Imagen.FileName,
                        Archivo = p1
                    };
                }
            }

            PaymentMethodEntity payment = new PaymentMethodEntity()
            {
                IdReceipt = id,
                CantidadLetra = null,
                TipoPago = model.TipoPago,
                DetalleLetra = null,
                EfectivoCheque = ecentity,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };

            try
            {
                await _context.PaymentMethod.InsertOneAsync(payment);

                return new SaveReceiptResponse(new ReceiptResponse() { Id = id });
            }
            catch (AppException ex)
            {
                return new SaveReceiptResponse(ex.Message, HttpStatusCode.InternalServerError);
            }

        }
    }
}
