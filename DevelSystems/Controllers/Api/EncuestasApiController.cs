using DevelSystem.Context;
using DevelSystem.Data;
using DevelSystem.Interfaces;
using DevelSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevelSystem.Controllers.Api
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EncuestasApiController : ControllerBase
    {
        private readonly IUnitOfWorkFactory uowFactory;
        private readonly IRepository<Encuesta> repository;
        private readonly IRepository<Pregunta> repositoryPreguntas;
        //
        // GET: /Encuestas
        public EncuestasApiController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            this.uowFactory = new EntityFrameworkUnitOfWorkFactory(context);
            this.repository = new EntityFrameworkRepository<Encuesta>(context);
            this.repositoryPreguntas = new EntityFrameworkRepository<Pregunta>(context);
        }

        //
        // GET: /Encuestas/Get
        [HttpGet]
        [Route("GetEncuestas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> Get()
        {
            var query = await repository.All();
            foreach (var item in query)
            {
                item.Preguntas = await repositoryPreguntas.GetBy(o => o.EncuestaId == item.Id).ToListAsync();
            }
            return query;
        }


        //
        // POST: /Encuestas/Create

        [HttpPost]
        [Route("CrearEncuestas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> Create([FromBody] Encuesta entity)
        {
            if (entity == null)
                return BadRequest();

            if (ModelState.IsValid)
                using (IUnitOfWork uow = uowFactory.Create())
                {                    
                    entity.Nombre = entity.Nombre?.ToUpper();
                    entity.Descripcion = entity.Descripcion?.ToUpper();
                    await repository.Add(entity);
                    entity.Url = $"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host.Value}/Encuestas/Responder?Id={entity.Id}";
                    if (entity.Preguntas.Any())
                        await repositoryPreguntas.AddAll(entity.Preguntas);
                    uow.Save();
                    return Ok(entity);
                }
            return BadRequest();
        }

        //
        // GET: /Encuestas/GetById		    
        [Route("GetByIdEncuestas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<Object> GetById(Guid? Id)
        {
            if (Id == null)
                return BadRequest();

            Encuesta? entity = await repository.GetBy(x => x.Id == Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }

            entity.Preguntas = await repositoryPreguntas.GetBy(o => o.EncuestaId == entity.Id).ToListAsync();
            return entity;
        }


        //
        // PUT: /Encuestas/Edit
        [HttpPut]
        [Route("EditEncuestas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> Update(System.Guid? Id, [FromBody] Encuesta entity)
        {
            if (entity == null || Id == null)
            {
                return BadRequest();
            }
            Encuesta? original = await repository.GetBy(x => x.Id == Id).FirstOrDefaultAsync();
            if (original == null)
            {
                return NotFound();
            }
            using (IUnitOfWork uow = uowFactory.Create())
            {
                original.Nombre = entity.Nombre?.ToUpper();
                original.Descripcion = entity.Descripcion?.ToUpper();
                repository.Update(original);
                uow.Save();
            }
            return Ok();
        }

        //
        // DELETE: /Encuestas/Delete
        [HttpDelete]
        [Route("DeleteEncuestas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> Delete(System.Guid? Id)
        {
            if (Id == null)
                return BadRequest();

            Encuesta? entity = await repository.GetBy(x => x.Id == Id).FirstOrDefaultAsync();
            if (entity == null)
            {
                return NotFound();
            }
            using (IUnitOfWork uow = uowFactory.Create())
            {
                repository.Remove(entity);
                uow.Save();
                return Ok();
            }
        }
    }
}

