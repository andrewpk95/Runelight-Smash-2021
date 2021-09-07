using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

[DisplayStringFormat("{firstPart}+{secondPart}")]
public class GamepadAttackComposite : InputBindingComposite<AttackInputStruct>
{
    [InputControl(layout = "Vector2")]
    public int joystick;

    [InputControl(layout = "Button")]
    public int button;

    public override AttackInputStruct ReadValue(ref InputBindingCompositeContext context)
    {
        return new AttackInputStruct()
        {
            joystick = context.ReadValue<Vector2, Vector2MagnitudeComparer>(joystick),
            button = context.ReadValue<float>(button),
        };
    }

    static GamepadAttackComposite()
    {
        InputSystem.RegisterBindingComposite<GamepadAttackComposite>();
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init() { }
}