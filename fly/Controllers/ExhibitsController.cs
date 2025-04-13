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
    public class ExhibitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExhibitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exhibits
        public async Task<IActionResult> Index(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationDbContext = _context.Exhibit.
                Where(t => t.MuseumId == id).Include(e => e.Museum);

            string Museum = _context.Museum.FirstOrDefault(t => t.MuseumId == id).MuseumName;
            ViewBag.Museum = Museum;
            ViewBag.MuseumId = id;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Exhibits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibit = await _context.Exhibit
                .Include(e => e.Museum)
                .FirstOrDefaultAsync(m => m.ExhibitId == id);
            if (exhibit == null)
            {
                return NotFound();
            }

            return View(exhibit);
        }

        // GET: Exhibits/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["MuseumId"] = id;
            return View();
        }

        // POST: Exhibits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExhibitId,MuseumId,ExhibitName,ExhibitDescription,CreationDate,Material,Size,Weight")] Exhibit exhibit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exhibit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = exhibit.MuseumId });
            }
            ViewData["MuseumId"] = new SelectList(_context.Museum, "MuseumId", "MuseumAddress", exhibit.MuseumId);
            return View(exhibit);
        }

        // GET: Exhibits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibit = await _context.Exhibit.FindAsync(id);
            if (exhibit == null)
            {
                return NotFound();
            }
            ViewData["MuseumId"] = new SelectList(_context.Museum, "MuseumId", "MuseumAddress", exhibit.MuseumId);
            return View(exhibit);
        }

        // POST: Exhibits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExhibitId,MuseumId,ExhibitName,ExhibitDescription,CreationDate,Material,Size,Weight")] Exhibit exhibit)
        {
            if (id != exhibit.ExhibitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exhibit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExhibitExists(exhibit.ExhibitId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = exhibit.MuseumId });
            }
            ViewData["MuseumId"] = new SelectList(_context.Museum, "MuseumId", "MuseumAddress", exhibit.MuseumId);
            return View(exhibit);
        }

        // GET: Exhibits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibit = await _context.Exhibit
                .Include(e => e.Museum)
                .FirstOrDefaultAsync(m => m.ExhibitId == id);
            if (exhibit == null)
            {
                return NotFound();
            }

            return View(exhibit);
        }

        // POST: Exhibits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exhibit = await _context.Exhibit.FindAsync(id);
            if (exhibit != null)
            {
                _context.Exhibit.Remove(exhibit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {id = exhibit.MuseumId });
        }

        private bool ExhibitExists(int id)
        {
            return _context.Exhibit.Any(e => e.ExhibitId == id);
        }
    }
}
