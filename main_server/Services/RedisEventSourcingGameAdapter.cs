// note: ChatGPT 3.5 wrote almost all of the code in this file.

using System.Text.Json;
using StackExchange.Redis;

namespace PixelBoard.MainServer.Services;

public class RedisEventSourcingGameAdapter : IGameService, IArchiveService
{
    private static readonly string RedisKeyPrefix = "game";
    private readonly IRedisDbService _redisConnection;
    private readonly IDatabase _redisDatabase;
    private readonly IGameService _originalGameService;
    private readonly ILogger<RedisEventSourcingGameAdapter> _logger;

    public RedisEventSourcingGameAdapter(
           IRedisDbService redisConnection,
           IGameService originalGameService,
           ILogger<RedisEventSourcingGameAdapter> logger)
    {
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _redisDatabase = _redisConnection.GetConnection();
        _originalGameService = originalGameService ?? throw new ArgumentNullException(nameof(originalGameService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Automatically replay events when the adapter is constructed
        ReplayEvents();
    }

    public void MakeMove(int x, int y, int team)
    {
        // Forward the request to the original game service
        _originalGameService.MakeMove(x, y, team);

        // Persist the move event to Redis
        var moveEvent = new MoveEvent { X = x, Y = y, Team = team, Timestamp = DateTime.UtcNow };
        StoreEvent(moveEvent);
    }

    public void Start(IEnumerable<int> teamIds)
    {
        // Forward the request to the original game service
        _originalGameService.Start(teamIds);

        // Persist the start event to Redis
        var startEvent = new StartEvent { TeamIds = teamIds.ToList(), Timestamp = DateTime.UtcNow };
        StoreEvent(startEvent);
    }

    public void Stop()
    {
        // Forward the request to the original game service
        _originalGameService.Stop();

        // Persist the stop event to Redis
        var stopEvent = new StopEvent { Timestamp = DateTime.UtcNow };
        StoreEvent(stopEvent);
    }

    public void Reset()
    {
        // Forward the request to the original game service
        _originalGameService.Reset();

        // Persist the reset event to Redis
        var resetEvent = new ResetEvent { Timestamp = DateTime.UtcNow };
        StoreEvent(resetEvent);
    }

    public void Tick()
    {
        // Forward the request to the original game service
        _originalGameService.Tick();

        // Persist the tick event to Redis
        var tickEvent = new TickEvent { Timestamp = DateTime.UtcNow };
        StoreEvent(tickEvent);
    }

    public Dictionary<string, string?>? GetTeamInfo(int team)
    {
        return _originalGameService.GetTeamInfo(team);
    }

    public GameState GetGameState()
    {
        return _originalGameService.GetGameState();
    }

    private void StoreEvent(GameEvent gameEvent)
    {
        var serializedEvent = JsonSerializer.Serialize(gameEvent);
        _redisDatabase.ListLeftPush(GetCurrentGameKey(), serializedEvent);
    }

    private void ReplayEvents()
    {
        ReplayEvents(GetCurrentGameKey());
    }

    private void ReplayEvents(string key)
    {
        // Retrieve and replay events from Redis
        var events = _redisDatabase.ListRange(key);
        foreach (var serializedEvent in events)
        {
            var gameEvent = JsonSerializer.Deserialize<GameEvent>(serializedEvent!);
            ReplayEvent(gameEvent);
        }
    }

    private void ReplayEvent(GameEvent? gameEvent)
    {
        // Replay the event by calling the appropriate method on the original game service
        switch (gameEvent)
        {
            case MoveEvent moveEvent:
                _originalGameService.MakeMove(moveEvent.X, moveEvent.Y, moveEvent.Team);
                break;
            case StartEvent startEvent:
                _originalGameService.Start(startEvent.TeamIds);
                break;
            case StopEvent _:
                _originalGameService.Stop();
                break;
            case ResetEvent _:
                _originalGameService.Reset();
                break;
            case TickEvent _:
                _originalGameService.Tick();
                break;
            default:
                _logger.LogWarning($"Unknown event type: {gameEvent?.GetType().Name}");
                break;
        }
    }

    // IArchiveService implementation

    public void ArchiveGame(string archiveKey)
    {
        var currentGameKey = GetCurrentGameKey();
        var events = _redisDatabase.ListRange(currentGameKey);
        _redisDatabase.ListLeftPush(archiveKey, events);
        _redisDatabase.KeyDelete(currentGameKey);
    }

    public void RotateToGame(string archiveKey)
    {
        // Replay events from the specified archive key
        ReplayEvents(archiveKey);
    }

    public List<string> GetAllGameKeys()
    {
        var server = _redisConnection.GetServer();
        var keys = server.Keys(pattern: $"{RedisKeyPrefix}*");
        return keys.Select(k => k.ToString()).ToList();
    }

    public void DeleteGame(string archiveKey)
    {
        _redisDatabase.KeyDelete(archiveKey);
    }

    public void InitializeGameFromArchive(string archiveKey)
    {
        // Replay events from the specified archive key to initialize the game
        ReplayEvents(archiveKey);
    }

    // Helper methods

    private string GetCurrentGameKey()
    {
        return $"{RedisKeyPrefix}:{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}

// Define event classes
public abstract class GameEvent
{
    public DateTime Timestamp { get; set; }
}

public class MoveEvent : GameEvent
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Team { get; set; }
}

public class StartEvent : GameEvent
{
    public required List<int> TeamIds { get; set; }
}

public class StopEvent : GameEvent { }

public class ResetEvent : GameEvent { }

public class TickEvent : GameEvent { }
