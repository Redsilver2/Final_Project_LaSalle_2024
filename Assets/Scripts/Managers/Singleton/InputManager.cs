using Redsilver2.Core.Player;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;

    public PlayerControls PlayerControls {
        get
        {
            if(playerControls == null)
            {
                playerControls = new PlayerControls();
            }

            return playerControls;
        }   
    }

    public void SetPlayerControlsState(bool isEnabled)
    {

        if (playerControls != null)
        {
            if (isEnabled)
            {
                playerControls.Movement.Enable();
                playerControls.Inventory.Enable();
                playerControls.Interaction.Enable();
                playerControls.Camera.Enable();
            }
            else
            {
                playerControls.Movement.Disable();
                playerControls.Inventory.Disable();
                playerControls.Interaction.Disable();
                playerControls.Camera.Disable();
            }
        }
    }
}
