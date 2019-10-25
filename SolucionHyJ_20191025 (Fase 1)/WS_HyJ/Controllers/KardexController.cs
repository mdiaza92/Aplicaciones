﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class KardexController : ControllerBase
    {
        private readonly IKardexDAO _context;
        private readonly IMapper _mapper;

        public KardexController(IKardexDAO dao, IMapper mapper)
        {
            _context = dao;
            _mapper = mapper;
        }

        // GET: api/PhysicalInventory
        [HttpGet()]
        public async Task<IEnumerable<KardexEntity>> Get() => await _context.GetAll();

        // GET: api/PhysicalInventory/5
        [HttpGet("{id:length(36)}", Name = "GetPI")]
        public async Task<KardexEntity> Get(string id)
        {
            var response = await _context.Get(id);

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

            return response._model;
        }

        [HttpGet("GetStock/{idProvider:length(36)}")]
        public async Task<IEnumerable<ProductsByProvider>> GetStock(string idProvider) => await _context.GetProductsByProvider(idProvider);

        // POST: api/PhysicalInventory
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] KardexRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _context.Add(model);

            if (result.Success)
                return Created(new Uri($"{HttpContext.Request.Path}", UriKind.Relative), new { id = result._model.Id });
            else
                return BadRequest(result.Message);
        }

        // PUT: api/PhysicalInventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] KardexRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rsp = await _context.Update(id, model);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = "Se actualizó correctamente." });
            else
                return BadRequest(rsp.Message);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rsp = await _context.Delete(id);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = "Se eliminó correctamente." });
            else
                return BadRequest(rsp.Message);
        }
    }
}
