using System.ComponentModel.DataAnnotations;

namespace BookshelfAPIWebApp.Models;

public class Language
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введіть мову")]
    [Display(Name = "Мова")]
    public string Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

}
