using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DeliverySystem.Helpers;

namespace DeliverySystem.Pages
{
    [Authorize]
    public class PurchaseModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PurchaseModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Fruit Fruit { get; set; }

        [BindProperty]
        public Order Order { get; set; }

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
            var fruit = await _context.Fruits.FindAsync(Order.FruitId);

            if (fruit == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Order.UserId = userId;
            Order.FinalPrice = fruit.Price * Order.Amount;
            Order.OrderDate = DateTime.Now;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Orders.Add(Order);
                    await _context.SaveChangesAsync();

                    fruit.Stock -= Order.Amount;
                    _context.Fruits.Update(fruit);
                    await _context.SaveChangesAsync();

                    var couriers = await _userManager.GetUsersInRoleAsync("Courier");

                    if (couriers.Count > 0)
                    {
                        var randomCourier = couriers[new Random().Next(couriers.Count)];
                        var delivery = new Delivery
                        {
                            OrderId = Order.Id,
                            UserId = randomCourier.Id,
                            IsDelivered = false
                        };

                        _context.Deliveries.Add(delivery);
                        await _context.SaveChangesAsync();

                        TwilioHelper.SendMessage("Order was received and it will be delivered soon", "+359885305612");
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return RedirectToPage("/Index");
        }
    }
}
