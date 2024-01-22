using Microsoft.AspNetCore.Mvc;
using PixelBoard.MainServer.Services;

namespace PixelBoard.MainServer.RestApi;

[ApiController]
[Route("api/game")]
public class GameApiController : ControllerBase
{
    [HttpGet("")]
    public string Api()
    {
        return "Game APi is live.";
    }

    [HttpGet("team/{id:int}")]
    public ActionResult<Dictionary<string, string?>> GetTeamInfo([FromServices] IGameService db, int id)
    {
        return db.GetTeamInfo(id) switch
        {
            Dictionary<string, string> info => Ok(info),
            null => NotFound(),
        };
    }
}
