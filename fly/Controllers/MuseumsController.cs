using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fly.Data;
using fly.Models;
using Microsoft.Extensions.Hosting;
using NuGet.ProjectModel;

namespace fly.Controllers
{
    public class MuseumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MuseumsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Museums
        public async Task<IActionResult> Index()
        {
            return View(await _context.Museum.Include(p =>p.Exhibit).
            ToListAsync());
        }

        // GET: Museums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var museum = await _context.Museum
                .FirstOrDefaultAsync(m => m.MuseumId == id);
            if (museum == null)
            {
                return NotFound();
            }

            return View(museum);
        }

        // GET: Museums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Museums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MuseumId,MuseumName,MuseumAddress")] Museum museum, IFormFile logoFile)
        {
            if (ModelState.IsValid)
            {

                if (logoFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "exhibit");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + logoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(fileStream);
                    }
                    museum.LogoPath = "/images/exhibit/" + uniqueFileName;
                }

                _context.Add(museum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
             

            }

            return View(museum);
        }

        // GET: Museums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var museum = await _context.Museum.FindAsync(id);
            if (museum == null)
            {
                return NotFound();
            }
            return View(museum);
        }

        // POST: Museums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MuseumId,MuseumName,MuseumAddress")] Museum museum)
        {
            if (id != museum.MuseumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(museum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MuseumExists(museum.MuseumId))
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
            return View(museum);
        }

        // GET: Museums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var museum = await _context.Museum
                .FirstOrDefaultAsync(m => m.MuseumId == id);
            if (museum == null)
            {
                return NotFound();
            }

            return View(museum);
        }

        // POST: Museums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var museum = await _context.Museum.FindAsync(id);
            if (museum != null)
            {
                _context.Museum.Remove(museum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MuseumExists(int id)
        {
            return _context.Museum.Any(e => e.MuseumId == id);
        }
    }
}
