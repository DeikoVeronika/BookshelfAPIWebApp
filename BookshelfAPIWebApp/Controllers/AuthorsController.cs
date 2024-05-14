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
public class AuthorsController : ControllerBase
{
    private readonly BookshelfAPIContext _context;

    public AuthorsController(BookshelfAPIContext context)
    {
        _context = context;
    }

    // GET: api/Authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
    {
        var authors = await _context.Authors.OrderBy(author => author.Name)
                                             .Include(author => author.Books)
                                             .ToListAsync();

        if (authors.Count == 0)
        {
            return NotFound("Жодного автора ще не створено.");
        }

        var result = authors.Select(author => new
        {
            Id = author.Id,
            Name = author.Name,
            Description = author.Description,
            Birthday = author.Birthday != null ? author.Birthday.ToString() : "",
            Books = author.Books.Select(book => new
            {
                Title = book.Title,
            })
        });

        return Ok(result);
    }


    // GET: api/Authors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> GetAuthor(int id)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author == null)
        {
            return NotFound("Автора не знайдено.");
        }

        var result = new
        {
            Id = author.Id,
            Name = author.Name,
            Description = author.Description,
            Birthday = author.Birthday != null ? author.Birthday.ToString() : "",
            Books = author.Books

        };

        return Ok(result);
    }

    // PUT: api/Authors/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, Author author)
    {
        if (id != author.Id)
        {
            return BadRequest($"Id {id}, переданий в URL, не співпадає з ідентифікатором автора.");
        }

        var existingAuthor = await _context.Authors.FindAsync(id);
        if (existingAuthor == null)
        {
            return NotFound("Автора не знайдено.");
        }

        if (string.IsNullOrWhiteSpace(author.Name))
        {
            return BadRequest("ПІБ автора не може бути порожнім");
        }

        if (_context.Authors.Any(a => a.Name == author.Name && a.Description == author.Description && a.Id != id))
        {
            return BadRequest("Автор з таким іменем та біографією вже існує");
        }

        if (author.Birthday?.Year < 1200 || author.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-15)))
        {
            return BadRequest("Дата народження автора має бути не меншою за 1200 рік і автору не може бути менше 15 років.");
        }

        try
        {
            _context.Entry(existingAuthor).CurrentValues.SetValues(author);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AuthorExists(id))
            {
                return NotFound("Автор не знайдений.");
            }
            else
            {
                throw;
            }
        }

        return Ok("Інформацію про автора успішно оновлено.");
    }


    // POST: api/Authors
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Author>> PostAuthor(Author author)
    {
        if (string.IsNullOrWhiteSpace(author.Name))
        {
            return BadRequest("Поле ПІБ автора не може бути порожнім");
        }

        if (_context.Authors.Any(a => a.Name == author.Name && a.Description == author.Description))
        {
            return BadRequest("Автор з таким іменем та біографією вже існує");
        }

        if (author.Birthday?.Year < 1200 || author.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-15)))
        {
            return BadRequest("Дата народження автора має бути не меншою за 1200 рік і автору не може бути менше 15 років.");
        }

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
    }


    // DELETE: api/Authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
        {
            return NotFound("Автора не знайдено.");
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return Ok("Автора успішно видалено.");
    }


    private bool AuthorExists(int id)
    {
        return _context.Authors.Any(e => e.Id == id);
    }
}
