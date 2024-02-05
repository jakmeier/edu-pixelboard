namespace PixelBoard.MainServer.Paduk;

public class PadukOptions
{
    public int BoardWidth { get; set; }
    public int BoardHeight { get; set; }
    public int StartBudget { get; set; }
    public int MaxBudget { get; set; }
    public bool StartWithCheckerboard { get; set; }
    /// <summary>
    /// Increase the paint budget of every team after N ticks.
    /// Do not increase frequently if set to 0.
    /// </summary>
    public int BudgetIncreaseDelay { get; set; }
    public int BudgetIncreaseSize { get; set; }
    public int TickDelayMs { get; set; }

    public PadukOptions()
    {
        BoardWidth = 16;
        BoardHeight = 16;
        StartBudget = 10;
        MaxBudget = 10;
        BudgetIncreaseDelay = 10;
        BudgetIncreaseSize = 1;
        StartWithCheckerboard = false;
        TickDelayMs = 1000;
    }
}