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
    }

    public void Stop()
    {
        _gameState = GameState.Done;
    }

    public void Tick()
    {
        // TODO: Score calculations
        throw new NotImplementedException();
    }


    public void MakeMove(int x, int y, int team)
    {
        if (_gameState is not GameState.Active)
            throw new InvalidOperationException("The game is not ready to accept moves.");

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
