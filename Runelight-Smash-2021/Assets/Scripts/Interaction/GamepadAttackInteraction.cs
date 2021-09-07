using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class GamepadAttackInteraction : IInputInteraction
{
    public float maxTapDuration = 0.05f;

    private bool wasButtonReleased;

    public void Process(ref InputInteractionContext context)
    {
        AttackInputStruct input = context.ReadValue<AttackInputStruct>();
        Vector2 joystick = input.joystick;
        float joystickMagnitude = joystick.sqrMagnitude;
        float button = input.button;

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (button >= 1.0f && wasButtonReleased)
                {
                    context.Performed();
                    wasButtonReleased = false;
                }
                else if (button <= 0.0f && !wasButtonReleased)
                {
                    wasButtonReleased = true;
                }
                break;
        }
    }

    public void Reset() { }

    static GamepadAttackInteraction()
    {
        InputSystem.RegisterInteraction<GamepadAttackInteraction>();
    }
}