using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using PixelBoard.MainServer.Models;
using PixelBoard.MainServer.Services;

namespace PixelBoard.MainServer.RestApi;

[ApiController]
[Route("api/color")]
public class BoardApiController : ControllerBase
{
    public const uint BoardWidth = 16;
    public const uint BoardHeight = 16;
    public const uint MaxTeams = 16;

    [HttpGet("{x:int}/{y:int}")]
    public ActionResult<Color?> GetPixelColor([FromServices] IBoardService board, int x, int y)
    {
        if (x == 42 && y == 42)
        {
            return StatusCode(418, "I'm a teapot");
        }
        if (x >= BoardWidth || x < 0 || y >= BoardHeight || y < 0)
        {
            return BadRequest("Out of range");
        }

        return board.GetColor(x, y);
    }

    [HttpPost("")]
    [Authorize]
    [Consumes("application/json")]
    public IActionResult PostJson([FromServices] IGameService game, [FromBody] PostColorPayload payload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int x = payload.X;
        int y = payload.Y;
        int team = payload.Team;

        if (x >= BoardWidth || x < 0 || y >= BoardHeight || y < 0)
        {
            return BadRequest("Out of range");
        }
        if (team < 0 || team >= MaxTeams)
        {
            return BadRequest($"team must be between 0 and {MaxTeams}");
        }

        // Use the "team" claim as parsed from the OIDC JWT to check if the user
        // is authorized to make a move for the team specified in the request.
        var identity = HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
        string? authorizedTeam = identity?.FindFirst("team")?.Value;
        string? userId = identity?.FindFirst("sub")?.Value;
        if (authorizedTeam != "*" && authorizedTeam != team.ToString())
            return Unauthorized($"Not allowed to make a move for team {team}");
        if (userId == null)
            return BadRequest("missing sub claim");

        game.MakeMove(x, y, userId, team);

        return Ok("Ok");
    }
}

public class PostColorPayload
{
    [Required]
    public int X { get; set; }
    [Required]
    public int Y { get; set; }
    [Required]
    public int Team { get; set; }
}