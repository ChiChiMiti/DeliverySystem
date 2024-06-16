using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Fruit> Fruits { get; set; }

        public async Task OnGetAsync()
        {
            Fruits = await _context.Fruits.ToListAsync();
        }
    }
}