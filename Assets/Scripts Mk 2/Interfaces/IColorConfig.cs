using UnityEngine;

public enum Colors
{
    grey = 1,
    red = 2,
    blue = 3,
    yellow = 4,
    green = 5,
    orange = 6,
    purple = 7,
    black = 8,
    white = 9
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
    public Teams TeamNumber;
    public Color PrimaryColor { get; set; }
    public Color SecondaryColor { get; set; }
    public Colors PrimaryColorCategory { get; set; }
    public Colors SecondaryColorCategory { get; set; }

    public Color GetColor(Colors color)
    {
        if (color == Colors.grey)
            return new Color(102/255, 102/255, 102/255, 1);
        if (color == Colors.red)
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
