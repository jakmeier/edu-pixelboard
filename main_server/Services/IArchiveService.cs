namespace PixelBoard.MainServer.Services;

public interface IArchiveService
{
    void ArchiveGame(string archiveKey);
    void RotateToGame(string archiveKey);
    List<string> GetAllGameKeys();
    void DeleteGame(string archiveKey);
    void InitializeGameFromArchive(string archiveKey);
}