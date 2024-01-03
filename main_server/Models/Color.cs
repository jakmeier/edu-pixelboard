namespace PixelBoard.MainServer.Models;

public class Color
{
    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }

    public Color(byte Red, byte Green, byte Blue)
    {
        this.Red = Red;
        this.Green = Green;
        this.Blue = Blue;
    }

    private static readonly Color[] Colors = {
        new Color(255,0,0),
        new Color(0,255,0),
        new Color(0,0,255),
        new Color(255,255,0),
        // TODO more colors and make them nice
    };

    public static Color Palette(int index)
    {
        index = Math.Abs(index);
        Color baseColor = Colors[index % Colors.Length];
        int shade = index / Colors.Length;
        if (shade == 0)
        {
            return baseColor;
        }
        int factor = shade;
        return new Color(
            (byte)(baseColor.Red / factor),
            (byte)(baseColor.Green / factor),
            (byte)(baseColor.Blue / factor)
        );
    }
}