using StackExchange.Redis;
using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class RedisColorDbService : IColorDbService
{
    // for now, the simple assumption is that redis runs on the same machine
    private readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");

    public Color? GetColor(int x, int y)
    {
        IDatabase db = this._redis.GetDatabase();

        string? s = db.StringGet(String.Format("({0}|{1})", x, y));
        if (s == null)
        {
            return null;
        }
        // simplistic coloring for now, more details TBD
        byte red = 0;
        byte.TryParse(s, out red);
        return new Color(red, 0, 0);
    }
}