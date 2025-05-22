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

namespace fly.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Categories
        // [Authorize(Roles = "IT, Warehouse, Administration")]
        public async Task<IActionResult> Index()
        {
            // Важно: добавляем Include, чтобы подгрузить экспонаты и избежать null в item.Exhibit
            var categories = await _context.Categorys
                .Include(c => c.Exhibit) // <-- добавлено
                .ToListAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        // [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Также Include, если хотите видеть экспонаты в Details
            var category = await _context.Categorys
                .Include(c => c.Exhibit)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        // [Authorize(Roles = "IT, Warehouse")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Categorys
                    .FirstOrDefaultAsync(b => b.Name == category.Name);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Категория с таким названием уже существует.");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        // [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categorys.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = await _context.Categorys
                        .FirstOrDefaultAsync(b => b.Name == category.Name && b.CategoryId != category.CategoryId);
                    if (existingCategory != null)
                    {
                        ModelState.AddModelError("Name", "Категория с таким названием уже существует.");
                        return View(category);
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        // [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categorys
                .Include(c => c.Exhibit) // <-- чтобы возможно выводить количество экспонатов при удалении
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "IT, Warehouse")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // !!! Изменено: удаляем категорию только если нет экспонатов !!!
            var category = await _context.Categorys
                .Include(c => c.Exhibit)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                if (category.Exhibit != null && category.Exhibit.Any())
                {
                    ModelState.AddModelError(string.Empty, "Нельзя удалить категорию, в которой есть экспонаты.");
                    return View("Delete", category);
                }
                _context.Categorys.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categorys.Any(e => e.CategoryId == id);
        }
    }
}