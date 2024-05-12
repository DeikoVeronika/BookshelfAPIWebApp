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
public class GenresController : ControllerBase
{
    private readonly BookshelfAPIContext _context;

    public GenresController(BookshelfAPIContext context)
    {
        _context = context;
    }

    // GET: api/Genres
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        var genres = await _context.Genres.ToListAsync();

        if (genres.Count == 0)
        {
            return NotFound("Жодного жанру ще не створено");
        }

        return genres;
    }


    // GET: api/Genres/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> GetGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);

        if (genre == null)
        {
            return NotFound("Жанр не знайдено.");
        }

        return genre;
    }

    // PUT: api/Genres/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGenre(int id, Genre genre)
    {
        if (id != genre.Id)
        {
            return BadRequest($"Жанр з Id {id} не знайдено.");
        }

        var existingGenre = await _context.Genres.FindAsync(id);
        if (existingGenre == null)
        {
            return NotFound("Жанр не знайдено.");
        }

        if (_context.Genres.Any(g => g.Name == genre.Name && g.Id != id))
        {
            return BadRequest("Жанр з такою назвою вже існує.");
        }

        try
        {
            // Оновлення властивостей об'єкта, якщо він уже відстежується в контексті
            _context.Entry(existingGenre).CurrentValues.SetValues(genre);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GenreExists(id))
            {
                return NotFound("Жанр не знайдено.");
            }
            else
            {
                throw;
            }
        }

        return Ok("Жанр успішно оновлено.");
    }


    // POST: api/Genres
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Genre>> PostGenre(Genre genre)
    {
        if (_context.Genres.Any(g => g.Name == genre.Name))
        {
            return BadRequest("Жанр з такою назвою вже існує");
        }

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetGenre", new { id = genre.Id }, genre);
    }

    // DELETE: api/Genres/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound("Жанр не знайдено.");
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return Ok("Жанр успішно видалено.");
    }

    private bool GenreExists(int id)
    {
        return _context.Genres.Any(e => e.Id == id);
    }
}
