using System.Reactive.Linq;
using GraphQL;
using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class BoardQuery
{
    static public GqlPixel Pixel(int x, int y)
    {
        return new(x, y);
    }
}

public class BoardSubscription
{
    static public IObservable<Pixel> PixelChanged([FromServices] IReadBoardService readBoardService)
    {
        return readBoardService.PixelChanges();
    }
}

[Name("ExtendedPixel")]
public class GqlPixel
{
    public int X { get; set; }
    public int Y { get; set; }
    public Color Color([FromServices] IReadBoardService readBoardService)
    {
        return readBoardService.GetColor(X, Y) ?? Models.Color.Black();
    }

    public uint? Lives([FromServices] IGameService game)
    {
        return game.Lives(X, Y);
    }

    public GqlPixel(int x, int y)
    {
        X = x;
        Y = y;
    }
}
