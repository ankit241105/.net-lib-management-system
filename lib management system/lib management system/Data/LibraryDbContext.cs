using Microsoft.EntityFrameworkCore;
using lib_management_system.Models;

namespace lib_management_system.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BookRequest> BookRequests { get; set; }
        public DbSet<MembershipRequest> MembershipRequests { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
