using System.ComponentModel.DataAnnotations;

namespace BookshelfAPIWebApp.Models
{
    public class Book
    {
        public Book()
        {
            BookAuthors = new List<BookAuthor>();
            BookCategories = new List<BookCategory>();
        }

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


        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
        public virtual ICollection<BookCategory> BookCategories { get; set; }
    }
}
