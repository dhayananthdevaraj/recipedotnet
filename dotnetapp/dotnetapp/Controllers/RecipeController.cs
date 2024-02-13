using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Models; // Assuming your models are in the DotNetApp.Models namespace

[Route("api/recipe")]
[ApiController]
public class RecipeController : ControllerBase
{
    private readonly ApplicationDbContext _context; // Replace ApplicationDbContext with your actual DbContext class

    public RecipeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes([FromQuery] int? sortValue = 1, [FromQuery] string? searchTerm = "")
    {
        var recipes = await _context.Recipes.ToListAsync(); // Retrieve all recipes from the database

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchRegex = new System.Text.RegularExpressions.Regex(searchTerm, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            recipes = recipes.Where(recipe => searchRegex.IsMatch(recipe.RecipeName)).ToList(); // Apply search filter
        }

        if (sortValue == -1)
        {
            recipes = recipes.OrderByDescending(recipe => recipe.Price).ToList(); // Sort in descending order
        }
        else
        {
            recipes = recipes.OrderBy(recipe => recipe.Price).ToList(); // Sort in ascending order (default)
        }

        return Ok(recipes);
    }

    [HttpGet("{recipeId}")]
    public async Task<ActionResult<Recipe>> GetRecipeById(int recipeId)
    {
        var recipe = await _context.Recipes.FirstOrDefaultAsync(recipe => recipe.RecipeId == recipeId);

        if (recipe == null)
        {
            return NotFound(new { message = "Cannot find any recipe" });
        }

        return Ok(recipe);
    }

    [HttpPost]
    public async Task<ActionResult> AddRecipe([FromBody] Recipe recipe)
    {
        try
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recipe added successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{recipeId}")]
    public async Task<ActionResult> UpdateRecipe(int recipeId, [FromBody] Recipe recipe)
    {
        try
        {
            var existingRecipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (existingRecipe == null)
            {
                return NotFound(new { message = "Cannot find any recipe" });
            }

            recipe.RecipeId = recipeId;
            _context.Entry(existingRecipe).CurrentValues.SetValues(recipe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recipe updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{recipeId}")]
    public async Task<ActionResult> DeleteRecipe(int recipeId)
    {
        try
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            if (recipe == null)
            {
                return NotFound(new { message = "Cannot find any recipe" });
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Recipe deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesByUserId(int userId, [FromQuery] string? searchTerm = null)
    {
        var recipes = await _context.Recipes.ToListAsync();

        recipes = recipes.Where(recipe => recipe.UserId == userId).ToList();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchRegex = new System.Text.RegularExpressions.Regex(searchTerm, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            recipes = recipes.Where(recipe => searchRegex.IsMatch(recipe.RecipeName)).ToList();
        }

        return Ok(recipes);
    }
}