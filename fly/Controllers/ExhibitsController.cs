using System;
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
        public async Task<IActionResult> Index(int? categoryId, string searchString)
        {
            ViewBag.Categories = new SelectList(_context.Categorys, "CategoryId", "Name", categoryId);

            var exhibits = _context.Exhibit
                .Include(e => e.Category)
                .Include(e => e.StorageLocation)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value != 0)
            {
                exhibits = exhibits.Where(e => e.CategoryId == categoryId);
                ViewBag.CategoryId = categoryId;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                exhibits = exhibits.Where(e => e.ExhibitName.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            return View(await exhibits.ToListAsync());
        }

        // GET: Exhibits/Details/5
        public async Task<IActionResult> Details(int? id, int? categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ExhibitModel = await _context.Exhibit
                .Include(c => c.Category)
                .Include(e => e.StorageLocation)
                .FirstOrDefaultAsync(m => m.ExhibitId == id);
            if (ExhibitModel == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = categoryId;
            return View(ExhibitModel);
        }

        public IActionResult Create(int? categoryId)
        {
            ViewBag.CategoryId = categoryId;
            ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "Name", categoryId);
            ViewData["StorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name");

            return View();
        }

        // POST: Exhibits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExhibitId,CategoryId,StorageLocationId,ExhibitName,ExhibitDescription,CreationDate,Material,Size,Weight,LogoPath")] Exhibit exhibit, IFormFile logoFile)
        {
            if (ModelState.IsValid)
            {
                var existingExhibit = await _context.Exhibit
                    .FirstOrDefaultAsync(e => e.ExhibitName == exhibit.ExhibitName && e.CategoryId == exhibit.CategoryId);
                if (existingExhibit != null)
                {
                    ModelState.AddModelError("ExhibitName", "Экспонат с таким названием уже существует в данной категории.");
                    ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "Name", exhibit.CategoryId);
                    ViewData["StorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", exhibit.StorageLocationId);
                    return View(exhibit);
                }

                if (logoFile != null && logoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "exhibit");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(logoFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(fileStream);
                    }
                    exhibit.LogoPath = "/images/exhibit/" + uniqueFileName;
                }

                _context.Add(exhibit);
                await _context.SaveChangesAsync();

                // === ДОБАВЛЕНИЕ В РЕГИСТР НАКОПЛЕНИЯ (Inventory) ===
                var inventory = new Inventory
                {
                    ExhibitId = exhibit.ExhibitId,
                    ExhibitName = exhibit.ExhibitName,
                    поступления = DateTime.Now,
                    списания = null
                };
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                // === КОНЕЦ ДОБАВЛЕНИЯ ===

                return RedirectToAction(nameof(Index), new { categoryId = exhibit.CategoryId });
            }
            ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "Name", exhibit.CategoryId);
            ViewData["StorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", exhibit.StorageLocationId);

            return View(exhibit);
        }

        // GET: Exhibits/Edit/5
        public async Task<IActionResult> Edit(int? id, int? categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ExhibitModel = await _context.Exhibit
                .Include(e => e.StorageLocation)
                .FirstOrDefaultAsync(e => e.ExhibitId == id);
            if (ExhibitModel == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = categoryId;
            ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "Name", ExhibitModel.CategoryId);
            ViewData["StorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", ExhibitModel.StorageLocationId);

            // Для отслеживания перемещений: сохраним старое значение StorageLocationId
            ViewBag.OldStorageLocationId = ExhibitModel.StorageLocationId;

            return View(ExhibitModel);
        }

        // POST: Exhibits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExhibitId,CategoryId,StorageLocationId,ExhibitName,ExhibitDescription,CreationDate,Material,Size,Weight,LogoPath")] Exhibit exhibit, IFormFile logoFile, int? oldStorageLocationId)
        {
            if (id != exhibit.ExhibitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingExhibitModel = await _context.Exhibit
                        .FirstOrDefaultAsync(cm => cm.ExhibitName == exhibit.ExhibitName && cm.CategoryId == exhibit.CategoryId && cm.ExhibitId != exhibit.ExhibitId);
                    if (existingExhibitModel != null)
                    {
                        ModelState.AddModelError("Name", "Модель с таким названием и годом выпуска уже существует.");
                        ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "Name", exhibit.CategoryId);
                        ViewData["StorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", exhibit.StorageLocationId);
                        return View(exhibit);
                    }

                    // Получение старого значения StorageLocationId для отслеживания перемещений
                    int oldStorageLocation;
                    if (oldStorageLocationId.HasValue)
                    {
                        oldStorageLocation = oldStorageLocationId.Value;
                    }
                    else
                    {
                        oldStorageLocation = await _context.Exhibit.AsNoTracking()
                            .Where(e => e.ExhibitId == id)
                            .Select(e => e.StorageLocationId)
                            .FirstOrDefaultAsync();
                    }

                    if (logoFile != null && logoFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "exhibit");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(logoFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(fileStream);
                        }
                        exhibit.LogoPath = "/images/exhibit/" + uniqueFileName;
                    }
                    else
                    {
                        // Не затираем логотип, если не выбран новый файл
                        var oldExhibit = await _context.Exhibit.AsNoTracking().FirstOrDefaultAsync(e => e.ExhibitId == id);
                        if (oldExhibit != null)
                            exhibit.LogoPath = oldExhibit.LogoPath;
                    }

                    _context.Update(exhibit);
                    await _context.SaveChangesAsync();

                    // === Добавление записи о перемещении, если изменено место хранения ===
                    if (oldStorageLocation != exhibit.StorageLocationId)
                    {
                        var movement = new Movement
                        {
                            ExhibitId = exhibit.ExhibitId,
                            FromStorageLocationId = oldStorageLocation,
                            ToStorageLocationId = exhibit.StorageLocationId,
                            MovementDate = DateTime.Now
                        };
                        _context.Movements.Add(movement);
                        await _context.SaveChangesAsync();
                    }
                    // === Конец добавления ===
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
            ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "Name", exhibit.CategoryId);
            ViewData["StorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", exhibit.StorageLocationId);

            return View(exhibit);
        }

        // GET: Exhibits/Delete/5
        public async Task<IActionResult> Delete(int? id, int? categoryId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibit = await _context.Exhibit
                .Include(e => e.Category)
                .Include(e => e.StorageLocation)
                .FirstOrDefaultAsync(m => m.ExhibitId == id);
            if (exhibit == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = categoryId;
            ViewData["CategoryId"] = new SelectList(_context.Categorys, "CategoryId", "Name", exhibit.CategoryId);
            ViewData["StorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", exhibit.StorageLocationId);

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
                var inventory = new Inventory
                {
                    ExhibitId = exhibit.ExhibitId,
                    ExhibitName = exhibit.ExhibitName,
                    поступления = null,
                    списания = DateTime.Now
                };
                _context.Inventories.Add(inventory);

                _context.Exhibit.Remove(exhibit);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { categoryId = exhibit?.CategoryId });
        }

        private bool ExhibitExists(int id)
        {
            return _context.Exhibit.Any(e => e.ExhibitId == id);
        }
    }
}