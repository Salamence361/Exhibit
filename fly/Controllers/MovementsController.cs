using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using fly.Data;
using fly.Models;
using fly.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace fly.Controllers
{
    public class MovementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;
        private readonly ICompositeViewEngine _viewEngine;

        public MovementsController(ApplicationDbContext context, PdfService pdfService, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _pdfService = pdfService;
            _viewEngine = viewEngine;
        }

        // GET: Movements
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? exhibitId, string exhibitSearch)
        {
            var movementsQuery = _context.Movements
                .Include(m => m.Exhibit)
                .Include(m => m.FromStorageLocation)
                .Include(m => m.ToStorageLocation)
                .AsQueryable();

            if (startDate.HasValue)
            {
                movementsQuery = movementsQuery.Where(m => m.MovementDate >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                movementsQuery = movementsQuery.Where(m => m.MovementDate < endDate.Value.Date.AddDays(1));
            }
            if (exhibitId.HasValue && exhibitId.Value > 0)
            {
                movementsQuery = movementsQuery.Where(m => m.ExhibitId == exhibitId.Value);
            }
            // Фильтрация по названию экспоната
            if (!string.IsNullOrWhiteSpace(exhibitSearch))
            {
                movementsQuery = movementsQuery.Where(m => m.Exhibit.ExhibitName.Contains(exhibitSearch));
            }

            var exhibits = await _context.Exhibit.ToListAsync();
            ViewBag.Exhibits = new SelectList(exhibits, "ExhibitId", "ExhibitName", exhibitId);

            // Для сохранения поискового запроса в поле
            ViewBag.ExhibitSearch = exhibitSearch;

            var movements = await movementsQuery.OrderByDescending(m => m.MovementDate).ToListAsync();
            return View(movements);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(DateTime? startDate, DateTime? endDate, int? exhibitId, string exhibitSearch)
        {
            var movementsQuery = _context.Movements
                .Include(m => m.Exhibit)
                .Include(m => m.FromStorageLocation)
                .Include(m => m.ToStorageLocation)
                .AsQueryable();

            if (startDate.HasValue)
            {
                movementsQuery = movementsQuery.Where(m => m.MovementDate >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                movementsQuery = movementsQuery.Where(m => m.MovementDate < endDate.Value.Date.AddDays(1));
            }
            if (exhibitId.HasValue && exhibitId.Value > 0)
            {
                movementsQuery = movementsQuery.Where(m => m.ExhibitId == exhibitId.Value);
            }
            // Фильтрация по названию экспоната
            if (!string.IsNullOrWhiteSpace(exhibitSearch))
            {
                movementsQuery = movementsQuery.Where(m => m.Exhibit.ExhibitName.Contains(exhibitSearch));
            }

            var movements = await movementsQuery.OrderByDescending(m => m.MovementDate).ToListAsync();

            var htmlContent = await RenderViewToStringAsync("Index", movements, new { isPdf = true });
            var pdfBytes = _pdfService.GeneratePdf(htmlContent);
            return File(pdfBytes, "application/pdf", "MovementsReport.pdf");
        }

        /// <summary>
        /// Для вызова из контроллера Exhibit, чтобы добавить запись о перемещении
        /// </summary>
        public async Task AddMovementRecord(int exhibitId, int? fromStorageLocationId, int toStorageLocationId)
        {
            var movement = new Movement
            {
                ExhibitId = exhibitId,
                FromStorageLocationId = fromStorageLocationId ?? 0,
                ToStorageLocationId = toStorageLocationId,
                MovementDate = DateTime.Now
            };
            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Рендеринг Razor в строку для PDF
        /// </summary>
        private async Task<string> RenderViewToStringAsync(string viewName, object model, object routeValues = null)
        {
            ViewData.Model = model;
            using (var writer = new System.IO.StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"View {viewName} not found");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                if (routeValues != null)
                {
                    foreach (var routeValue in new RouteValueDictionary(routeValues))
                    {
                        viewContext.RouteData.Values[routeValue.Key] = routeValue.Value;
                    }
                }

                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}