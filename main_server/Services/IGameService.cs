namespace PixelBoard.MainServer.Services;

/// <summary>
/// Game logic for a game that can be played on the pixel board.
/// </summary>
public interface IGameService
{
    /// <summary>
    ///  A user activates a pixel in the name of their team.
    /// </summary>
    /// <param name="x">pixel coordinate</param>
    /// <param name="y">pixel coordinate</param>
    /// <param name="user">the unique user identifier, must be authenticated</param>
    /// <param name="team">the team number for which the move is , must be authenticated</param>
    void MakeMove(int x, int y, string user, int team);

    Task<Dictionary<string, string?>> GetTeamInfo(int team);
}