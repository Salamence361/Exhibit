using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fly.Data;
using fly.Models;

namespace fly.Controllers
{
    public class InsurancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsurancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var insurance = await _context.Insurances
                .Include(r => r.Exhibit)
                .ToListAsync();
            return View(insurance);
        }


        // GET: Insurances/Create

        public IActionResult Create()
        {
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName");
            return View();
        }

        // POST: Insurances/Create
        


        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InsuranceId,ExhibitId,InsuranceCompany,PolicyNumber,StartDate,EndDate,CoverageAmount")] Insurance insurance)
        {
            
            if (ModelState.IsValid)
            {
                Console.WriteLine("Model state is valid, loading related entities...");
                insurance.Exhibit = await _context.Exhibit.FindAsync(insurance.ExhibitId);
                if (insurance.Exhibit == null)
                {
                    ModelState.AddModelError("", "Exhibit not found.");
                    ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", insurance.ExhibitId);
                    return View(insurance);
                }
                Console.WriteLine("Related entities loaded, saving to database...");
                _context.Add(insurance);
                await _context.SaveChangesAsync();
                Console.WriteLine("Save successful, redirecting to Index...");
                return RedirectToAction(nameof(Index));
            }
            Console.WriteLine("Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", insurance.ExhibitId);
            return View(insurance);
        }


        // GET: Insurances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var insurance = await _context.Insurances.FindAsync(id);
            if (insurance == null)
                return NotFound();

            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitDescription", insurance.ExhibitId);
            return View(insurance);
        }

        // POST: Insurances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InsuranceId,ExhibitId,InsuranceCompany,PolicyNumber,StartDate,EndDate,CoverageAmount")] Insurance insurance)
        {
            if (id != insurance.InsuranceId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insurance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsuranceExists(insurance.InsuranceId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitDescription", insurance.ExhibitId);
            return View(insurance);
        }

        // GET: Insurances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var insurance = await _context.Insurances
                .Include(i => i.Exhibit)
                .FirstOrDefaultAsync(m => m.InsuranceId == id);

            if (insurance == null)
                return NotFound();

            return View(insurance);
        }

        // POST: Insurances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var insurance = await _context.Insurances.FindAsync(id);
            if (insurance != null)
                _context.Insurances.Remove(insurance);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsuranceExists(int id)
        {
            return _context.Insurances.Any(e => e.InsuranceId == id);
        }
    }
}