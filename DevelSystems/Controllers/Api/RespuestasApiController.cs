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
    public class RespuestasApiController : ControllerBase
    {
        private readonly IUnitOfWorkFactory uowFactory;
        private readonly IRepository<Respuesta> repository;
        private readonly IRepository<Pregunta> repositoryPreguntas;
        //
        // GET: /Encuestas
        public RespuestasApiController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            this.uowFactory = new EntityFrameworkUnitOfWorkFactory(context);
            this.repository = new EntityFrameworkRepository<Respuesta>(context);
            this.repositoryPreguntas = new EntityFrameworkRepository<Pregunta>(context);
        }



        //
        // POST: /Encuestas/Create

        [HttpPost]
        [Route("CrearRespuestas")]
        public async Task<Object> Create([FromBody] List<Respuesta> entity)
        {
            if (entity == null)
                return BadRequest();

            foreach (var item in entity)
            {
                var pregunta = await repositoryPreguntas.GetBy(x => x.Id == item.PreguntaId).FirstOrDefaultAsync();
                item.Titulo = pregunta.Titulo;
            }

            if (ModelState.IsValid)
                using (IUnitOfWork uow = uowFactory.Create())
                {
                    await repository.AddAll(entity);
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

            List<Respuesta>? entity = await repository.GetBy(x => x.EncuestaId == Id).ToListAsync();
            if (entity == null)
            {
                return NotFound();
            }
            var respuesta = entity.Select(e => new
            {
                Pregunta = e.Titulo,
                Respuesta = e.ValorRespuesta,
                IdPregunta = e.PreguntaId
            }).ToList();
            return respuesta;
        }



    }
}

