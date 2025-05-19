using fly.Data;
using fly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace fly.Controllers
{
    public class RestorationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestorationsController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index()
        {
            var restorations = await _context.Restorations
                .Include(r => r.Exhibit)
                .ToListAsync();
            return View(restorations);
        }

        
        public IActionResult Create()
        {
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName");
            return View();
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RestorationId,ExhibitId,RestorationDate,Description,RestorerName,IsApproved,StartDate,EndDate")] Restoration restoration)
        {
            Console.WriteLine($"Received POST request for Restoration: ExhibitId={restoration.ExhibitId}, RestorationDate={restoration.RestorationDate}, Description={restoration.Description}");
            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid, loading related entities...");
                restoration.Exhibit = await _context.Exhibit.FindAsync(restoration.ExhibitId);
                if (restoration.Exhibit == null)
                {
                    ModelState.AddModelError("", "Exhibit not found.");
                    ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", restoration.ExhibitId);
                    return View(restoration);
                }
                Console.WriteLine("Related entities loaded, saving to database...");
                _context.Add(restoration);
                await _context.SaveChangesAsync();
                Console.WriteLine("Save successful, redirecting to Index...");
                return RedirectToAction(nameof(Index));
            }
            Console.WriteLine("Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", restoration.ExhibitId);
            return View(restoration);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var restoration = await _context.Restorations.FindAsync(id);
            if (restoration == null) return NotFound();
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", restoration.ExhibitId);
            return View(restoration);
        }

        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RestorationId,ExhibitId,RestorationDate,Description,RestorerName,IsApproved,StartDate,EndDate")] Restoration restoration)
        {
            if (id != restoration.RestorationId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restoration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Restorations.Any(e => e.RestorationId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", restoration.ExhibitId);
            return View(restoration);
        }

       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var restoration = await _context.Restorations
                .Include(r => r.Exhibit)
                .FirstOrDefaultAsync(m => m.RestorationId == id);
            if (restoration == null) return NotFound();
            return View(restoration);
        }

        [HttpPost, ActionName("Delete")]
        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restoration = await _context.Restorations.FindAsync(id);
            _context.Restorations.Remove(restoration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}