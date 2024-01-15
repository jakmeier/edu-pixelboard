using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using PixelBoard.MainServer.Models;
using PixelBoard.MainServer.Services;

namespace PixelBoard.MainServer.RestApi;

[ApiController]
[Route("api/player")]
public class PlayerApiController : ControllerBase
{
    [HttpGet("")]
    public IEnumerable<Player> GetPlayers([FromServices] IPlayerService players)
    {
        return players.GetAllPlayers();
    }

    [HttpGet("{id}")]
    public ActionResult<Player> GetPlayer([FromServices] IPlayerService players, string id)
    {
        return players.GetPlayer(id) switch
        {
            Player p => Ok(p),
            null => NotFound(),
        };
    }


    [Authorize]
    [HttpPost("register")]
    public ActionResult<int> PostRegistration([FromServices] IPlayerService players, [FromBody] RegistrationPayload payload)
    {
        // Use the "team" claim as parsed from the OIDC JWT to check if the user
        // is authorized to make a move for the team specified in the request.
        var identity = HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
        string? id = identity?.FindFirst("sub")?.Value;
        string? name = identity?.FindFirst("name")?.Value;
        string? team = identity?.FindFirst("team")?.Value;
        int teamNumber;
        bool teamIsNumber = int.TryParse(team, out teamNumber);

        if (id == null)
            return BadRequest("missing sub claim");
        if (name == null)
            return BadRequest("missing name claim");
        if (team == null)
            return Unauthorized($"No team authorization found in token claims");
        if (!teamIsNumber)
            return BadRequest($"Team number could not be parsed from {team}");

        players.Register(id, name, teamNumber);

        return Ok($"Registered user {id}");
    }
}

public class RegistrationPayload
{
    [Required]
    public int X { get; set; }
}