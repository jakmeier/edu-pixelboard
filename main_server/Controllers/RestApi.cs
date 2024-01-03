using Microsoft.AspNetCore.Mvc;
using PixelBoard.MainServer.Models;
using PixelBoard.MainServer.Services;

namespace PixelBoard.MainServer.RestApi;

[ApiController]
[Route("api")]
public class RestApiController : ControllerBase
{
    public const uint BoardWidth = 16;
    public const uint BoardHeight = 16;

    [HttpGet("")]
    public string Api()
    {
        return "Rest API for PixelBoard running";
    }

    [HttpGet("color/{x:int}/{y:int}")]
    public ActionResult<Color?> PixelColor([FromServices] IColorDbService db, int x, int y)
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
}
