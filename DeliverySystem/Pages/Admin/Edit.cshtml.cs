using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Pages.Admin
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Fruit Fruit { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Fruit = await _context.Fruits.FindAsync(id);

            if (Fruit == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fruitToUpdate = await _context.Fruits.FindAsync(Fruit.Id);

            if (fruitToUpdate == null)
            {
                return NotFound();
            }

            fruitToUpdate.Name = Fruit.Name;
            fruitToUpdate.Price = Fruit.Price;
            fruitToUpdate.Stock = Fruit.Stock;

            _context.Attach(fruitToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FruitExists(Fruit.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool FruitExists(int id)
        {
            return _context.Fruits.Any(e => e.Id == id);
        }
    }
}
