using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DeliverySystem.Helpers;

namespace DeliverySystem.Pages.Courier
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Order> Orders { get; set; }
        public IList<ApplicationUser> Couriers { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            if (User.IsInRole("Administrator"))
            {
                Orders = await _context.Orders
                    .Include(o => o.Fruit)
                    .Include(o => o.User)
                    .Include(o => o.Delivery).ThenInclude(d => d.Courier)
                    .ToListAsync();
            }
            else if (User.IsInRole("Courier"))
            {
                Orders = await _context.Orders
                    .Include(o => o.Fruit)
                    .Include(o => o.User)
                    .Include(o => o.Delivery).ThenInclude(d => d.Courier)
                    .Where(o => o.Delivery == null || o.Delivery.UserId == userId)
                    .ToListAsync();
            }

            Couriers = await _userManager.GetUsersInRoleAsync("Courier");
        }

        public async Task<IActionResult> OnPostUpdateCourierAsync(int id, string courierId)
        {
            var order = await _context.Orders.Include(o => o.Delivery).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var courier = await _userManager.FindByIdAsync(courierId);
            if (courier == null)
            {
                return BadRequest("Courier not found.");
            }

            if (User.IsInRole("Administrator") || (order.Delivery == null || order.Delivery.UserId == (await _userManager.GetUserAsync(User)).Id))
            {
                order.Delivery.UserId = courierId;
                order.Delivery.DeliveryDate = DateTime.Now;
                _context.Deliveries.Update(order.Delivery);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMarkAsDeliveredAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.Delivery).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            if (order.Delivery != null && order.Delivery.UserId == userId)
            {
                order.IsDelivered = true;
                _context.Orders.Update(order);

                TwilioHelper.SendMessage("Order has been delivered", "+359885305612");

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
