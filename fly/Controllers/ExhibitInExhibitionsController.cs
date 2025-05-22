using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fly.Data;
using fly.Models;

namespace fly.Controllers
{
    public class ExhibitInExhibitionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExhibitInExhibitionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExhibitInExhibitions
        public async Task<IActionResult> Index(int? exhibitionId)
        {
            if (exhibitionId == null)
            {
                return NotFound();
            }

            var exhibition = await _context.Exhibition.FirstOrDefaultAsync(e => e.ExhibitionId == exhibitionId);
            if (exhibition == null)
            {
                return NotFound();
            }

            ViewBag.ExhibitionName = exhibition.ExhibitionName;
            ViewBag.ExhibitionId = exhibitionId;

            var exhibitInExhibitions = _context.ExhibitInExhibition
                .Include(x => x.Exhibit)
                .Where(x => x.ExhibitionId == exhibitionId);

            return View(await exhibitInExhibitions.ToListAsync());
        }

        // GET: ExhibitInExhibitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibitInExhibition = await _context.ExhibitInExhibition
                .Include(e => e.Exhibit)
                .Include(e => e.Exhibition)
                .FirstOrDefaultAsync(m => m.ExhibitInExhibitionId == id);
            if (exhibitInExhibition == null)
            {
                return NotFound();
            }

            return View(exhibitInExhibition);
        }

        public IActionResult Create(int? exhibitionId)
        {
            if (exhibitionId == null)
                return NotFound();

            ViewBag.ExhibitionId = exhibitionId;
            ViewBag.ExhibitionName = _context.Exhibition.FirstOrDefault(e => e.ExhibitionId == exhibitionId)?.ExhibitionName;
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExhibitInExhibitionId,ExhibitId,ExhibitionId,PlacementDate")] ExhibitInExhibition exhibitInExhibition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exhibitInExhibition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { exhibitionId = exhibitInExhibition.ExhibitionId });
            }
            ViewBag.ExhibitionId = exhibitInExhibition.ExhibitionId;
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", exhibitInExhibition.ExhibitId);
            return View(exhibitInExhibition);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var exhibitInExhibition = await _context.ExhibitInExhibition
                .Include(e => e.Exhibition)
                .FirstOrDefaultAsync(e => e.ExhibitInExhibitionId == id);
            if (exhibitInExhibition == null)
                return NotFound();

            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", exhibitInExhibition.ExhibitId);
            ViewData["ExhibitionId"] = exhibitInExhibition.ExhibitionId;
            return View(exhibitInExhibition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExhibitInExhibitionId,ExhibitId,ExhibitionId,PlacementDate")] ExhibitInExhibition exhibitInExhibition)
        {
            if (id != exhibitInExhibition.ExhibitInExhibitionId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exhibitInExhibition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExhibitInExhibitionExists(exhibitInExhibition.ExhibitInExhibitionId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Details", "Exhibitions", new { id = exhibitInExhibition.ExhibitionId });
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", exhibitInExhibition.ExhibitId);
            ViewData["ExhibitionId"] = exhibitInExhibition.ExhibitionId;
            return View(exhibitInExhibition);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var exhibitInExhibition = await _context.ExhibitInExhibition
                .Include(e => e.Exhibit)
                .Include(e => e.Exhibition)
                .FirstOrDefaultAsync(m => m.ExhibitInExhibitionId == id);
            if (exhibitInExhibition == null)
                return NotFound();

            return View(exhibitInExhibition);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exhibitInExhibition = await _context.ExhibitInExhibition.FindAsync(id);
            int exhibitionId = exhibitInExhibition.ExhibitionId;
            _context.ExhibitInExhibition.Remove(exhibitInExhibition);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Exhibitions", new { id = exhibitionId });
        }

        private bool ExhibitInExhibitionExists(int id)
        {
            return _context.ExhibitInExhibition.Any(e => e.ExhibitInExhibitionId == id);
        }
    }
}
