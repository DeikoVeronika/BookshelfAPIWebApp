using System.ComponentModel.DataAnnotations;

namespace BookshelfAPIWebApp.Models;

public class Book
{


    public int Id { get; set; }
    [Display(Name = "Назва")]
    public string Title { get; set; }
    [Display(Name = "Опис")]
    public string Description { get; set; }
    [Display(Name = "Рік")]
    public int? Year { get; set; }
    [Display(Name = "Кількість сторінок")]
    public int? NumberOfPages { get; set; }
    [Display(Name = "Вподобані")]
    public bool Favorite { get; set; }

    public int LanguageId { get; set; }

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
