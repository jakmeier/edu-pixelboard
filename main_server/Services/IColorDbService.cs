using PixelBoard.MainServer.Models;

namespace PixelBoard.MainServer.Services;

/// <summary>
/// Provides access to a source which assigns colors to each pixel in the
/// pixel board.
/// </summary>
public interface IColorDbService
{
    Color? GetColor(int x, int y);
}