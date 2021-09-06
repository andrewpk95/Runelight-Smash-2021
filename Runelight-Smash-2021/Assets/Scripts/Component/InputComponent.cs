using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
    // Required Variables

    // Public Continuous Input State
    public Vector2 joystick;

    // Public Action Input State
    public FighterInputAction actionInput;

    public void SetFighterActionInput(FighterInputAction action)
    {
        actionInput = action;
    }

    public void SetJoystickInput(Vector2 input)
    {
        joystick = input;
    }

    public bool isDashStarted()
    {
        return actionInput.type == ActionType.Dash && actionInput.strength != ActionStrength.None;
    }

    public bool isDashCanceled()
    {
        return actionInput.type == ActionType.Dash && actionInput.strength == ActionStrength.None;
    }
}
