using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class PlayerService : IPlayerService
{
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
}