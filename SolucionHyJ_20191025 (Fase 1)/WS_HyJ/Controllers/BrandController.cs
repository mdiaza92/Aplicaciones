using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Responses;

namespace WS_HyJ.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandDAO _context;
        private readonly IMapper _mapper;

        public BrandController(IBrandDAO dao, IMapper mapper)
        {
            _context = dao;
            _mapper = mapper;
        }

        // GET: api/Brand
        [HttpGet]
        public async Task<IEnumerable<BrandEntity>> Get() => await _context.GetAll();

        // GET: api/Brand/5
        [HttpGet("{id}", Name = "GetBrand")]
        public async Task<BrandEntity> Get(string id)
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

        // POST: api/Brand
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BrandEntity model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var _model = _mapper.Map<BrandEntity>(model);

                var result = await _context.Add(_model);

                if (result.Success)
                    return Created(new Uri($"{HttpContext.Request.Path}", UriKind.Relative), new { id = result._model.Id });
                else
                    return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Brand/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] BrandEntity model)
        {
            var _model = _mapper.Map<BrandEntity>(model);
            _model.Id = id;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rsp = await _context.Update(_model);
            
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
