using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Librarian> Librarians { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<BorrowingParameters> BorrowingParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(t => t.Book)
                .WithMany(b => b.BorrowTransactions)
                .HasForeignKey(t => t.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(t => t.Member)
                .WithMany(m => m.BorrowTransactions)
                .HasForeignKey(t => t.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fine>()
                .HasOne(f => f.BorrowTransaction)
                .WithOne(t => t.Fine)
                .HasForeignKey<Fine>(f => f.BorrowTransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reservations)
                .HasForeignKey(r => r.BookID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Member)
                .WithMany(m => m.Reservations)
                .HasForeignKey(r => r.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Book)
                .WithMany(b => b.Feedbacks)
                .HasForeignKey(f => f.BookID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Member)
                .WithMany(m => m.Feedbacks)
                .HasForeignKey(f => f.MemberID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BorrowingParameters>()
                .Property(p => p.OverduePenaltyPerDay)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Fine>()
                .Property(f => f.Amount)
                .HasColumnType("decimal(10,2)");

            // Enforce uniqueness on auto-generated business IDs
            modelBuilder.Entity<Librarian>()
                .HasIndex(l => l.EmployeeID)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.MembershipNumber)
                .IsUnique();
        }
    }
}
