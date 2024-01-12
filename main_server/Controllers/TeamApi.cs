using Microsoft.AspNetCore.Mvc;
using PixelBoard.MainServer.Models;
using PixelBoard.MainServer.Services;

namespace PixelBoard.MainServer.RestApi;

[ApiController]
[Route("api/team")]
public class TeamApiController : ControllerBase
{
    [HttpGet("")]
    public IEnumerable<Team> GetTeams([FromServices] IPlayerService db)
    {
        return db.GetAllTeams();
    }

    [HttpGet("{id:int}")]
    public ActionResult<Team> GetTeam([FromServices] IPlayerService db, int id)
    {
        return db.GetTeam(id) switch
        {
            Team team => Ok(team),
            null => NotFound(),
        };
    }
}
