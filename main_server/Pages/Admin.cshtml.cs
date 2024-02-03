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

    public void OnPostStart([FromServices] IGameService game, [FromServices] IPlayerService players)
    {
        try
        {
            game.Start(players.GetAllTeamIds());
            _logger.LogWarning("Game started");
        }
        catch (Exception exception)
        {
            _logger.LogError("{}", exception);
        }
    }

    public void OnPostStop([FromServices] IGameService game)
    {
        try
        {

            game.Stop();
            _logger.LogWarning("Game stopped");
        }
        catch (Exception exception)
        {
            _logger.LogError("{}", exception);
        }
    }

    public void OnPostReset([FromServices] IGameService game)
    {
        try
        {
            game.Reset();
            _logger.LogWarning("Game reset");
        }
        catch (Exception exception)
        {
            _logger.LogError("{}", exception);
        }
    }
}
