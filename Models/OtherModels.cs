using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShadiHall.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int HallId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required, MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ApplicationUser? User { get; set; }
        public virtual Hall? Hall { get; set; }
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int ReservationId { get; set; }

        public string TransactionId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, bKash, Nagad

        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        // Navigation
        public virtual Reservation? Reservation { get; set; }
    }

    public class ContactMessage
    {
        [Key]
        public int MessageId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

    public class BannerSlide
    {
        [Key]
        public int SlideId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CtaText { get; set; } = "Book Now";
        public string CtaLink { get; set; } = "/Hall";
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
