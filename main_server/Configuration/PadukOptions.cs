namespace PixelBoard.MainServer.Paduk;

public class PadukOptions
{
    public int BoardWidth { get; set; }
    public int BoardHeight { get; set; }
    public int StartBudget { get; set; }

    public PadukOptions()
    {
        BoardWidth = 16;
        BoardHeight = 16;
        StartBudget = 10;
    }
}