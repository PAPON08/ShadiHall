using System.ComponentModel.DataAnnotations;
using ShadiHall.Models;

namespace ShadiHall.ViewModels
{
    // Auth ViewModels
    public class RegisterViewModel
    {
        [Required, MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must have uppercase, lowercase, digit, and special character.")]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }

    public class LoginViewModel
    {
        [Required, EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    // Hall ViewModels
    public class HallListViewModel
    {
        public List<Hall> Halls { get; set; } = new();
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinCapacity { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    public class HallDetailsViewModel
    {
        public Hall Hall { get; set; } = null!;
        public List<Review> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool UserHasReviewed { get; set; }
        public ReviewFormViewModel ReviewForm { get; set; } = new();
    }

    public class HallFormViewModel
    {
        public int HallId { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Hall Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Description")]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; } = string.Empty;

        [Required, Range(1000, 10000000)]
        [Display(Name = "Price Per Day (BDT)")]
        public decimal PricePerDay { get; set; }

        [Required, Range(10, 10000)]
        [Display(Name = "Capacity (persons)")]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Location / Wing")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Amenities (comma-separated)")]
        public string Amenities { get; set; } = string.Empty;

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Featured Hall")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Hall Image")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
    }

    // Reservation ViewModels
    public class ReservationCreateViewModel
    {
        public int HallId { get; set; }
        public Hall? Hall { get; set; }

        [Required]
        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today.AddDays(7);

        [Required]
        [Display(Name = "Time Slot")]
        public string TimeSlot { get; set; } = "12hr"; // "6hr-morning", "6hr-evening", "12hr"

        [Required, MaxLength(100)]
        [Display(Name = "Guest Name")]
        public string GuestName { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string GuestEmail { get; set; } = string.Empty;

        [Required, Phone]
        [Display(Name = "Phone Number")]
        public string GuestPhone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address")]
        public string GuestAddress { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "Event Type")]
        public string EventType { get; set; } = string.Empty;

        [Required, Range(10, 10000)]
        [Display(Name = "Expected Guest Count")]
        public int GuestCount { get; set; }

        [MaxLength(500)]
        [Display(Name = "Special Requests")]
        public string? SpecialRequests { get; set; }
    }

    public class PaymentViewModel
    {
        public Reservation Reservation { get; set; } = null!;
        public Hall Hall { get; set; } = null!;

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Cash";

        [Display(Name = "Transaction / Reference Number")]
        public string? TransactionRef { get; set; }
    }

    public class ReservationConfirmationViewModel
    {
        public Reservation Reservation { get; set; } = null!;
        public Hall Hall { get; set; } = null!;
        public Payment? Payment { get; set; }
    }

    // Review ViewModel
    public class ReviewFormViewModel
    {
        public int HallId { get; set; }

        [Required, Range(1, 5)]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [Required, MinLength(10), MaxLength(1000)]
        [Display(Name = "Your Review")]
        public string Comment { get; set; } = string.Empty;
    }

    // Contact ViewModel
    public class ContactViewModel
    {
        [Required, MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required, MinLength(20)]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;
    }

    // Admin Dashboard ViewModel
    public class AdminDashboardViewModel
    {
        public int TotalHalls { get; set; }
        public int TotalUsers { get; set; }
        public int TotalReservations { get; set; }
        public int PendingReservations { get; set; }
        public int ConfirmedReservations { get; set; }
        public int UnreadMessages { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Reservation> RecentReservations { get; set; } = new();
        public List<ContactMessage> RecentMessages { get; set; } = new();
    }

    // User Dashboard ViewModel
    public class UserDashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<Reservation> Reservations { get; set; } = new();
        public int TotalReservations { get; set; }
        public int ActiveReservations { get; set; }
        public int CancelledReservations { get; set; }
    }
}
