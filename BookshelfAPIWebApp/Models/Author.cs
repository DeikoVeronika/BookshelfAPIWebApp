using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookshelfAPIWebApp.Models;

public class Author
{

    public int Id { get; set; }


    [Required(ErrorMessage = "Введіть ім'я та прізвище автора")]
    [Display(Name = "Ім'я та прізвище")]
    public string Name { get; set; }


    [Display(Name = "Опис")]
    public string? Description { get; set; }


    [Display(Name = "Дата народження")]
    public DateOnly? Birthday { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
