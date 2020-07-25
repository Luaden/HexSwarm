using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameOptions
{
    float GameDifficulty { get;  }
    MapShape MapShape { get; }
    ColorConfig[] TeamColors { get; }

    void ChangeTeamColor(int teamIndex, ColorConfig colors);
}
