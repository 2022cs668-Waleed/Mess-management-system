using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using _2022_CS_668.Models;

namespace _2022_CS_668.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MessGroup> MessGroups { get; set; }
        public DbSet<UserMessGroup> UserMessGroups { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AttendanceMenuItem> AttendanceMenuItems { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // MessGroup configuration
            builder.Entity<MessGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // UserMessGroup configuration
            builder.Entity<UserMessGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.MessGroupId });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserMessGroups)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MessGroup)
                    .WithMany(m => m.UserMessGroups)
                    .HasForeignKey(e => e.MessGroupId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Menu configuration
            builder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => new { e.EffectiveDate, e.MessGroupId });

                entity.HasOne(e => e.MessGroup)
                    .WithMany(m => m.Menus)
                    .HasForeignKey(e => e.MessGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Attendance configuration
            builder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Attendances)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AttendanceMenuItem configuration
            builder.Entity<AttendanceMenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PriceAtSelection).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => new { e.AttendanceId, e.MenuId });

                entity.HasOne(e => e.Attendance)
                    .WithMany(a => a.AttendanceMenuItems)
                    .HasForeignKey(e => e.AttendanceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Menu)
                    .WithMany()
                    .HasForeignKey(e => e.MenuId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Bill configuration
            builder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FoodAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.WaterTeaAmount).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => new { e.UserId, e.Month, e.Year }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Bills)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // BillDetail configuration
            builder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Bill)
                    .WithMany(b => b.BillDetails)
                    .HasForeignKey(e => e.BillId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Menu)
                    .WithMany(m => m.BillDetails)
                    .HasForeignKey(e => e.MenuId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment configuration
            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);

                entity.HasOne(e => e.Bill)
                    .WithMany(b => b.Payments)
                    .HasForeignKey(e => e.BillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ApplicationUser additional configuration
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            });
        }
    }
}
