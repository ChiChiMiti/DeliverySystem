using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DeliverySystem.Pages.Orders
{
    public class MyOrdersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyOrdersModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Delivery> Deliveries { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            Deliveries = await _context.Deliveries
                .Include(o => o.Order)
                .Include(o=>o.Order.Fruit)
                .Include(o=>o.Courier)
                .Where(o => o.Order.UserId == userId)
                .ToListAsync();

            return Page();
        }
    }
}
