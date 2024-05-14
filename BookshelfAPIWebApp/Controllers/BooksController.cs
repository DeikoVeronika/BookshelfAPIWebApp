using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshelfAPIWebApp.Models;

namespace BookshelfAPIWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly BookshelfAPIContext _context;

    public BooksController(BookshelfAPIContext context)
    {
        _context = context;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var books = await _context.Books.OrderBy(book => book.Title).ToListAsync();

        if(books.Count == 0)
        {
            return NotFound("Жодної книги ще не створено.");
        }
        return books;
    }

    // GET: api/Books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound("Книгу не знайдено.");
        }

        return book;
    }



    // PUT: api/Books/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult<Book>> PutBook(int id, Book book)
    {
        if (id != book.Id)
        {
            return BadRequest($"Ідентифікатор {id}, вказаний в URL, не збігається з ідентифікатором книги.");
        }

        
        if (await IdenticalBookExist(book))
        {
            return BadRequest("Книга з такими характеристиками вже існує");
        }

        var existingBook = await _context.Books.Include(b => b.Authors).Include(b => b.Genres).Include(b => b.Categories).FirstOrDefaultAsync(b => b.Id == id);

        if (existingBook == null)
        {
            return NotFound("Книга не знайдена");
        }

      
        // Очистити старі жанри, категорії та авторів
        existingBook.Authors.Clear();
        existingBook.Genres.Clear();
        existingBook.Categories.Clear();

        // Оновити інформацію про книгу
        _context.Entry(existingBook).CurrentValues.SetValues(book);

        // Додати нові жанри, категорії та авторів
        var (existingAuthors, missingAuthorId) = await GetExistingAuthors(book.Authors);
        if (missingAuthorId != null)
        {
            return BadRequest($"Відсутній автор з ID {missingAuthorId}");
        }
        existingBook.Authors = existingAuthors;

        var (existingGenres, missingGenreId) = await GetExistingGenres(book.Genres);
        if (missingGenreId != null)
        {
            return BadRequest($"Відсутній жанр з ID {missingGenreId}");
        }
        existingBook.Genres = existingGenres;

        var (existingCategories, missingCategoryId) = await GetExistingCategories(book.Categories);
        if (missingCategoryId != null)
        {
            return BadRequest($"Відсутня категорія з ID {missingCategoryId}");
        }
        existingBook.Categories = existingCategories;




        try
        {
            
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
            {
                return NotFound("Книгу не знайдено.");
            }
            else
            {
                throw;
            }
        }

        return Ok("Інформацію про книгу успішно оновлено.");
    }



    // POST: api/Books
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        if (!await LanguageExists(book.LanguageId))
        {
            return BadRequest("Відсутня мова з таким ID");
        }


        var (existingAuthors, missingAuthorId) = await GetExistingAuthors(book.Authors);

        if (missingAuthorId != null)
        {
            return BadRequest($"Відсутній автор з ID {missingAuthorId}");
        }

        book.Authors = existingAuthors;

        if (await IdenticalBookExist(book))
        {
            return BadRequest("Книга з такими характеристиками вже існує");
        }


        var (existingGenres, missingGenreId) = await GetExistingGenres(book.Genres);

        if (missingGenreId != null)
        {
            return BadRequest($"Відсутній жанр з ID {missingGenreId}");
        }

        book.Genres = existingGenres;


        var (existingCategories, missingCategoryId) = await GetExistingCategories(book.Categories);

        if (missingCategoryId != null)
        {
            return BadRequest($"Відсутня категорія з ID {missingCategoryId}");
        }

        book.Categories = existingCategories;


        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetBook", new { id = book.Id }, book);
    }


    // DELETE: api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound("Книгу не знайдено.");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return Ok("Книгу успішно видалено.");
    }


    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }

    private async Task<bool> LanguageExists(int languageId)
    {
        var existingLanguage = await _context.Languages.FindAsync(languageId);
        return existingLanguage != null;
    }

    private async Task<bool> IdenticalBookExist(Book book)
    {
        //var existingBook = await _context.Books
        //    .Where(b => b.Title == book.Title &&
        //                b.Description == book.Description &&
        //                b.Year == book.Year &&
        //                b.LanguageId == book.LanguageId &&
        //                b.Authors.Any(a => book.Authors.Select(x => x.Id).Contains(a.Id)))
        //    .FirstOrDefaultAsync();
        var existingBook = await _context.Books
            .Where(b => b.Title == book.Title &&
                        b.Description == book.Description &&
                        b.Year == book.Year &&
                        b.LanguageId == book.LanguageId &&
                        b.Authors.Count == book.Authors.Count &&
                        b.Authors.All(a => book.Authors.Select(x => x.Id).Contains(a.Id)))
            .FirstOrDefaultAsync();

        return existingBook != null;

    }

    private async Task<(List<Author> existingAuthors, int? missingAuthorId)> GetExistingAuthors(ICollection<Author> authors)
    {
        List<Author> existingAuthors = new List<Author>();
        int? missingAuthorId = null;

        foreach (var author in authors)
        {
            var existingAuthor = await _context.Authors.FindAsync(author.Id);

            if (existingAuthor == null)
            {
                missingAuthorId = author.Id;
                break;
            }

            existingAuthors.Add(existingAuthor);
        }

        return (existingAuthors, missingAuthorId);
    }
    private async Task<(List<Genre> existingGenres, int? missingGenreId)> GetExistingGenres(ICollection<Genre> genres)
    {
        List<Genre> existingGenres = new List<Genre>();
        int? missingGenreId = null;

        foreach (var genre in genres)
        {
            var existingGenre = await _context.Genres.FindAsync(genre.Id);

            if (existingGenre == null)
            {
                missingGenreId = genre.Id;
                break;
            }

            existingGenres.Add(existingGenre);
        }

        return (existingGenres, missingGenreId);
    }

    private async Task<(List<Category> existingCategories, int? missingCategoryId)> GetExistingCategories(ICollection<Category> categories)
    {
        List<Category> existingCategories = new List<Category>();
        int? missingCategoryId = null;

        foreach (var category in categories)
        {
            var existingCategory = await _context.Categories.FindAsync(category.Id);

            if (existingCategory == null)
            {
                missingCategoryId = category.Id;
                break;
            }

            existingCategories.Add(existingCategory);
        }

        return (existingCategories, missingCategoryId);
    }


}
