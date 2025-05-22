using fly.Data;
using fly.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Text;
using System.Threading.Tasks;

namespace Museum.Controllers
{
    public class MovementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var movements = await _context.Movements
                .Include(m => m.Exhibit)
                .Include(m => m.FromStorageLocation)
                .Include(m => m.ToStorageLocation)
                .ToListAsync();
            return View(movements);
        }

       
        public IActionResult Create()
        {
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName");
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name");
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name");
            return View();
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovementId,ExhibitId,FromStorageLocationId,ToStorageLocationId,MovementDate,Notes")] Movement movement)
        {
            Console.WriteLine($"Received POST request for Movement: ExhibitId={movement.ExhibitId}, FromStorageLocationId={movement.FromStorageLocationId}, ToStorageLocationId={movement.ToStorageLocationId}, MovementDate={movement.MovementDate}, Notes={movement.Notes}");

            if (ModelState.IsValid)
            {
                var exhibitExists = await _context.Exhibit.AnyAsync(e => e.ExhibitId == movement.ExhibitId);
                var fromLocationExists = await _context.StorageLocations.AnyAsync(sl => sl.StorageLocationId == movement.FromStorageLocationId);
                var toLocationExists = await _context.StorageLocations.AnyAsync(sl => sl.StorageLocationId == movement.ToStorageLocationId);

                if (!exhibitExists || !fromLocationExists || !toLocationExists)
                {
                    ModelState.AddModelError("", "Одна или более связанных сущностей не найдены.");
                    ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", movement.ExhibitId);
                    ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.FromStorageLocationId);
                    ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.ToStorageLocationId);
                    return View(movement);
                }

                Console.WriteLine("Model state is valid, saving to database...");
                _context.Add(movement);
                await _context.SaveChangesAsync();
                Console.WriteLine("Save successful, redirecting to Index...");
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("Model state is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", movement.ExhibitId);
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.FromStorageLocationId);
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.ToStorageLocationId);
            return View(movement);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null) return NotFound();
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", movement.ExhibitId);
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.FromStorageLocationId);
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.ToStorageLocationId);
            return View(movement);
        }

        [HttpPost]
     
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovementId,ExhibitId,FromStorageLocationId,ToStorageLocationId,MovementDate,Notes")] Movement movement)
        {
            if (id != movement.MovementId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Movements.Any(e => e.MovementId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExhibitId"] = new SelectList(_context.Exhibit, "ExhibitId", "ExhibitName", movement.ExhibitId);
            ViewData["FromStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.FromStorageLocationId);
            ViewData["ToStorageLocationId"] = new SelectList(_context.StorageLocations, "StorageLocationId", "Name", movement.ToStorageLocationId);
            return View(movement);
        }

       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var movement = await _context.Movements
                .Include(m => m.Exhibit)
                .Include(m => m.FromStorageLocation)
                .Include(m => m.ToStorageLocation)
                .FirstOrDefaultAsync(m => m.MovementId == id);
            if (movement == null) return NotFound();
            return View(movement);
        }

        [HttpPost, ActionName("Delete")]
        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null)
            {
                return NotFound();
            }
            _context.Movements.Remove(movement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> GenerateReport(int id)
        {
            try
            {
                var movement = await _context.Movements
                    .Include(m => m.Exhibit)
                    .Include(m => m.FromStorageLocation)
                    .Include(m => m.ToStorageLocation)
                    .FirstOrDefaultAsync(m => m.MovementId == id);

                if (movement == null)
                {
                    Console.WriteLine($"Movement with ID {id} not found.");
                    return NotFound();
                }

                // Создаем новый PDF-документ
                PdfDocument document = new PdfDocument();
                document.Info.Title = $"Отчет о перемещении #{movement.MovementId}";
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Определяем шрифты
                XFont titleFont = new XFont("Arial", 20, XFontStyle.Bold);
                XFont sectionFont = new XFont("Arial", 14, XFontStyle.Bold);
                XFont regularFont = new XFont("Arial", 12, XFontStyle.Regular);

                // Определяем цвета и кисти
                XBrush blackBrush = XBrushes.Black;
                XPen borderPen = new XPen(XColors.Black, 1);

                // Рисуем рамку по краям страницы
                double margin = 40;
                gfx.DrawRectangle(borderPen, margin, margin, page.Width - 2 * margin, page.Height - 2 * margin);

                // Заголовок
                string title = "Отчет о перемещении экспоната";
                double titleHeight = titleFont.GetHeight();
                gfx.DrawString(title, titleFont, blackBrush, new XRect(0, margin, page.Width, titleHeight), XStringFormats.TopCenter);

                // Логотип или графический элемент (в виде текста)
                string museumName = "Музей истории";
                XFont museumFont = new XFont("Arial", 10, XFontStyle.Italic);
                gfx.DrawString(museumName, museumFont, XBrushes.Gray, new XRect(0, margin + titleHeight + 5, page.Width, museumFont.GetHeight()), XStringFormats.TopCenter);

                // Секция: Общая информация
                double yPosition = margin + titleHeight + 30;
                gfx.DrawString("Общая информация", sectionFont, blackBrush, new XRect(margin + 10, yPosition, page.Width, sectionFont.GetHeight()), XStringFormats.TopLeft);
                yPosition += sectionFont.GetHeight() + 5;

                // Данные отчета
                var reportLines = new[]
                {
                    $"Идентификатор перемещения: #{movement.MovementId}",
                    $"Дата генерации: {DateTime.Now:dd.MM.yyyy HH:mm}",
                    $"Экспонат: {movement.Exhibit?.ExhibitName ?? "Неизвестно"}",
                    $"Дата перемещения: {movement.MovementDate:dd.MM.yyyy HH:mm}",
                    $"Откуда: {movement.FromStorageLocation?.Name ?? "Неизвестно"}",
                    $"Куда: {movement.ToStorageLocation?.Name ?? "Неизвестно"}",
                    $"Примечания: {movement.Notes ?? "Нет примечаний"}"
                };

                // Выводим данные построчно
                foreach (var line in reportLines)
                {
                    string encodedLine = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(line));
                    gfx.DrawString(encodedLine, regularFont, blackBrush, new XRect(margin + 20, yPosition, page.Width - 2 * margin - 20, regularFont.GetHeight()), XStringFormats.TopLeft);
                    yPosition += regularFont.GetHeight() + 5;
                }

                // Добавляем подпись внизу
                string signature = "Документ сгенерирован автоматически системой управления музеем.";
                gfx.DrawString(signature, regularFont, XBrushes.Gray, new XRect(margin, page.Height - margin - regularFont.GetHeight(), page.Width - 2 * margin, regularFont.GetHeight()), XStringFormats.TopLeft);

                // Сохраняем PDF в память
                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    byte[] pdfBytes = stream.ToArray();

                    // Возвращаем PDF как файл для скачивания
                    return File(pdfBytes, "application/pdf", $"Отчет_о_перемещении_{movement.MovementId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF report: {ex.Message}");
                return StatusCode(500, "Ошибка при генерации PDF-отчета. Подробности в логах.");
            }
        }
    }
}