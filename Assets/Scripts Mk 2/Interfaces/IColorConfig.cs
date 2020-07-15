using UnityEngine;

public enum Colors
{
    grey = 0,
    red = 1,
    blue = 2,
    yellow = 3,
    green = 4,
    orange = 5,
    purple = 6,
    black = 7,
    white = 8
}


public interface IColorConfig
{
    Color PrimaryColor { get; }
    Color SecondaryColor { get; }

    Colors PrimaryColorCategory { get; }
    Colors SecondaryColorCategory { get; }
}

[System.Serializable]
public class ColorConfig : IColorConfig
{
    public Color PrimaryColor { get; set; }
    public Color SecondaryColor { get; set; }
    public Colors PrimaryColorCategory { get; set; }
    public Colors SecondaryColorCategory { get; set; }

    public Color GetColor(Colors color)
    {
        if(color == Colors.red)
            return Color.red;
        if (color == Colors.blue)
            return Color.blue;
        if (color == Colors.yellow)
            return Color.yellow;
        if (color == Colors.green)
            return Color.green;
        if (color == Colors.orange)
            return new Color(255 / 255, 112 / 255, 0, 255 / 255);
        if (color == Colors.purple)
            return new Color(97 / 255, 13 / 255, 224 / 255, 255 / 255);
        if (color == Colors.black)
            return Color.black;
        
        return Color.white;
    }
}
