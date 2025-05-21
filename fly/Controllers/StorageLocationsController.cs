using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fly.Models;
using fly.Data;

namespace fly.Controllers
{
    public class StorageLocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StorageLocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StorageLocations
        public async Task<IActionResult> Index()
        {
            return View(await _context.StorageLocations.ToListAsync());
        }

        // GET: StorageLocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var storageLocation = await _context.StorageLocations
                .FirstOrDefaultAsync(m => m.StorageLocationId == id);

            if (storageLocation == null)
                return NotFound();

            return View(storageLocation);
        }

        // GET: StorageLocations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StorageLocations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StorageLocationId,Name,Description,Address")] StorageLocation storageLocation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(storageLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(storageLocation);
        }

        // GET: StorageLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var storageLocation = await _context.StorageLocations.FindAsync(id);
            if (storageLocation == null)
                return NotFound();

            return View(storageLocation);
        }

        // POST: StorageLocations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StorageLocationId,Name,Description,Address")] StorageLocation storageLocation)
        {
            if (id != storageLocation.StorageLocationId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(storageLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StorageLocationExists(storageLocation.StorageLocationId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(storageLocation);
        }

        // GET: StorageLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var storageLocation = await _context.StorageLocations
                .FirstOrDefaultAsync(m => m.StorageLocationId == id);
            if (storageLocation == null)
                return NotFound();

            return View(storageLocation);
        }

        // POST: StorageLocations/Delete/5
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

        private bool StorageLocationExists(int id)
        {
            return _context.StorageLocations.Any(e => e.StorageLocationId == id);
        }
    }
}