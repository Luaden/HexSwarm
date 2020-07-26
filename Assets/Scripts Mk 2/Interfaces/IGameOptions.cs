using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameOptions
{
    float GameDifficulty { get;  }
    MapShape MapShape { get; }
    Dictionary<Teams, ColorConfig> TeamColors { get; set; }

    void ChangeTeamColor(Teams team, ColorConfig colors);
}
