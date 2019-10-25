using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WS_HyJ.Helpers;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class OptionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IImageHandler _imageHandler;

        public OptionController(IConfiguration configuration, IImageHandler imageHandler)
        {
            _configuration = configuration;
            _imageHandler = imageHandler;
        }

        /// <summary>
        /// Obtener el IGV actual.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetIGV")]
        public IActionResult GetIGV()
        {
            try
            {
                string IGV = _configuration["IGV"];

                var response = double.TryParse(IGV, out double result);

                if (response)
                    return Ok(result);
                else
                    return BadRequest("No se logró obtener un valor válido.");
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtener la lista de unidades de medida disponibles para el sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUnidadMedida")]
        public IActionResult GetUnidadMedida()
        {
            Dictionary<string, int> lista = ((EUnitOfMeasurement[])Enum.GetValues(typeof(EUnitOfMeasurement))).ToDictionary(k => k.ToString(), v => (int)v);

            return Ok(lista.ToList());
        }

        /// <summary>
        /// Obtener la lista de monedas disponibles para el sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTipoMoneda")]
        public IActionResult GetTipoMoneda()
        {
            Dictionary<string, int> lista = ((ECurrencyType[])Enum.GetValues(typeof(ECurrencyType))).ToDictionary(k => k.ToString(), v => (int)v);

            return Ok(lista.ToList());
        }

        /// <summary>
        /// Obtener la lista de estados de las órdenes disponibles para el sistema.
        /// </summary>
        [HttpGet("GetEstadoOrden")]
        public IActionResult GetEstadoOrden()
        {
            Dictionary<string, int> lista = ((EOrderStatus[])Enum.GetValues(typeof(EOrderStatus))).ToDictionary(k => k.ToString(), v => (int)v);

            return Ok(lista.ToList());
        }

        [HttpGet("GetEstadoLetra")]
        public IActionResult GetEstadoLetra()
        {
            Dictionary<string, int> lista = ((ELetterStatus[])Enum.GetValues(typeof(ELetterStatus))).ToDictionary(k => k.ToString(), v => (int)v);

            return Ok(lista.ToList());
        }

        [HttpGet("GetTipoComprobante")]
        public IActionResult GetTipoComprobante()
        {
            Dictionary<string, int> lista = ((EReceiptType[])Enum.GetValues(typeof(EReceiptType))).ToDictionary(k => k.ToString(), v => (int)v);

            return Ok(lista.ToList());
        }

        [HttpGet("GetEstadoRecibo")]
        public IActionResult GetEstadoRecibo()
        {
            Dictionary<string, int> lista = ((EReceiptStatus[])Enum.GetValues(typeof(EReceiptStatus))).ToDictionary(k => k.ToString(), v => (int)v);

            return Ok(lista.ToList());
        }

        [HttpGet("GetTipoPago")]
        public IActionResult GetTipoPago()
        {
            Dictionary<string, int> lista = ((EPaymentType[])Enum.GetValues(typeof(EPaymentType))).ToDictionary(k => k.ToString(), v => (int)v);

            return Ok(lista.ToList());
        }

        /// <summary>
        /// Uplaods an image to the server.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            return await _imageHandler.UploadImage(file);
        }
    }
}