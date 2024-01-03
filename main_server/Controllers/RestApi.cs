using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using PixelBoard.MainServer.Models;
using PixelBoard.MainServer.Services;

namespace PixelBoard.MainServer.RestApi;

[ApiController]
[Route("api")]
public class RestApiController : ControllerBase
{
    public const uint BoardWidth = 16;
    public const uint BoardHeight = 16;
    public const uint MaxTeams = 16;

    [HttpGet("")]
    public string Api()
    {
        return "Rest API for PixelBoard running";
    }

    [HttpGet("color/{x:int}/{y:int}")]
    public ActionResult<Color?> GetPixelColor([FromServices] IColorDbService db, int x, int y)
    {
        if (x == 42 && y == 42)
        {
            return StatusCode(418, "I'm a teapot");
        }
        if (x >= BoardWidth || x < 0 || y >= BoardHeight || y < 0)
        {
            return BadRequest("Out of range");
        }

        return db.GetColor(x, y);
    }

    [HttpPost("color")]
    [Consumes("application/json")]
    public IActionResult PostJson([FromServices] IColorDbService db, [FromBody] PostColorPayload payload)
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
        // TODO: authorization
        // TODO: probably also do some game logic for account the update
        db.SetColor(x, y, Color.Palette(team));
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