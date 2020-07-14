using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameOptions
{
    float GameDifficulty { get; set; }
    MapShape MapShape { get; set; }
    IColorConfig[] TeamColors { get; set; }
}
