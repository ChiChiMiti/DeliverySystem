using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Fruits.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,ImageUrl")] Fruit fruit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fruit);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(fruit);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fruit = await _context.Fruits.FindAsync(id);

            if (fruit == null)
            {
                return NotFound();
            }

            return View(fruit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Stock,ImageUrl")] Fruit fruit)
        {
            if (id != fruit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fruit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FruitExists(fruit.Id))
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

            return View(fruit);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fruit = await _context.Fruits
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fruit == null)
            {
                return NotFound();
            }

            return View(fruit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fruit = await _context.Fruits.FindAsync(id);
            _context.Fruits.Remove(fruit);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FruitExists(int id)
        {
            return _context.Fruits.Any(e => e.Id == id);
        }
    }
}
