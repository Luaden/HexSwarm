using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : Team
{
    public Player(GameManager game, string name, string description, Sprite icon, TileBase tile) 
        : base(game, name, description,icon, tile) { }

    public override void StartTurn()
    {
        
    }
}
