using fly.Data;
using fly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Museum.Controllers
{
    
    public class StorageLocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StorageLocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.StorageLocations.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StorageLocationId,Name,Address")] StorageLocation storageLocation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storageLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(storageLocation);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var storageLocation = await _context.StorageLocations.FindAsync(id);
            if (storageLocation == null) return NotFound();
            return View(storageLocation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StorageLocationId,Name,Address")] StorageLocation storageLocation)
        {
            if (id != storageLocation.StorageLocationId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storageLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.StorageLocations.Any(e => e.StorageLocationId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(storageLocation);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var storageLocation = await _context.StorageLocations.FirstOrDefaultAsync(m => m.StorageLocationId == id);
            if (storageLocation == null) return NotFound();
            return View(storageLocation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storageLocation = await _context.StorageLocations.FindAsync(id);
            if (storageLocation != null)
            {
                _context.StorageLocations.Remove(storageLocation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}