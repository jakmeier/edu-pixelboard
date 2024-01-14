using PixelBoard.MainServer.Services;
using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

public class RealTimGoGameService : IGameService
{
    private readonly IBoardService _board;

    public RealTimGoGameService(IBoardService board)
    {
        _board = board;
    }

    public void MakeMove(int x, int y, string user, int team)
    {
        // TODO: check team matches the registered one (time of check vs time of use)
        // TODO: game rules
        _board.SetColor(x, y, Color.Palette(team));
    }
}