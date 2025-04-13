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
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationDbContext = _context.ExhibitInExhibition.Include(e => e.Exhibit).Include(e => e.Exhibition);

            string Exhibit = _context.Exhibit.FirstOrDefault(t => t.ExhibitId == id).ExhibitName;
            ViewBag.Exhibit = Exhibit;

            return View(await applicationDbContext.ToListAsync());
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

        // GET: ExhibitInExhibitions/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["ExhibitId"] = id;
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId");
            return View();
        }

        // POST: ExhibitInExhibitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExhibitInExhibitionId,ExhibitId,ExhibitionId,PlacementDate")] ExhibitInExhibition exhibitInExhibition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exhibitInExhibition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = exhibitInExhibition.ExhibitId });
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitId", exhibitInExhibition.ExhibitId);
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId", exhibitInExhibition.ExhibitionId);
            return View(exhibitInExhibition);
        }

        // GET: ExhibitInExhibitions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibitInExhibition = await _context.ExhibitInExhibition.FindAsync(id);
            if (exhibitInExhibition == null)
            {
                return NotFound();
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitId", exhibitInExhibition.ExhibitId);
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId", exhibitInExhibition.ExhibitionId);
            return View(exhibitInExhibition);
        }

        // POST: ExhibitInExhibitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExhibitInExhibitionId,ExhibitId,ExhibitionId,PlacementDate")] ExhibitInExhibition exhibitInExhibition)
        {
            if (id != exhibitInExhibition.ExhibitInExhibitionId)
            {
                return NotFound();
            }

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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitId", exhibitInExhibition.ExhibitId);
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId", exhibitInExhibition.ExhibitionId);
            return View(exhibitInExhibition);
        }

        // GET: ExhibitInExhibitions/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: ExhibitInExhibitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exhibitInExhibition = await _context.ExhibitInExhibition.FindAsync(id);
            if (exhibitInExhibition != null)
            {
                _context.ExhibitInExhibition.Remove(exhibitInExhibition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitInExhibitionExists(int id)
        {
            return _context.ExhibitInExhibition.Any(e => e.ExhibitInExhibitionId == id);
        }
    }
}
