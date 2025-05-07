using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fly.Data;
using fly.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Blazorise;

namespace fly.Controllers
{
    public class ExhibitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ExhibitsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Exhibits
        //[Authorize(Roles = "IT, Warehouse, Administration")]
        public async Task<IActionResult> Index(int? categoryId)
        {
            if (categoryId == null)
            {
                var allExhibit = await _context.Exhibit.Include(c => c.Category).ToListAsync();
                return View(allExhibit);
            }
            else
            {
                var modelsForCategory = await _context.Exhibit
                    .Where(m => m.CategoryId == categoryId)
                    .Include(c => c.Category)
                    .ToListAsync();
                ViewBag.CategoryId = categoryId;
                ViewBag.CategoryName = _context.Category.FirstOrDefault(b => b.CategoryId == categoryId)?.Name;
                return View(modelsForCategory);
            }
        }

        // GET: Exhibits/Details/5
        //[Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Details(int? id, int? categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ExhibitModel = await _context.Exhibit
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.ExhibitId == id);
            if (ExhibitModel == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = categoryId;
            return View(ExhibitModel);
        }

        // GET: Exhibits/Create
        //[Authorize(Roles = "IT, Warehouse")]
        public IActionResult Create(int? categoryId)
        {
            ViewBag.CategoryId = categoryId;
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", categoryId);
            return View();
        }

        // POST: Exhibits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Create([Bind("ExhibitId,CategoryId,ExhibitName,ExhibitDescription,CreationDate,Material,Size,Weight,LogoPath")] Exhibit exhibit, IFormFile logoFile)
        {
            if (ModelState.IsValid)
            {
                // Проверка на существование экспоната с таким же названием 
                var existingExhibitModel = await _context.Exhibit
                    .FirstOrDefaultAsync(cm => cm.ExhibitName == exhibit.ExhibitName  && cm.CategoryId == exhibit.CategoryId);
                if (existingExhibitModel != null)
                {
                    ModelState.AddModelError("Name", "Модель с таким названием и годом выпуска уже существует.");
                    ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", exhibit.CategoryId);
                    return View(exhibit);
                }

                if (logoFile != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "exhibit");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + logoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(fileStream);
                    }
                    exhibit.LogoPath = "/images/exhibit/" + uniqueFileName;
                }

                _context.Add(exhibit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { categoryId = exhibit.CategoryId });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", exhibit.CategoryId);
            return View(exhibit);
        }

        // GET: Exhibits/Edit/5
        //[Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Edit(int? id, int? categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ExhibitModel = await _context.Exhibit.FindAsync(id);
            if (ExhibitModel == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = categoryId;
            ViewData["CategoryList"] = new SelectList(_context.Exhibit, "BrandId", "Name", ExhibitModel.CategoryId);
            return View(ExhibitModel);
        }

        // POST: Exhibits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Edit(int id, [Bind("ExhibitId,MuseumId,ExhibitName,ExhibitDescription,CreationDate,Material,Size,Weight")] Exhibit exhibit, IFormFile logoFile)
        {
            if (id != exhibit.ExhibitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Проверка на существование экспоната с таким же названием 
                    var existingExhibitModel = await _context.Exhibit
                        .FirstOrDefaultAsync(cm => cm.ExhibitName == exhibit.ExhibitName && cm.CategoryId == exhibit.CategoryId);
                    if (existingExhibitModel != null)
                    {
                        ModelState.AddModelError("Name", "Модель с таким названием и годом выпуска уже существует.");
                        ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", exhibit.CategoryId);
                        return View(exhibit);
                    }

                    if (logoFile != null)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "exhibit");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + logoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(fileStream);
                        }
                        exhibit.LogoPath = "/images/exhibit/" + uniqueFileName;
                    }

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
                return RedirectToAction(nameof(Index), new { categoryId = exhibit.CategoryId });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", exhibit.CategoryId);
            return View(exhibit);
        }

        // GET: Exhibits/Delete/5
        //[Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Delete(int? id, int? categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibit = await _context.Exhibit
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.ExhibitId == id);
            if (exhibit == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = categoryId;
            return View(exhibit);
        }

        // POST: Exhibits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, int? categoryId)
        {
            var exhibit = await _context.Exhibit.FindAsync(id);
            if (exhibit != null)
            {
                _context.Exhibit.Remove(exhibit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {id = exhibit.CategoryId });
        }

        private bool ExhibitExists(int id)
        {
            return _context.Exhibit.Any(e => e.ExhibitId == id);
        }
    }
}
