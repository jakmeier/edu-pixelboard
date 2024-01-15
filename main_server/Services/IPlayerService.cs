using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

/// <summary>
/// Provides access to registered players and teams.
/// </summary>
public interface IPlayerService
{
    IEnumerable<Player> GetAllPlayers();
    Player? GetPlayer(string id);

    IEnumerable<Team> GetAllTeams();
    Team? GetTeam(int id);

    void Register(string id, string name, int team);
}