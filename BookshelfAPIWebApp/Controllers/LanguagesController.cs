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
public class LanguagesController : ControllerBase
{
    private readonly BookshelfAPIContext _context;

    public LanguagesController(BookshelfAPIContext context)
    {
        _context = context;
    }

    // GET: api/Languages    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
    {
        var languages = await _context.Languages.ToListAsync();

        if (languages.Count == 0)
        {
            return NotFound("Жодної мови ще не створено");
        }

        return languages;
    }


    // GET: api/Languages/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Language>> GetLanguage(int id)
    {
        var language = await _context.Languages.FindAsync(id);

        if (language == null)
        {
            return NotFound("Мову не знайдено.");
        }

        return language;
    }

    // PUT: api/Languages/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutLanguage(int id, Language language)
    {
        if (id != language.Id)
        {
            return BadRequest($"Мову з Id {id} не знайдено.");
        }

        var existingLanguage = await _context.Languages.FindAsync(id);
        if (existingLanguage == null)
        {
            return NotFound("Мова не знайдена.");
        }

        if (_context.Languages.Any(l => l.Name == language.Name && l.Id != id))
        {
            return BadRequest("Мова з такою назвою вже існує.");
        }

        try
        {
            // Оновлення властивостей об'єкта, якщо він уже відстежується в контексті
            _context.Entry(existingLanguage).CurrentValues.SetValues(language);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LanguageExists(id))
            {
                return NotFound("Мова не знайдена.");
            }
            else
            {
                throw;
            }
        }

        return Ok("Мову успішно оновлено.");
    }


    // POST: api/Languages
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Language>> PostLanguage(Language language)
    {
        if (_context.Languages.Any(l => l.Name == language.Name))
        {
            return BadRequest("Мова з такою назвою вже існує.");
        }

        _context.Languages.Add(language);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetLanguage", new { id = language.Id }, language);
    }

    // DELETE: api/Languages/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLanguage(int id)
    {
        var language = await _context.Languages.FindAsync(id);
        if (language == null)
        {
            return NotFound("Мову не знайдено.");
        }

        _context.Languages.Remove(language);
        await _context.SaveChangesAsync();

        return Ok("Мову успішно видалено.");
    }

    private bool LanguageExists(int id)
    {
        return _context.Languages.Any(e => e.Id == id);
    }
}
