using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShadiHall.Data;
using ShadiHall.Models;
using ShadiHall.Services;
using ShadiHall.ViewModels;

namespace ShadiHall.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IHallService _hallService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ReservationController(IReservationService reservationService, IHallService hallService,
            UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _reservationService = reservationService;
            _hallService = hallService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int hallId)
        {
            var hall = await _hallService.GetHallByIdAsync(hallId);
            if (hall == null || !hall.IsAvailable) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var vm = new ReservationCreateViewModel
            {
                HallId = hallId,
                Hall = hall,
                GuestName = user?.FullName ?? "",
                GuestEmail = user?.Email ?? "",
                GuestPhone = user?.PhoneNumber ?? "",
                GuestAddress = user?.Address ?? ""
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationCreateViewModel model)
        {
            var hall = await _hallService.GetHallByIdAsync(model.HallId);
            model.Hall = hall;

            if (!ModelState.IsValid) return View(model);

            if (model.EventDate.Date < DateTime.Today.AddDays(1))
            {
                ModelState.AddModelError("EventDate", "Event date must be at least 1 day from today.");
                return View(model);
            }

            if (!await _reservationService.IsHallAvailable(model.HallId, model.EventDate, model.TimeSlot))
            {
                ModelState.AddModelError("", "This hall is already booked for the selected date and time slot.");
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;
            var reservation = await _reservationService.CreateAsync(model, userId);

            return RedirectToAction(nameof(Payment), new { id = reservation.ReservationId });
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (reservation.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            return View(new PaymentViewModel
            {
                Reservation = reservation,
                Hall = reservation.Hall!
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            var reservation = await _reservationService.GetByIdAsync(model.Reservation.ReservationId);
            if (reservation == null) return NotFound();

            await _reservationService.ProcessPaymentAsync(reservation.ReservationId, model.PaymentMethod, model.TransactionRef);
            await _reservationService.UpdateStatusAsync(reservation.ReservationId, ReservationStatus.Confirmed);

            return RedirectToAction(nameof(Confirmation), new { id = reservation.ReservationId });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (reservation.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            return View(new ReservationConfirmationViewModel
            {
                Reservation = reservation,
                Hall = reservation.Hall!,
                Payment = reservation.Payment
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (reservation.UserId != userId) return Forbid();

            if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Completed)
            {
                TempData["Error"] = "This reservation cannot be edited.";
                return RedirectToAction("Dashboard", "User");
            }

            var vm = new ReservationCreateViewModel
            {
                HallId = reservation.HallId,
                Hall = reservation.Hall,
                EventDate = reservation.EventDate,
                TimeSlot = reservation.TimeSlot,
                GuestName = reservation.GuestName,
                GuestEmail = reservation.GuestEmail,
                GuestPhone = reservation.GuestPhone,
                GuestAddress = reservation.GuestAddress,
                EventType = reservation.EventType,
                GuestCount = reservation.GuestCount,
                SpecialRequests = reservation.SpecialRequests
            };

            ViewBag.ReservationId = id;
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int reservationId, ReservationCreateViewModel model)
        {
            var reservation = await _reservationService.GetByIdAsync(reservationId);
            if (reservation == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (reservation.UserId != userId) return Forbid();

            if (!ModelState.IsValid)
            {
                model.Hall = await _hallService.GetHallByIdAsync(model.HallId);
                ViewBag.ReservationId = reservationId;
                return View(model);
            }

            reservation.EventDate = model.EventDate;
            reservation.TimeSlot = model.TimeSlot;
            reservation.GuestName = model.GuestName;
            reservation.GuestEmail = model.GuestEmail;
            reservation.GuestPhone = model.GuestPhone;
            reservation.GuestAddress = model.GuestAddress;
            reservation.EventType = model.EventType;
            reservation.GuestCount = model.GuestCount;
            reservation.SpecialRequests = model.SpecialRequests;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Reservation updated successfully.";
            return RedirectToAction("Dashboard", "User");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var success = await _reservationService.CancelAsync(id, userId);
            TempData[success ? "Success" : "Error"] = success ? "Reservation cancelled successfully." : "Unable to cancel reservation.";
            return RedirectToAction("Dashboard", "User");
        }
    }
}
