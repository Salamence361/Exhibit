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
    public class MovementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movements
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Movements.Include(m => m.Exhibit).Include(m => m.FromStorageLocation).Include(m => m.ToStorageLocation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Movements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movement = await _context.Movements
                .Include(m => m.Exhibit)
                .Include(m => m.FromStorageLocation)
                .Include(m => m.ToStorageLocation)
                .FirstOrDefaultAsync(m => m.MovementId == id);
            if (movement == null)
            {
                return NotFound();
            }

            return View(movement);
        }

        // GET: Movements/Create
        public IActionResult Create()
        {
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitDescription");
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId");
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId");
            return View();
        }

        // POST: Movements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovementId,ExhibitId,FromStorageLocationId,ToStorageLocationId,MovementDate,Notes")] Movement movement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitDescription", movement.ExhibitId);
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId", movement.FromStorageLocationId);
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId", movement.ToStorageLocationId);
            return View(movement);
        }

        // GET: Movements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movement = await _context.Movements.FindAsync(id);
            if (movement == null)
            {
                return NotFound();
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitDescription", movement.ExhibitId);
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId", movement.FromStorageLocationId);
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId", movement.ToStorageLocationId);
            return View(movement);
        }

        // POST: Movements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovementId,ExhibitId,FromStorageLocationId,ToStorageLocationId,MovementDate,Notes")] Movement movement)
        {
            if (id != movement.MovementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovementExists(movement.MovementId))
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
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitDescription", movement.ExhibitId);
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId", movement.FromStorageLocationId);
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "StorageLocationId", movement.ToStorageLocationId);
            return View(movement);
        }

        // GET: Movements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movement = await _context.Movements
                .Include(m => m.Exhibit)
                .Include(m => m.FromStorageLocation)
                .Include(m => m.ToStorageLocation)
                .FirstOrDefaultAsync(m => m.MovementId == id);
            if (movement == null)
            {
                return NotFound();
            }

            return View(movement);
        }

        // POST: Movements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movement = await _context.Movements.FindAsync(id);
            if (movement != null)
            {
                _context.Movements.Remove(movement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovementExists(int id)
        {
            return _context.Movements.Any(e => e.MovementId == id);
        }
    }
}
