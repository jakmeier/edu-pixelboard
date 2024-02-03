using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PixelBoard.MainServer.Services;

namespace PixelBoard.TeacherClient;

[Authorize(Policy = "Admin")]
public class AdminModel : PageModel
{
    private readonly ILogger<AdminModel> _logger;

    public AdminModel(ILogger<AdminModel> logger)
    {
        _logger = logger;
    }

    public ActionResult OnGet()
    {
        return Page();
    }

    public void OnPost([FromServices] IGameService game, [FromServices] IPlayerService players)
    {
        game.Start(players.GetAllTeamIds());
        _logger.LogWarning("Game starts");
    }
}
