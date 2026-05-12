using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShadiHall.Models
{
    public class Hall
    {
        [Key]
        public int HallId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerDay { get; set; }

        public int Capacity { get; set; }

        public string Location { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = "/images/default-hall.svg";

        public string? GalleryImages { get; set; } // JSON array of image paths

        public bool IsAvailable { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        public string Amenities { get; set; } = string.Empty; // comma-separated

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
