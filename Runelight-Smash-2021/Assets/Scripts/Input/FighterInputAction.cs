using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FighterInputAction
{
    public ActionType type;
    public Vector2 input;
    public ActionStrength strength;

    public static FighterInputAction none = new FighterInputAction(ActionType.None, ActionStrength.None);

    public FighterInputAction(ActionType type, ActionStrength strength)
    {
        this.type = type;
        this.strength = strength;
        this.input = Vector2.zero;
    }

    public FighterInputAction(ActionType type, ActionStrength strength, bool pressed)
    {
        this.type = type;
        this.strength = strength;
        this.input = Vector2.one * (pressed ? 1 : 0);
    }

    public FighterInputAction(ActionType type, ActionStrength strength, float amount)
    {
        this.type = type;
        this.strength = strength;
        this.input = Vector2.right * amount;
    }

    public FighterInputAction(ActionType type, ActionStrength strength, Vector2 direction)
    {
        this.type = type;
        this.strength = strength;
        this.input = direction;
    }

    public T ReadValue<T>() where T : struct
    {
        if (typeof(T) == typeof(bool))
        {
            return (T)Convert.ChangeType(this.input.x > 0, typeof(T));
        }
        if (typeof(T) == typeof(float))
        {
            return (T)Convert.ChangeType(this.input.x, typeof(T));
        }
        if (typeof(T) == typeof(Vector2))
        {
            return (T)Convert.ChangeType(this.input, typeof(T));
        }
        throw new Exception($"[FighterInputAction] ReadValue<T> Error: Cannot convert to given generic type {typeof(T).FullName}");
    }

    public ActionDirection GetActionDirection()
    {
        if (Vector2.Angle(Vector2.right, input) <= 45.0f)
        {
            return ActionDirection.Right;
        }
        if (Vector2.Angle(Vector2.up, input) <= 45.0f)
        {
            return ActionDirection.Up;
        }
        if (Vector2.Angle(Vector2.left, input) <= 45.0f)
        {
            return ActionDirection.Left;
        }
        return ActionDirection.Down;
    }
}
