using Microsoft.EntityFrameworkCore;

namespace LenusBookStore.Models
{
    /// <summary>
    /// DbContext class for access to database data.
    /// In this case, it allows access to the book data for the app.
    /// </summary>
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) { }

        // Book data set
        public DbSet<Book> Books => Set<Book>();
    }
}
