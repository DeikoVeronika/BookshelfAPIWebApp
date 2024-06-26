﻿using System;
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
public class CategoriesController : ControllerBase
{
    private readonly BookshelfAPIContext _context;

    public CategoriesController(BookshelfAPIContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _context.Categories.OrderBy(category => category.Name).ToListAsync();

        if (categories.Count == 0)
        {
            return NotFound("Жодної категорії ще не створено");
        }

        return categories;
    }

    // GET: api/Categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound("Категорію не знайдено.");
        }

        return category;
    }

    // PUT: api/Categories/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest($"Id {id}, переданий в URL, не співпадає з ідентифікатором категорії.");
        }

        var existingCategory = await _context.Categories.FindAsync(id);
        if (existingCategory == null)
        {
            return NotFound("Категорія не знайдена.");
        }

        if (string.IsNullOrWhiteSpace(category.Name))
        {
            return BadRequest("Назва категорії не може бути порожньою");
        }

        if (_context.Categories.Any(c => c.Name == category.Name && c.Id != id))
        {
            return BadRequest("Категорія з такою назвою вже існує.");
        }

        try
        {
            _context.Entry(existingCategory).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
            {
                return NotFound("Категорія не знайдена.");
            }
            else
            {
                throw;
            }
        }

        return Ok("Категорію успішно оновлено.");
    }


    // POST: api/Categories
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
        {
            return BadRequest("Назва категорії не може бути порожньою");
        }

        if (_context.Categories.Any(c => c.Name == category.Name))
        {
            return BadRequest("Категорія з такою назвою вже існує");
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCategory", new { id = category.Id }, category);
    }


    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound("Категорія не знайдена");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok("Категорію успішно видалено");
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
