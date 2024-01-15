using PixelBoard.MainServer.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace PixelBoard.MainServer.Services;

public class PlayerService : IPlayerService
{
    private const string NumTeamDbKey = "int:numTeams";
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

    public Player? GetPlayer(string id)
    {
        IDatabase db = _redis.GetConnection();

        string? s = db.StringGet(this.PlayerKey(id));
        if (s == null)
            return null;

        return JsonSerializer.Deserialize<Player>(s);
    }

    public Team? GetTeam(int id)
    {
        IDatabase db = _redis.GetConnection();

        string? s = db.StringGet(this.TeamKey(id));
        if (s == null)
            return null;

        return JsonSerializer.Deserialize<Team>(s);
    }

    public void Register(string id, string name, int teamId)
    {
        if (null != this.GetPlayer(id))
            throw new BadApiRequestException("player already registered");

        IDatabase db = _redis.GetConnection();
        if (null == this.GetTeam(teamId))
        {
            Team team = new($"{name}'s team", this.NextTeamColor());
            db.StringSet(this.TeamKey(teamId), JsonSerializer.Serialize(team));
        }

        Player player = new(name, teamId);
        db.StringSet(this.PlayerKey(id), JsonSerializer.Serialize(player));
    }

    private string PlayerKey(string playerId)
    {
        return $"player:{playerId}:";
    }

    private string TeamKey(int teamId)
    {
        return $"team:{teamId}:";
    }

    private Color NextTeamColor()
    {
        IDatabase db = _redis.GetConnection();
        string? s = db.StringGet(NumTeamDbKey);
        int seq = s == null ? 0 : int.Parse(s);
        db.StringSet(NumTeamDbKey, seq + 1);
        return Color.Palette(seq);
    }
}