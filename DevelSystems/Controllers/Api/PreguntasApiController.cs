using DevelSystem.Context;
using DevelSystem.Data;
using DevelSystem.Interfaces;
using DevelSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevelSystem.Controllers.Api
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PreguntasApiController : ControllerBase
    {
        private readonly IUnitOfWorkFactory uowFactory;
        private readonly IRepository<Pregunta> repository;
        private readonly IRepository<Encuesta> EncuestaRepository;

        //
        // GET: /Preguntas
        public PreguntasApiController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            this.uowFactory = new EntityFrameworkUnitOfWorkFactory(context);
            this.repository = new EntityFrameworkRepository<Pregunta>(context);
            this.EncuestaRepository = new EntityFrameworkRepository<Encuesta>(context);
        }


        //
        // POST: /Preguntas/Create

        [HttpPost]
        [Route("CrearPreguntas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> Create([FromBody] Pregunta entity)
        {
            if (entity == null)
                return BadRequest();

            if (ModelState.IsValid)
                using (IUnitOfWork uow = uowFactory.Create())
                {
                    entity.Id = entity.Id;
                    entity.Nombre = entity.Nombre?.ToUpper();
                    entity.Titulo = entity.Titulo?.ToUpper();
                    entity.Requerido = entity.Requerido;
                    entity.Tipo = entity.Tipo;
                    entity.EncuestaId = entity.EncuestaId;
                    await repository.Add(entity);
                    uow.Save();
                    return Ok(entity);
                }
            return BadRequest();
        }


        //
        // PUT: /Preguntas/Edit
        [HttpPut]
        [Route("EditPreguntas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> Update(System.Guid? Id, [FromBody] Pregunta entity)
        {
            if (entity == null || Id == null)
            {
                return BadRequest();
            }
            Pregunta? original = await repository.GetBy(x => x.Id == Id).FirstOrDefaultAsync();
            if (original == null)
            {
                return NotFound();
            }
            using (IUnitOfWork uow = uowFactory.Create())
            {
                original.Nombre = entity.Nombre?.ToUpper();
                original.Titulo = entity.Titulo?.ToUpper();
                original.Requerido = entity.Requerido;
                original.Tipo = entity.Tipo;
                repository.Update(original);
                uow.Save();
            }
            return Ok();
        }

        //
        // DELETE: /Preguntas/Delete
        [HttpDelete]
        [Route("DeletePreguntas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> Delete(System.Guid? Id)
        {
            if (Id == null)
                return BadRequest();

            Pregunta? entity = await repository.GetBy(x => x.Id == Id).FirstOrDefaultAsync();
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

