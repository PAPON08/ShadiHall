using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShadiHall.Data;
using ShadiHall.Models;
using ShadiHall.Services;
using ShadiHall.ViewModels;

namespace ShadiHall.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHallService _hallService;

        public HomeController(ApplicationDbContext context, IHallService hallService)
        {
            _context = context;
            _hallService = hallService;
        }

        public async Task<IActionResult> Index()
        {
            var banners = await _context.BannerSlides.Where(b => b.IsActive).OrderBy(b => b.SortOrder).ToListAsync();
            var featured = await _hallService.GetFeaturedHallsAsync();
            var recentReviews = await _context.Reviews.Include(r => r.User).Include(r => r.Hall)
                .Where(r => r.IsApproved).OrderByDescending(r => r.CreatedAt).Take(6).ToListAsync();

            ViewBag.Banners = banners;
            ViewBag.FeaturedHalls = featured;
            ViewBag.RecentReviews = recentReviews;
            return View();
        }

        public IActionResult About() => View();

        public async Task<IActionResult> Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.ContactMessages.Add(new ContactMessage
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Subject = model.Subject,
                Message = model.Message
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your message has been sent! We'll get back to you soon.";
            return RedirectToAction(nameof(Contact));
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
