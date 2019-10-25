using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WS_HyJ.Helpers;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses;

namespace WS_HyJ.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderDAO _context;
        private readonly IMapper _mapper;

        public ProviderController(IProviderDAO dao, IMapper mapper)
        {
            _context = dao;
            _mapper = mapper;
        }

        // GET: api/Provider
        [HttpGet(Name = "GetProvider")]
        public async Task<IEnumerable<ProviderEntity>> Get() => await _context.GetProviders();

        // GET: api/Provider/5
        [HttpGet("{id}")]
        public async Task<ProviderEntity> Get(string id)
        {
            var response = await _context.GetProvider(id);

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

        // POST: api/Provider
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProviderRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var _model = _mapper.Map<ProviderEntity>(model);
                _model.CreatedOn = DateTime.Now;

                var result = await _context.Add(_model);

                if (result.Success)
                    return Created(new Uri($"{HttpContext.Request.Path}", UriKind.Relative), new { id = result._model.Id });
                else
                    return BadRequest(result.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Provider/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] ProviderRequest model)
        {
            var _model = _mapper.Map<ProviderEntity>(model);
            _model.Id = id;
            _model.UpdatedOn = DateTime.Now;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rsp = await _context.Update(_model);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = $"Se actualizó el registro con ID {id}." });
            else
                return BadRequest(rsp.Message);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var rsp = await _context.Delete(id);

            if (rsp.Success)
                return Ok(new OkResponse { Id = id, Response = $"Se eliminó el registro con ID {id}." });
            else
                return BadRequest(rsp.Message);
        }
    }
}
