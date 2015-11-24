﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Backend.Core.Service;
using Shared.Core.DTOs;

namespace Frontend.Web.Core.Controllers
{
    public abstract class RestControllerBase<TService, TDto> : ReadonlyRestControllerBase<TService, TDto> where TDto : IDTO
        where TService : IRestService<TDto>
    {
        protected RestControllerBase(TService service) : base(service)
        {
        }

        protected RestControllerBase()
        {
        }

        public virtual async Task<IHttpActionResult> Put(TDto dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await Service.Put(dto);

            return Ok(result);
        }

        public virtual async Task<IHttpActionResult> Post(TDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await Service.Post(dto);

            return Ok(result);
        }

        public async Task<IHttpActionResult> Delete(int id)
        {
            var wasDeleted = await Service.Delete(id);
            if (!wasDeleted)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}