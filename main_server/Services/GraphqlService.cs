using GraphQL;
using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class BoardQuery
{
    static public Pixel Pixel([FromServices] IReadBoardService readBoardService, int x, int y)
    {
        Color? color = readBoardService.GetColor(x, y);
        return new Pixel(x, y, color ?? Color.Black());
    }
}

public class BoardSubscription
{
    static public IObservable<Pixel> PixelChanged([FromServices] IReadBoardService readBoardService)
    {
        return readBoardService.PixelChanges();
    }
}
