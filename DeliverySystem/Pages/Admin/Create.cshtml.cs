using DeliverySystem.Data.Models;
using DeliverySystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliverySystem.Pages.Admin
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Fruit Fruit { get; set; }

        [BindProperty]
        public IFormFile Image { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Image != null)
            {
                var fileName = Path.GetFileName(Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                Fruit.ImageUrl = $"/images/{fileName}";
            }

            _context.Fruits.Add(Fruit);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
