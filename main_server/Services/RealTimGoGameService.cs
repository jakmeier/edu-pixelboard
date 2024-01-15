using PixelBoard.MainServer.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace PixelBoard.MainServer.Services;

public class RealTimGoGameService : IGameService
{
    private readonly IBoardService _board;
    private readonly IRedisDbService _redis;

    public RealTimGoGameService(IBoardService board, IRedisDbService redis)
    {
        _board = board;
        _redis = redis;
    }

    public void MakeMove(int x, int y, string user, int team)
    {
        // TODO: check team matches the registered one (time of check vs time of use)
        // TODO: game rules
        _board.SetColor(x, y, Color.Palette(team));
    }


    public async Task<Dictionary<string, string?>> GetTeamInfo(int team)
    {
        IDatabase db = _redis.GetConnection();
        string? infoString = await db.StringGetAsync($"rtgo_team:{team}");

        if (infoString == null)
            return new TeamInfo().ToDictionary();

        var info = JsonSerializer.Deserialize<TeamInfo>(infoString);

        if (info == null)
        {
            Console.WriteLine($"Corrupt data in DB, cannot decode {infoString} into RTGO TeamInfo field");
            return new Dictionary<string, string?>() {
                {"Status", "Corrupt Data"},
            };
        }

        return info.ToDictionary();
    }

    /// <summary>
    /// Private class to define DB layout. Don't use in other classes.
    /// </summary>
    private class TeamInfo
    {
        public int Score { get; set; }
        public int PaintBudget { get; set; }
        public DateTime? Locked { get; set; }

        public TeamInfo()
        {
            Score = 0;
            PaintBudget = 0;
            Locked = null;
        }

        public Dictionary<string, string?> ToDictionary()
        {
            return new Dictionary<string, string?>() {
                {"Score",Score.ToString()},
                {"PaintBudget",PaintBudget.ToString()},
                {"Locked", Locked?.ToString("o")}
            };
        }
    }
}
