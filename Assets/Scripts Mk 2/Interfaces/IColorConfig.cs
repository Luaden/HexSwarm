using UnityEngine;

public enum Colors
{
    grey = 1 << 0,
    red = 1 << 1,
    blue = 1 << 2,
    yellow = 1 << 3,
    green = 1 << 4,
    orange = 1 << 5,
    purple = 1 << 6,
    pink = 1 << 7,
    black = 1 << 8,
    white = 1 << 9
}

public interface IColorConfig
{
    Color PrimaryColor { get; }

    Colors PrimaryColorCategory { get; }
}

[System.Serializable]
public class ColorConfig : IColorConfig
{
    public Teams TeamNumber;
    public Color PrimaryColor { get; set; }
    public Colors PrimaryColorCategory { get; set; }

    public Color GetColor(Colors color)
    {
        if (color == Colors.grey)
            return new Color(102/255f, 102/255f, 102/255f, 1f);
        if (color == Colors.red)
            return Color.red;
        if (color == Colors.blue)
            return Color.blue;
        if (color == Colors.yellow)
            return Color.yellow;
        if (color == Colors.green)
            return Color.green;
        if (color == Colors.orange)
            return new Color(255/255f, 112/255f, 0f, 255/255f);
        if (color == Colors.purple)
            return new Color(97/255f, 13/255f, 224/255f, 255/255f);
        if (color == Colors.pink)
            return new Color(255/255f, 0f, 203/255f, 255f);
        if (color == Colors.black)
            return Color.black;
        
        return Color.white;
    }
}
