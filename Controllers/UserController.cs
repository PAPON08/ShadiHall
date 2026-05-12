using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShadiHall.Models;
using ShadiHall.Services;
using ShadiHall.ViewModels;

namespace ShadiHall.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;

        public UserController(UserManager<ApplicationUser> userManager, IReservationService reservationService)
        {
            _userManager = userManager;
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var reservations = await _reservationService.GetUserReservationsAsync(user.Id);

            var vm = new UserDashboardViewModel
            {
                User = user,
                Reservations = reservations,
                TotalReservations = reservations.Count,
                ActiveReservations = reservations.Count(r => r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending),
                CancelledReservations = reservations.Count(r => r.Status == ReservationStatus.Cancelled)
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            ViewBag.User = user;
            return View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(string fullName, string address, string? phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FullName = fullName;
            user.Address = address;
            user.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                TempData["Success"] = "Profile updated successfully.";
            else
                TempData["Error"] = "Failed to update profile.";

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully.";
                return RedirectToAction(nameof(Dashboard));
            }

            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View();
        }
    }
}
