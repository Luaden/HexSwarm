using UnityEngine;

public class GetPlayerInput : MonoBehaviour 
{
    protected PlayerTeam player;

    public void GetInput()
    {
        if (player == null)
            player = FindObjectOfType<AITesting>().Player;

        Debug.Log("Getting mouse input.");
        player.GetMouseInput();
    }
}
