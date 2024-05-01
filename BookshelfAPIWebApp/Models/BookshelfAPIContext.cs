using Microsoft.EntityFrameworkCore;

namespace BookshelfAPIWebApp.Models
{
    public class BookshelfAPIContext : DbContext
    {
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookAuthor> BookAuthors { get; set; }
        public virtual DbSet<BookCategory> BookCategories { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public BookshelfAPIContext(DbContextOptions<BookshelfAPIContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
