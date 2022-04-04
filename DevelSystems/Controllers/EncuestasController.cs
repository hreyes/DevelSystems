using DevelSystem.Context;
using DevelSystem.Data;
using DevelSystem.Interfaces;
using DevelSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace DevelSystem.Controllers
{
    public class EncuestasController : Controller
    {
        private IUnitOfWorkFactory uowFactory;
        private IRepository<Encuesta> repository;
        private IRepository<Pregunta> repositoryPreguntas;
        public EncuestasController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            this.uowFactory = new EntityFrameworkUnitOfWorkFactory(context);
            this.repository = new EntityFrameworkRepository<Encuesta>(context);
            this.repositoryPreguntas = new EntityFrameworkRepository<Pregunta>(context);
        }

        //
        // GET: /Encuestas

        public ViewResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Paging([FromBody] DataManagerRequest dm)
        {
            IQueryable<Encuesta> DataSource = await repository.All();

            DataSource = DataSource.OrderByDescending(o => o.Id);
            DataOperations operation = new();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0)
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<Encuesta>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        

        //
        // GET: /Encuestas/Details

        public async Task<ActionResult> Responder(System.Guid Id)
        {
            Encuesta? entity = await repository.GetBy(x => x.Id == Id).FirstOrDefaultAsync();
            entity.Preguntas = await repositoryPreguntas.GetBy(o => o.EncuestaId == entity.Id).ToListAsync();
            return View(entity);
        }

    }
}

