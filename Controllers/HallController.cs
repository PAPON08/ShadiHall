using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShadiHall.Data;
using ShadiHall.Models;
using ShadiHall.Services;
using ShadiHall.ViewModels;

namespace ShadiHall.Controllers
{
    public class HallController : Controller
    {
        private readonly IHallService _hallService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HallController(IHallService hallService, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _hallService = hallService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, decimal? minPrice, decimal? maxPrice, int? minCapacity, int page = 1)
        {
            var vm = await _hallService.GetHallsAsync(search, minPrice, maxPrice, minCapacity, page);
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hall = await _hallService.GetHallByIdAsync(id);
            if (hall == null) return NotFound();

            var reviews = await _context.Reviews.Include(r => r.User)
                .Where(r => r.HallId == id && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt).ToListAsync();

            string? currentUserId = _userManager.GetUserId(User);
            bool hasReviewed = currentUserId != null && reviews.Any(r => r.UserId == currentUserId);

            var vm = new HallDetailsViewModel
            {
                Hall = hall,
                Reviews = reviews,
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                ReviewCount = reviews.Count,
                UserHasReviewed = hasReviewed,
                ReviewForm = new ReviewFormViewModel { HallId = id }
            };

            return View(vm);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(ReviewFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill all review fields correctly.";
                return RedirectToAction(nameof(Details), new { id = model.HallId });
            }

            var userId = _userManager.GetUserId(User)!;
            var exists = await _context.Reviews.AnyAsync(r => r.HallId == model.HallId && r.UserId == userId);
            if (exists)
            {
                TempData["Error"] = "You have already reviewed this hall.";
                return RedirectToAction(nameof(Details), new { id = model.HallId });
            }

            _context.Reviews.Add(new Review
            {
                UserId = userId,
                HallId = model.HallId,
                Rating = model.Rating,
                Comment = model.Comment
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thank you for your review!";
            return RedirectToAction(nameof(Details), new { id = model.HallId });
        }
    }
}
