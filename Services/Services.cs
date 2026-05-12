using Microsoft.EntityFrameworkCore;
using ShadiHall.Data;
using ShadiHall.Models;
using ShadiHall.ViewModels;

namespace ShadiHall.Services
{
    public interface IHallService
    {
        Task<HallListViewModel> GetHallsAsync(string? search, decimal? minPrice, decimal? maxPrice, int? minCapacity, int page = 1);
        Task<Hall?> GetHallByIdAsync(int id);
        Task<Hall> CreateHallAsync(HallFormViewModel model);
        Task<bool> UpdateHallAsync(HallFormViewModel model);
        Task<bool> DeleteHallAsync(int id);
        Task<List<Hall>> GetFeaturedHallsAsync();
        Task<double> GetAverageRatingAsync(int hallId);
    }

    public class HallService : IHallService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const int PageSize = 8;

        public HallService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<HallListViewModel> GetHallsAsync(string? search, decimal? minPrice, decimal? maxPrice, int? minCapacity, int page = 1)
        {
            var query = _context.Halls.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(h => h.Name.Contains(search) || h.ShortDescription.Contains(search));

            if (minPrice.HasValue) query = query.Where(h => h.PricePerDay >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(h => h.PricePerDay <= maxPrice.Value);
            if (minCapacity.HasValue) query = query.Where(h => h.Capacity >= minCapacity.Value);

            var total = await query.CountAsync();
            var halls = await query.OrderByDescending(h => h.IsFeatured).ThenBy(h => h.Name)
                .Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            return new HallListViewModel
            {
                Halls = halls,
                SearchTerm = search,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinCapacity = minCapacity,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)total / PageSize),
                TotalCount = total
            };
        }

        public async Task<Hall?> GetHallByIdAsync(int id)
            => await _context.Halls.FirstOrDefaultAsync(h => h.HallId == id);

        public async Task<Hall> CreateHallAsync(HallFormViewModel model)
        {
            var hall = new Hall
            {
                Name = model.Name,
                Description = model.Description,
                ShortDescription = model.ShortDescription,
                PricePerDay = model.PricePerDay,
                Capacity = model.Capacity,
                Location = model.Location,
                Amenities = model.Amenities,
                IsAvailable = model.IsAvailable,
                IsFeatured = model.IsFeatured,
                ImageUrl = await SaveImageAsync(model.ImageFile) ?? "/images/default-hall.svg"
            };

            _context.Halls.Add(hall);
            await _context.SaveChangesAsync();
            return hall;
        }

        public async Task<bool> UpdateHallAsync(HallFormViewModel model)
        {
            var hall = await _context.Halls.FindAsync(model.HallId);
            if (hall == null) return false;

            hall.Name = model.Name;
            hall.Description = model.Description;
            hall.ShortDescription = model.ShortDescription;
            hall.PricePerDay = model.PricePerDay;
            hall.Capacity = model.Capacity;
            hall.Location = model.Location;
            hall.Amenities = model.Amenities;
            hall.IsAvailable = model.IsAvailable;
            hall.IsFeatured = model.IsFeatured;
            hall.UpdatedAt = DateTime.UtcNow;

            if (model.ImageFile != null)
                hall.ImageUrl = await SaveImageAsync(model.ImageFile) ?? hall.ImageUrl;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHallAsync(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall == null) return false;
            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Hall>> GetFeaturedHallsAsync()
            => await _context.Halls.Where(h => h.IsFeatured && h.IsAvailable).Take(4).ToListAsync();

        public async Task<double> GetAverageRatingAsync(int hallId)
        {
            var reviews = await _context.Reviews.Where(r => r.HallId == hallId && r.IsApproved).ToListAsync();
            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            var folder = Path.Combine(_env.WebRootPath, "images", "halls");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/images/halls/{fileName}";
        }
    }

    public interface IReservationService
    {
        Task<Reservation?> GetByIdAsync(int id);
        Task<Reservation?> GetByCodeAsync(string code);
        Task<Reservation> CreateAsync(ReservationCreateViewModel model, string userId);
        Task<bool> UpdateStatusAsync(int id, ReservationStatus status);
        Task<bool> CancelAsync(int id, string userId);
        Task<List<Reservation>> GetUserReservationsAsync(string userId);
        Task<List<Reservation>> GetAllReservationsAsync();
        Task ProcessPaymentAsync(int reservationId, string method, string? txRef);
        Task<bool> IsHallAvailable(int hallId, DateTime date, string timeSlot);
    }

    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context) => _context = context;

        public async Task<Reservation?> GetByIdAsync(int id)
            => await _context.Reservations.Include(r => r.Hall).Include(r => r.User).Include(r => r.Payment).FirstOrDefaultAsync(r => r.ReservationId == id);

        public async Task<Reservation?> GetByCodeAsync(string code)
            => await _context.Reservations.Include(r => r.Hall).Include(r => r.User).Include(r => r.Payment).FirstOrDefaultAsync(r => r.ReservationCode == code);

        public async Task<Reservation> CreateAsync(ReservationCreateViewModel model, string userId)
        {
            var (start, end) = GetTimeSlotRange(model.TimeSlot);
            var hall = await _context.Halls.FindAsync(model.HallId);
            var totalAmount = hall != null ? (model.TimeSlot == "12hr" ? hall.PricePerDay : hall.PricePerDay * 0.6m) : 0;

            var reservation = new Reservation
            {
                UserId = userId,
                HallId = model.HallId,
                EventDate = model.EventDate,
                TimeSlot = model.TimeSlot,
                StartTime = start,
                EndTime = end,
                GuestName = model.GuestName,
                GuestEmail = model.GuestEmail,
                GuestPhone = model.GuestPhone,
                GuestAddress = model.GuestAddress,
                EventType = model.EventType,
                GuestCount = model.GuestCount,
                SpecialRequests = model.SpecialRequests,
                TotalAmount = totalAmount,
                Status = ReservationStatus.Pending
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> UpdateStatusAsync(int id, ReservationStatus status)
        {
            var r = await _context.Reservations.FindAsync(id);
            if (r == null) return false;
            r.Status = status;
            r.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int id, string userId)
        {
            var r = await _context.Reservations.FindAsync(id);
            if (r == null || r.UserId != userId) return false;
            r.Status = ReservationStatus.Cancelled;
            r.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Reservation>> GetUserReservationsAsync(string userId)
            => await _context.Reservations.Include(r => r.Hall).Where(r => r.UserId == userId).OrderByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<List<Reservation>> GetAllReservationsAsync()
            => await _context.Reservations.Include(r => r.Hall).Include(r => r.User).OrderByDescending(r => r.CreatedAt).ToListAsync();

        public async Task ProcessPaymentAsync(int reservationId, string method, string? txRef)
        {
            var payment = new Payment
            {
                ReservationId = reservationId,
                Amount = (await _context.Reservations.FindAsync(reservationId))!.TotalAmount,
                PaymentMethod = method,
                Status = "Completed",
                Notes = txRef
            };
            _context.Payments.Add(payment);
            var res = await _context.Reservations.FindAsync(reservationId);
            if (res != null) res.PaymentStatus = PaymentStatus.Paid;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsHallAvailable(int hallId, DateTime date, string timeSlot)
        {
            return !await _context.Reservations.AnyAsync(r =>
                r.HallId == hallId &&
                r.EventDate.Date == date.Date &&
                r.TimeSlot == timeSlot &&
                r.Status != ReservationStatus.Cancelled);
        }

        private static (TimeSpan start, TimeSpan end) GetTimeSlotRange(string slot) => slot switch
        {
            "6hr-morning" => (new TimeSpan(8, 0, 0), new TimeSpan(14, 0, 0)),
            "6hr-evening" => (new TimeSpan(16, 0, 0), new TimeSpan(22, 0, 0)),
            _ => (new TimeSpan(8, 0, 0), new TimeSpan(20, 0, 0))
        };
    }
}
