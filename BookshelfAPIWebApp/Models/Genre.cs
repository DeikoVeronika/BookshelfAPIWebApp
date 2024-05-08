using System.ComponentModel.DataAnnotations;

namespace BookshelfAPIWebApp.Models;

public class Genre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введіть назву жанру")]
    [Display(Name = "Назва жанру")]
    public string Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

}
