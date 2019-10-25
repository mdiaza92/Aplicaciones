using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WS_HyJ.Helpers;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses;
using WS_HyJ.Models.Responses.Receipt;

namespace WS_HyJ.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptDAO _receiptDAO;
        private readonly IPaymentMethodDAO _paymentMethodDAO;
        private readonly IMapper _mapper;

        public ReceiptController(IReceiptDAO ReceiptDAO, IMapper mapper, IPaymentMethodDAO paymentMethod)
        {
            _receiptDAO = ReceiptDAO;
            _mapper = mapper;
            _paymentMethodDAO = paymentMethod;
        }

        // GET: api/Receipt
        [HttpGet]
        public async Task<IEnumerable<ReceiptResponse>> Get() => await _receiptDAO.GetAll();

        // GET: api/Receipt/5
        [HttpGet("{id}", Name = "GetReceipt")]
        public async Task<ReceiptResponse> Get(string id)
        {
            var response = await _receiptDAO.Get(id);

            if (!response.Success)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        NotFound(response.Message);
                        break;
                    default:
                        BadRequest(response.Message);
                        break;
                }
            }

            return response._receiptResponse;
        }

        /// <summary>
        /// Crear un nuevo recibo
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReceiptRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _receiptDAO.Add(model);

            if (result.Success)
                return Created(new Uri($"{HttpContext.Request.Path}", UriKind.Relative), new { id = result._receiptResponse.Id });
            else
                return BadRequest(result.Message);
        }

        [HttpPost("AddLetterPaymentMethod/{id}")]
        public async Task<IActionResult> AddLetterPaymentMethod(string id, LetterMethodPaymentRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rsp = await _paymentMethodDAO.AddLetterPaymentMethod(id, model);
            
            if (rsp.Success)
                return Created(new Uri($"{HttpContext.Request.Path}", UriKind.Relative), new OkResponse { Id = id, Response = $"Se agregó el método de pago tipo {model.TipoPago.ToDescriptionString()}." });
            else
                return BadRequest(rsp.Message);
        }

        [HttpPost("AddOtherPaymentMethod/{id}")]
        public async Task<IActionResult> AddOtherPaymentMethod(string id, [FromForm] OtherMethodPaymentRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentMethodDAO.AddOtherPaymentMethod(id, model);

            if (result.Success)
                return Created(new Uri($"{HttpContext.Request.Path}", UriKind.Relative), new OkResponse { Id = id, Response = $"Se agregó el método de pago tipo {model.TipoPago.ToDescriptionString()} y la imagen {model.Imagen.FileName} como sustento." });
            else
                return BadRequest(result.Message);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rsp = await _receiptDAO.Delete(id);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = rsp.Message });
            else
                return BadRequest(rsp.Message);
        }

        [HttpGet("GetAllPM")]
        public async Task<IEnumerable<PaymentMethodEntity>> GetAllPM() => await _paymentMethodDAO.GetAll();
    }
}
