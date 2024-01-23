
namespace PixelBoard.MainServer.Utils;

/// <summary>
/// Finds each connected component on the pixel board and counts their lives.
/// </summary>
public class ComponentScanner
{
    private int?[,] _board;
    private uint?[,] _components;
    private Dictionary<uint, uint> _lives;

    public ComponentScanner(int?[,] board)
    {
        _board = board;
        _components = new uint?[_board.GetLength(0), _board.GetLength(1)];
        _lives = new();
    }

    /// <summary>
    /// Returns the number of lives for the group at a given index or null if
    /// the position is empty.
    /// </summary>
    public uint? CountLives(int x, int y)
    {
        if (IsOutside(x, y))
            return null;

        int? team = _board[x, y];
        if (team is null)
            return null;

        uint component = _components[x, y] ?? (uint)_lives.Count;

        if (!_lives.ContainsKey(component))
        {
            var lifeSpots = ComputeLives(x, y, component, team.Value);
            _lives[component] = (uint)lifeSpots.Count;
        }

        return _lives[component];
    }

    public uint? GetComponent(int x, int y)
    {
        return _components[x, y];
    }

    public IEnumerable<(int, int)> GetComponentFields(uint component)
    {
        for (int x = 0; x < _board.GetLength(0); x++)
        {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
                if (_components[x, y] == component)
                {
                    yield return (x, y);
                }
            }
        }
    }

    private HashSet<(int, int)> ComputeLives(int x, int y, uint component, int team)
    {
        HashSet<(int, int)> lifeSpots = new();

        // recursion stop guarantees each (x,y) is only computed once
        if (_components[x, y] is not null)
            return lifeSpots;
        _components[x, y] = component;

        foreach ((int neighborX, int neighborY) in Neighbors(x, y))
        {
            if (IsOutside(neighborX, neighborY))
            {
                // Outside does not count as life.
                continue;
            }
            else if (_board[neighborX, neighborY] is null)
            {
                // Found an empty spot, add it to life spots.
                // Note that the search can potentially add each a life spot
                // multiple times, using a set to deduplicate is crucial.
                lifeSpots.Add((x, y));
            }
            else if (_board[neighborX, neighborY] == team)
            {
                // Same color found, need to recurse and find transitive lives.
                var transitiveLives = ComputeLives(neighborX, neighborY, component, team);
                lifeSpots.UnionWith(transitiveLives);
            }
            // else: neighbor is of different color, hence provides no lifes
        }

        return lifeSpots;
    }

    private bool IsOutside(int x, int y)
    {
        return
            x < 0 || y < 0
            || x >= _board.GetLength(0)
            || y >= _board.GetLength(1);
    }

    public static (int, int)[] Neighbors(int x, int y)
    {
        return [
            (x-1,y),
            (x+1,y),
            (x,y-1),
            (x,y+1)
        ];
    }
}
