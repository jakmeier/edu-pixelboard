using StackExchange.Redis;
using PixelBoard.MainServer.Models;
using System.Text.Json;

namespace PixelBoard.MainServer.Services;

public class RedisColorDbService : IColorDbService
{
    // for now, the simple assumption is that redis runs on the same machine
    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");

    public Color? GetColor(int x, int y)
    {
        IDatabase db = this._redis.GetDatabase();

        string? s = db.StringGet(this.Key(x, y));
        if (s == null)
        {
            return null;
        }

        // simplistic coloring if a single number was stored (for testing)
        byte red = 0;
        bool isNumber = byte.TryParse(s, out red);
        if (isNumber)
        {
            return new Color(red, 0, 0);
        }

        // otherwise, expect a serialized `Color`
        return JsonSerializer.Deserialize<Color>(s);
    }

    public void SetColor(int x, int y, Color color)
    {
        IDatabase db = this._redis.GetDatabase();
        db.StringSet(this.Key(x, y), JsonSerializer.Serialize(color));
    }

    private string Key(int x, int y)
    {
        return $"({x}|{y})";
    }
}