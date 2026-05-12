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
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHallService _hallService;
        private readonly IReservationService _reservationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, IHallService hallService,
            IReservationService reservationService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hallService = hallService;
            _reservationService = reservationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalHalls = await _context.Halls.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalReservations = await _context.Reservations.CountAsync(),
                PendingReservations = await _context.Reservations.CountAsync(r => r.Status == ReservationStatus.Pending),
                ConfirmedReservations = await _context.Reservations.CountAsync(r => r.Status == ReservationStatus.Confirmed),
                UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsRead),
                TotalRevenue = await _context.Payments.Where(p => p.Status == "Completed").SumAsync(p => p.Amount),
                RecentReservations = await _context.Reservations.Include(r => r.Hall).Include(r => r.User)
                    .OrderByDescending(r => r.CreatedAt).Take(10).ToListAsync(),
                RecentMessages = await _context.ContactMessages.OrderByDescending(m => m.SentAt).Take(5).ToListAsync()
            };
            return View(vm);
        }

        // ── Halls ────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Halls()
        {
            var halls = await _context.Halls.OrderByDescending(h => h.CreatedAt).ToListAsync();
            return View(halls);
        }

        [HttpGet]
        public IActionResult CreateHall() => View(new HallFormViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHall(HallFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _hallService.CreateHallAsync(model);
            TempData["Success"] = "Hall created successfully.";
            return RedirectToAction(nameof(Halls));
        }

        [HttpGet]
        public async Task<IActionResult> EditHall(int id)
        {
            var hall = await _hallService.GetHallByIdAsync(id);
            if (hall == null) return NotFound();

            return View(new HallFormViewModel
            {
                HallId = hall.HallId,
                Name = hall.Name,
                Description = hall.Description,
                ShortDescription = hall.ShortDescription,
                PricePerDay = hall.PricePerDay,
                Capacity = hall.Capacity,
                Location = hall.Location,
                Amenities = hall.Amenities,
                IsAvailable = hall.IsAvailable,
                IsFeatured = hall.IsFeatured,
                ExistingImageUrl = hall.ImageUrl
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHall(HallFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _hallService.UpdateHallAsync(model);
            TempData["Success"] = "Hall updated successfully.";
            return RedirectToAction(nameof(Halls));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHall(int id)
        {
            await _hallService.DeleteHallAsync(id);
            TempData["Success"] = "Hall deleted.";
            return RedirectToAction(nameof(Halls));
        }

        // ── Reservations ─────────────────────────────────────────────────────────
        public async Task<IActionResult> Reservations(string? status)
        {
            var query = _context.Reservations.Include(r => r.Hall).Include(r => r.User).AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReservationStatus>(status, out var s))
                query = query.Where(r => r.Status == s);

            ViewBag.CurrentStatus = status;
            var list = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReservationStatus(int id, ReservationStatus status)
        {
            await _reservationService.UpdateStatusAsync(id, status);
            TempData["Success"] = $"Reservation status updated to {status}.";
            return RedirectToAction(nameof(Reservations));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var r = await _context.Reservations.FindAsync(id);
            if (r != null) { _context.Reservations.Remove(r); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Reservation deleted.";
            return RedirectToAction(nameof(Reservations));
        }

        // ── Users ────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var u in users)
                userRoles[u.Id] = await _userManager.GetRolesAsync(u);

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        // ── Messages ─────────────────────────────────────────────────────────────
        public async Task<IActionResult> Messages()
        {
            var messages = await _context.ContactMessages.OrderByDescending(m => m.SentAt).ToListAsync();
            return View(messages);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkMessageRead(int id)
        {
            var m = await _context.ContactMessages.FindAsync(id);
            if (m != null) { m.IsRead = true; await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Messages));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var m = await _context.ContactMessages.FindAsync(id);
            if (m != null) { _context.ContactMessages.Remove(m); await _context.SaveChangesAsync(); }
            TempData["Success"] = "Message deleted.";
            return RedirectToAction(nameof(Messages));
        }
    }
}
