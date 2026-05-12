# ✦ Shadi Hall — ASP.NET Core 8.0 Web Application

A full-stack, role-based hall reservation management system built with ASP.NET Core 8.0, Entity Framework Core, ASP.NET Identity, and SQL Server.

---

## 📋 Prerequisites

| Tool | Version |
|------|---------|
| Visual Studio (Insider or 2022) | 17.x+ |
| .NET SDK | 8.0 |
| SQL Server | Any (LocalDB, Express, Full) |
| SQL Server Management Studio | 20.1 |

---

## 🚀 Getting Started

### 1. Clone / Open Project

Open `ShadiHall.sln` in Visual Studio Insider.

### 2. Configure Connection String

Edit `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShadiHallDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

For full SQL Server:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=ShadiHallDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

### 3. Apply EF Migrations

**Option A — Package Manager Console (Visual Studio):**
```powershell
Add-Migration InitialCreate
Update-Database
```

**Option B — .NET CLI:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the Application

Press **F5** or `Ctrl+F5` in Visual Studio, or:
```bash
dotnet run
```

---

## 🔑 Default Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@shadihall.com | Admin@123! |

---

## 📁 Project Structure

```
ShadiHall/
├── Controllers/
│   ├── HomeController.cs         ← Home, About, Contact
│   ├── AccountController.cs      ← Register, Login, Logout
│   ├── HallController.cs         ← Hall listing, details, reviews
│   ├── ReservationController.cs  ← Create, Payment, Confirmation, Edit, Cancel
│   ├── AdminController.cs        ← Admin dashboard (Role: Admin)
│   └── UserController.cs         ← User dashboard (Role: User)
│
├── Models/
│   ├── ApplicationUser.cs        ← Extended Identity user (FullName, Address, LastLogin)
│   ├── Hall.cs                   ← Hall entity
│   ├── Reservation.cs            ← Reservation entity with status enums
│   └── OtherModels.cs            ← Review, Payment, ContactMessage, BannerSlide
│
├── ViewModels/
│   └── ViewModels.cs             ← All ViewModels (Register, Login, HallForm, etc.)
│
├── Data/
│   └── ApplicationDbContext.cs   ← EF DbContext with seed data
│
├── Services/
│   └── Services.cs               ← IHallService, HallService, IReservationService, ReservationService
│
├── Views/
│   ├── Shared/_Layout.cshtml     ← Master layout with navbar & footer
│   ├── Home/                     ← Index, About, Contact
│   ├── Hall/                     ← Index, Details
│   ├── Reservation/              ← Create, Payment, Confirmation, Edit
│   ├── Account/                  ← Login, Register, AccessDenied
│   ├── Admin/                    ← Index, Halls, CreateHall, EditHall, Reservations, Users, Messages
│   └── User/                     ← Dashboard (reservations, profile, password)
│
├── wwwroot/
│   ├── css/site.css              ← Full responsive stylesheet
│   ├── js/site.js                ← Slider, hamburger, animations
│   └── images/                   ← Place hall images here
│
├── Database/
│   └── setup.sql                 ← Reference SQL (EF migrations handle this)
│
├── Program.cs                    ← App startup & DI configuration
├── appsettings.json
└── ShadiHall.csproj
```

---

## 🖼️ Adding Hall Images

Place images in `wwwroot/images/`:

| File | Used for |
|------|----------|
| `banner1.jpg` | Hero slider slide 1 |
| `banner2.jpg` | Hero slider slide 2 |
| `banner3.jpg` | Hero slider slide 3 |
| `hall1.jpg` | Royal Grand Ballroom |
| `hall2.jpg` | Garden Terrace |
| `hall3.jpg` | Diamond Conference Hall |
| `hall4.jpg` | Jasmine Banquet |
| `default-hall.jpg` | Fallback image |
| `about-why.jpg` | Why Choose Us section |

> **Tip:** Use 1200×800px JPEGs for best quality. Hall images auto-crop to fit cards.

---

## 🗃️ Database Schema

### Core Tables

| Table | Key Fields |
|-------|-----------|
| `Users` | Id, UserName, Email, PasswordHash, FullName, Address, PhoneNumber, CreatedAt, LastLogin |
| `Roles` | Id, Name (Admin / User) |
| `UserRoles` | UserId, RoleId |
| `Halls` | HallId, Name, Description, PricePerDay, Capacity, Location, ImageUrl, IsAvailable, IsFeatured |
| `Reservations` | ReservationId, ReservationCode, UserId, HallId, EventDate, TimeSlot, Status, PaymentStatus, TotalAmount |
| `Reviews` | ReviewId, UserId, HallId, Rating, Comment, IsApproved |
| `Payments` | PaymentId, ReservationId, TransactionId, Amount, PaymentMethod, Status |
| `ContactMessages` | MessageId, FullName, Email, Phone, Subject, Message, IsRead |
| `BannerSlides` | SlideId, Title, Subtitle, ImageUrl, CtaText, CtaLink, IsActive |

---

## ✅ Feature Checklist

### Navigation
- [x] Logo + Menu links
- [x] Login / Logout / Register buttons
- [x] Responsive hamburger menu

### Homepage
- [x] Auto-sliding hero banner with dots & arrows
- [x] Stats bar (events, halls, years, families)
- [x] Featured halls grid
- [x] Why Choose Us section
- [x] Customer reviews section
- [x] CTA banner

### Halls & Services
- [x] Grid layout with search & filters
- [x] Hall card: image, title, price, capacity, amenities
- [x] View Details button
- [x] Pagination

### Hall Detail Page
- [x] Full description + amenities
- [x] Average rating & review count
- [x] Booking sidebar with price
- [x] Reserve button (auth-gated)
- [x] Reviews section with star rating form

### Reservation
- [x] Date picker (min: tomorrow)
- [x] Time slot: 6hr-morning / 6hr-evening / 12hr
- [x] Auto-filled user info
- [x] Hall availability check
- [x] Payment page (Cash / bKash / Nagad / Card)
- [x] Confirmation page with reservation code

### Authentication
- [x] Register: FullName, Username, Email, Password (hashed), Address, Phone
- [x] Login with email + password
- [x] Remember me
- [x] UserId, RoleId, LastLogin tracked
- [x] Password strength validation

### Admin Dashboard (Role: Admin)
- [x] Stats overview (halls, users, reservations, revenue)
- [x] Insert / Update / Delete hall
- [x] Manage all reservations
- [x] Change reservation status
- [x] View all users with roles
- [x] Read / delete contact messages

### User Dashboard (Role: User)
- [x] View own reservations
- [x] Edit pending reservation
- [x] Cancel reservation
- [x] View reservation status
- [x] Edit profile (name, phone, address)
- [x] Change password

### Reviews
- [x] Authenticated users only
- [x] 1–5 star interactive rating
- [x] Text comment (min 10 chars)
- [x] One review per hall per user

### Contact Page
- [x] Full contact form with validation
- [x] Contact info (address, phone, email, hours)
- [x] Messages stored in database
- [x] Admin can read/delete messages

---

## 🔐 Role-Based Access

| Route | Guest | User | Admin |
|-------|-------|------|-------|
| Home, Halls, About, Contact | ✅ | ✅ | ✅ |
| Hall Details | ✅ | ✅ | ✅ |
| Reserve / Payment | ❌ | ✅ | ✅ |
| User Dashboard | ❌ | ✅ | ✅ |
| Admin Dashboard | ❌ | ❌ | ✅ |
| Submit Review | ❌ | ✅ | ✅ |

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **ORM:** Entity Framework Core 8.0
- **Auth:** ASP.NET Core Identity
- **Database:** SQL Server / LocalDB
- **Frontend:** Vanilla CSS (custom design system) + Vanilla JS
- **Fonts:** Playfair Display + DM Sans (Google Fonts)
- **Icons:** Font Awesome 6.5

---

## 📞 Support

For setup issues, check:
1. Connection string is correct
2. .NET 8 SDK is installed: `dotnet --version`
3. EF tools installed: `dotnet tool install --global dotnet-ef`
4. Migrations ran successfully: `Update-Database` in PMC
