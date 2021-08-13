using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickComponent : MonoBehaviour
{
    public Vector2 joystick;

    public void SetJoystickInput(Vector2 input)
    {
        joystick = input;
    }
}
