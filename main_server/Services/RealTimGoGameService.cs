using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class RealTimGoGameService : IGameService
{
    private readonly IBoardService _displayedBoard;

    private Dictionary<int, TeamInfo> _teams;

    private int?[,] _board;

    private HashSet<(int, int)> _blockedFields;

    private GameState _gameState = GameState.Init;

    public RealTimGoGameService(IBoardService boardService)
    {
        _displayedBoard = boardService;
        // TODO: config for 16
        _board = new int?[16, 16];
        _teams = new();
        _blockedFields = new();
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
        for (int x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                int? team = _board[x, y];
                if (team is not null)
                {
                    _teams[team.Value].Score += 1;
                }
            }
        }
        _blockedFields.Clear();
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

        bool blocked = !_blockedFields.Add((x, y));
        if (blocked)
            throw new InvalidOperationException($"Field ({x}|{y}) has just been played.");

        // TODO: persist team info changes
        info.PaintBudget -= 1;
        _board[x, y] = team;
        _displayedBoard.SetColor(x, y, Color.Palette(team));
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
