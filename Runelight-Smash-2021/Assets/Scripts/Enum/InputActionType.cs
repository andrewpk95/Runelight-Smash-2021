using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Dash,
    Jump,
    Crouch,
    FallThrough,
    Attack,
    Special,
    Shield,
    Grab,
    None,
}

public enum ActionDirection
{
    Right,
    Left,
    Up,
    Down,
    None,
}

public enum ActionStrength
{
    Strong,
    Weak,
    None,
}