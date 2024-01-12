using Microsoft.AspNetCore.Mvc;
using PixelBoard.MainServer.Models;
using PixelBoard.MainServer.Services;

namespace PixelBoard.MainServer.RestApi;

[ApiController]
[Route("api/player")]
public class PlayerApiController : ControllerBase
{
    [HttpGet("")]
    public IEnumerable<Player> GetPlayers([FromServices] IPlayerService db)
    {
        return db.GetAllPlayers();
    }

    [HttpGet("{id:int}")]
    public ActionResult<Player> GetPlayer([FromServices] IPlayerService db, int id)
    {
        return db.GetPlayer(id) switch
        {
            Player p => Ok(p),
            null => NotFound(),
        };
    }
}
