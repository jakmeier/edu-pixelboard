using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class PlayerService : IPlayerService
{
    private readonly IRedisDbService _redis;

    public PlayerService(IRedisDbService redis)
    {
        _redis = redis;
    }

    public IEnumerable<Player> GetAllPlayers()
    {
        // TODO
        return new List<Player>();
    }

    public IEnumerable<Team> GetAllTeams()
    {
        // TODO
        return new List<Team>();
    }

    public Player? GetPlayer(int id)
    {
        // TODO
        return new Player($"Tester{id}", id % 3);
    }

    public Team? GetTeam(int id)
    {
        // TODO
        return new Team($"Team{id}", Color.Palette(id));
    }

    public void Register(string id, string name, int team)
    {
        // TODO: persist in DB
        throw new NotImplementedException();
    }
}