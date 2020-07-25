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
    pink = 8,
    black = 9,
    white = 10
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
            return new Color(255 / 255f, 0f, 203 / 255f, 255f);
        if (color == Colors.black)
            return Color.black;
        
        return Color.white;
    }
}
