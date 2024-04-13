using CulturNary.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CulturNary.DAL.Abstract;
using CulturNary.Web.Models;
[Authorize(Roles = "Signed,Admin")]
[Route("Recommendations")]
public class UserRecommendationsController : Controller
{
    private readonly UserManager<SiteUser> _userManager;
    private readonly CulturNaryDbContext _context;
    private readonly IFavoriteRecipeRepository _favoriteRecipeRepository;
    private readonly ILogger<UserRecommendationsController> _logger;

    public UserRecommendationsController(
        UserManager<SiteUser> userManager,
        CulturNaryDbContext context,
        IFavoriteRecipeRepository favoriteRecipeRepository,
        ILogger<UserRecommendationsController> logger)
    {
        _userManager = userManager;
        _context = context;
        _favoriteRecipeRepository = favoriteRecipeRepository;
        _logger = logger;
    }
    [Route("Favorite")]
    public async Task<IActionResult> Favorite()
    {
        var user = await _userManager.GetUserAsync(User);
        var person = await _context.People.FirstOrDefaultAsync(p => p.IdentityId == user.Id);

        if (person == null)
        {
            // Handle case where person is not found
            return NotFound();
        }

        var favoriteRecipes = _favoriteRecipeRepository.GetFavoriteRecipeForPersonID(person.Id);

        if (favoriteRecipes.Any())
        {
            _logger.LogInformation("Favorite recipes retrieved successfully.");
        }
        else
        {
            _logger.LogInformation("No favorite recipes found.");
        }

        return View(favoriteRecipes);
    }
    [HttpPost]
    public async Task<IActionResult> DeleteFavoriteRecipe(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var person = await _context.People.FirstOrDefaultAsync(p => p.IdentityId == user.Id);

        if (person == null)
        {
            return NotFound();
        }

        var recipe = _favoriteRecipeRepository.GetFavoriteRecipeForPersonIDAndRecipeID(person.Id, id);

        if (recipe == null)
        {
            return NotFound();
        }

        _favoriteRecipeRepository.Delete(recipe);

        return RedirectToAction(nameof(Favorite));
    }
}