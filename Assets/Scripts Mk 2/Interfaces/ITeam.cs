using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam 
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }    
    Teams TeamNumber { get; }
    Vector3Int StartPosition { get; }
    IEnumerable<IUnit> Units { get; }
}
