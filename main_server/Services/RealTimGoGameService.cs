using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class RealTimGoGameService : IGameService
{
    private readonly IBoardService _board;

    private Dictionary<int, TeamInfo> _teams;

    private GameState _gameState = GameState.Init;

    public RealTimGoGameService(IBoardService board)
    {
        _board = board;
        _teams = new();
    }

    public void Start(IEnumerable<int> teamIds)
    {
        _teams = teamIds.ToDictionary(teamId => teamId, teamId => new TeamInfo());
        _gameState = GameState.Active;
    }

    public void Stop()
    {
        _gameState = GameState.Done;
    }

    public void Tick()
    {
        // TODO: config for 16
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                // TODO: use own board representation rather than IBoardService, then compare team int rather than colors
                Color? color = _board.GetColor(x, y);
                if (color is not null)
                {
                    foreach (var (team, info) in _teams)
                    {
                        if (Color.Palette(team) == color)
                        {
                            info.Score += 1;
                        }
                    }
                }
            }
        }
    }


    public void MakeMove(int x, int y, int team)
    {
        if (_gameState is not GameState.Active)
            throw new InvalidOperationException("The game is not ready to accept moves.");

        TeamInfo? info;
        _teams.TryGetValue(team, out info);
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

    public GameState GetGameState()
    {
        return this._gameState;
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
            // TODO: config
            PaintBudget = 10;
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
