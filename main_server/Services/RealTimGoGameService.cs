using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class RealTimGoGameService : IGameService
{
    private readonly IBoardService _board;

    private Dictionary<int, TeamInfo> _teams;

    public RealTimGoGameService(IBoardService board)
    {
        _board = board;
        _teams = new();
    }

    public void MakeMove(int x, int y, int team)
    {
        TeamInfo? info = _teams[team];
        if (info is null)
            throw new InvalidOperationException($"Team {team} is not registered.");
        if (info.PaintBudget <= 0)
            throw new InvalidOperationException($"Team {team} has no paint to make a move.");
        // TODO: check field has not been colored since last tick

        // TODO: persist team info changes
        info.PaintBudget -= 1;
        _board.SetColor(x, y, Color.Palette(team));
    }


    public Dictionary<string, string?>? GetTeamInfo(int team)
    {
        return _teams[team]?.ToDictionary();
    }

    /// <summary>
    /// Private class to define in-memory data layout per team.
    /// Don't use in other classes.
    /// </summary>
    private class TeamInfo
    {
        public int Score { get; set; }
        public int PaintBudget { get; set; }

        public TeamInfo()
        {
            Score = 0;
            PaintBudget = 0;
        }

        public Dictionary<string, string?> ToDictionary()
        {
            return new Dictionary<string, string?>() {
                {"Score",Score.ToString()},
                {"PaintBudget",PaintBudget.ToString()},
            };
        }
    }
}
