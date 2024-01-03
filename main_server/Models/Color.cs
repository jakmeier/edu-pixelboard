namespace PixelBoard.MainServer.Models;

public class Color
{
    public byte red { get; set; }
    public byte green { get; set; }
    public byte blue { get; set; }

    public Color(byte r, byte g, byte b)
    {
        this.red = r;
        this.green = g;
        this.blue = b;
    }
}