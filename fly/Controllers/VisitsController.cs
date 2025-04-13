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
    public class VisitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Visits
        public async Task<IActionResult> Index(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var applicationDbContext = _context.Visit.Where
                (t => t.VisitorId == id).Include(v => v.Visitor);

           // string Exhibition = _context.Exhibition.FirstOrDefault(t => t.ExhibitionId == id).ExhibitionName;
            string Visitor = _context.Visitor.FirstOrDefault(t => t.VisitorId == id).VisitorLastName;
            ViewBag.Visitor = Visitor;
            ViewBag.VisitorId = id;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Visits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visit
                .Include(v => v.Exhibition)
                .Include(v => v.Visitor)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        // GET: Visits/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId");
            ViewData["VisitorId"] = id;
            return View();
        }

        // POST: Visits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VisitId,VisitorId,ExhibitionId,VisitDate")] Visit visit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = visit.VisitorId });
            }
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId", visit.ExhibitionId);
            ViewData["VisitorId"] = new SelectList(_context.Visitor, "VisitorId", "VisitorId", visit.VisitorId);
            return View(visit);
        }

        // GET: Visits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visit.FindAsync(id);
            if (visit == null)
            {
                return NotFound();
            }
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId", visit.ExhibitionId);
            ViewData["VisitorId"] = new SelectList(_context.Visitor, "VisitorId", "VisitorId", visit.VisitorId);
            return View(visit);
        }

        // POST: Visits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisitId,VisitorId,ExhibitionId,VisitDate")] Visit visit)
        {
            if (id != visit.VisitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitExists(visit.VisitId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = visit.VisitorId });
            }
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibition, "ExhibitionId", "ExhibitionId", visit.ExhibitionId);
            ViewData["VisitorId"] = new SelectList(_context.Visitor, "VisitorId", "VisitorId", visit.VisitorId);
            return View(visit);
        }

        // GET: Visits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visit
                .Include(v => v.Exhibition)
                .Include(v => v.Visitor)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        // POST: Visits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visit = await _context.Visit.FindAsync(id);
            if (visit != null)
            {
                _context.Visit.Remove(visit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = visit.VisitorId });
        }

        private bool VisitExists(int id)
        {
            return _context.Visit.Any(e => e.VisitId == id);
        }
    }
}
