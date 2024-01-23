using Microsoft.Extensions.Options;
using PixelBoard.MainServer.Models;
using PixelBoard.MainServer.Paduk;
using PixelBoard.MainServer.Utils;

namespace PixelBoard.MainServer.Services;

public class PadukGameService : IGameService
{
    private readonly PadukOptions _options;

    private readonly IBoardService _displayedBoard;

    private Dictionary<int, TeamInfo> _teams;

    private int?[,] _board;

    private HashSet<(int, int)> _blockedFields;

    private GameState _gameState = GameState.Init;

    public PadukGameService(IBoardService boardService, IOptions<PadukOptions> options)
    {
        _options = options.Value;
        _displayedBoard = boardService;
        _board = new int?[_options.BoardWidth, _options.BoardHeight];
        _teams = new();
        _blockedFields = new();
    }

    public void Start(IEnumerable<int> teamIds)
    {
        _teams = teamIds.ToDictionary(teamId => teamId, teamId => new TeamInfo(_options.StartBudget));
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
        SetField(x, y, team);

        // Check if any components have died due to the new move.
        ComponentScanner neighborScanner = new(_board);
        HashSet<uint> deadComponents = new();
        foreach ((int neighborX, int neighborY) in ComponentScanner.Neighbors(x, y))
        {
            uint? lives = neighborScanner.CountLives(neighborX, neighborY);
            if (lives == 0)
            {
                uint? component = neighborScanner.GetComponent(neighborX, neighborY);
                if (component is not null)
                {
                    deadComponents.Add(component.Value);
                }
            }
        }
        foreach (uint component in deadComponents)
        {
            RemoveComponent(neighborScanner, component);
        }

        // Check if the newly placed stone is alive.
        // (need a new scanner because the board has changed)
        ComponentScanner scanner = new(_board);
        if (0 == scanner.CountLives(x, y))
        {
            uint? component = scanner.GetComponent(x, y);
            if (component is not null)
            {
                RemoveComponent(scanner, component.Value);
            }
        }
    }

    private void RemoveComponent(ComponentScanner scanner, uint component)
    {
        HashSet<int> adjacentTeams = scanner.GetAdjacentTeams(component);
        if (adjacentTeams.Count == 1)
        {
            int capturer = adjacentTeams.First();
            foreach ((int x, int y) in scanner.GetComponentFields(component))
            {
                SetField(x, y, capturer);
            }
        }
        else
        {
            foreach ((int x, int y) in scanner.GetComponentFields(component))
            {
                DeleteField(x, y);
            }
        }
    }

    private void SetField(int x, int y, int team)
    {
        _board[x, y] = team;
        _displayedBoard.SetColor(x, y, Color.Palette(team));
    }

    private void DeleteField(int x, int y)
    {
        _board[x, y] = null;
        _displayedBoard.DeleteColor(x, y);
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

        public TeamInfo(int startBudget)
        {
            Score = 0;
            PaintBudget = startBudget;
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
