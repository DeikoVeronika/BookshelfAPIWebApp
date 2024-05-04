using System.ComponentModel.DataAnnotations;

namespace BookshelfAPIWebApp.Models
{
    public class Category
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Введіть назву категорії")]
        [Display(Name = "Назва категорії")]
        public string Name { get; set; }
        [Display(Name = "Опис")]
        public string? Description { get; set; }
        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    }
}
