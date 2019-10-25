using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Enum;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses;
using WS_HyJ.Models.Responses.Order;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderDAO _orderDAO;
        private readonly IReceiptDAO _receiptDAO;
        private readonly IKardexDAO _kardexDAO;

        public OrderController(IOrderDAO dao, IMapper mapper, IReceiptDAO receiptDAO, IKardexDAO kardexDAO)
        {
            _mapper = mapper;
            _orderDAO = dao;
            _receiptDAO = receiptDAO;
            _kardexDAO = kardexDAO;
        }

        /// <summary>
        /// Listar todas las ordenes
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<OrderResponse>> Get() => await _orderDAO.GetAll();

        /// <summary>
        /// Buscar una orden en específico
        /// </summary>
        /// <param name="id">ID de la órden</param>
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<OrderResponse> Get(string id)
        {
            var response = await _orderDAO.Get(id);

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

            return response._orderEntity;
        }

        /// <summary>
        /// Agregar una nueva orden
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderDAO.Add(model);

            if (result.Success)
                return Created(new Uri($"{HttpContext.Request.Path}", UriKind.Relative), new OkResponse { Id = result._orderEntity.Id, Response = result.Message });
            else
                return BadRequest(result.Message);
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] OrderEntity model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rsp = await _orderDAO.Update(null);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = $"Se actualizó el registro con ID {id}." });
            else
                return BadRequest(rsp.Message);
        }

        /// <summary>
        /// Actualiza el estado de la orden a "recibido"
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("UpdateStatus/{id}", Name = "UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rsp = await _orderDAO.UpdateStatus(id);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = $"Se actualizó el registro con ID {id}, con el status {Enums.EOrderStatus.Recibido}." });
            else
                return BadRequest(rsp.Message);
        }

        /// <summary>
        /// Actualiza el detalle de la orden, el estado del comprobante y actualiza el stock o kardex.
        /// </summary>
        /// <param name="idpedido">ID del pedido</param>
        /// <param name="model">Detalles del pedido</param>
        [HttpPut("UpdateOrderDetails/{idpedido}", Name = "UpdateOrderDetails")]
        public async Task<IActionResult> UpdateOrderDetails(string idpedido, [FromBody] List<OrderDetailRequest> model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                //Actualizo los detalles de la orden
                var rsp = await _orderDAO.UpdateOrderDetails(idpedido, model);

                if (rsp.Success)
                {
                    //Actualizo el estatus del comprobante en base a la orden seleccionada
                    await _receiptDAO.UpdateStatusByOrder(idpedido);

                    //Actualizo el kardex
                    await _kardexDAO.UpdateStockByOrder(idpedido);

                    //Confirmar que el pedido ya ha sido validado
                    await _orderDAO.ValidateOrder(idpedido);

                    return Ok(new OkResponse { Id = idpedido, Response = $"Se actualizó el detalle del pedido con ID {idpedido}." });
                }
                else
                    return BadRequest(rsp.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rsp = await _orderDAO.Delete(id);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = rsp.Message });
            else
                return BadRequest(rsp.Message);
        }
    }
}
