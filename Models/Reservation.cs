using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShadiHall.Models
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }

    public enum PaymentStatus
    {
        Unpaid,
        Paid,
        Refunded
    }

    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        public string ReservationCode { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int HallId { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        // 6 or 12 hour slot
        public string TimeSlot { get; set; } = "12hr"; // "6hr-morning", "6hr-evening", "12hr"

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public string GuestAddress { get; set; } = string.Empty;

        public string EventType { get; set; } = string.Empty; // Wedding, Birthday, etc.

        public int GuestCount { get; set; }

        public string? SpecialRequests { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public string? AdminNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ApplicationUser? User { get; set; }
        public virtual Hall? Hall { get; set; }
        public virtual Payment? Payment { get; set; }
    }
}
