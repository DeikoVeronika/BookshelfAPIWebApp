using Microsoft.EntityFrameworkCore;

namespace BookshelfAPIWebApp.Models;

public class BookshelfAPIContext : DbContext
{
    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Genre> Genres { get; set; }
    public virtual DbSet<Language> Languages { get; set; }


    public BookshelfAPIContext(DbContextOptions<BookshelfAPIContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
