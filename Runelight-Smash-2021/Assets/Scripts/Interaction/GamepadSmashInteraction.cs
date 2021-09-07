using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class GamepadSmashInteraction : IInputInteraction
{
    private enum ButtonInputState
    {
        Neutral,
        Pressed,
        WaitingForNextRelease,
    }
    private enum SmashInputState
    {
        Neutral,
        WaitingForNeutral,
        WaitingForJoystickTap,
        WaitingForButtonPress,
        Charging,
        Smash,
    }

    public float maxTapDuration = 0.05f;
    public float maxButtonPressWindow = 0.05f;

    private SmashInputState smashInputState;
    private ButtonInputState buttonInputState;
    private bool isButtonPressed;

    public void Process(ref InputInteractionContext context)
    {
        AttackInputStruct input = context.ReadValue<AttackInputStruct>();
        Vector2 joystick = input.joystick;
        float joystickMagnitude = joystick.sqrMagnitude;
        float button = input.button;

        switch (buttonInputState)
        {
            case ButtonInputState.Neutral:
                if (button >= 1.0f)
                {
                    buttonInputState = ButtonInputState.Pressed;
                    isButtonPressed = true;
                }
                break;
            case ButtonInputState.Pressed:
                buttonInputState = ButtonInputState.WaitingForNextRelease;
                isButtonPressed = false;
                break;
            case ButtonInputState.WaitingForNextRelease:
                if (button <= 0.0f)
                {
                    buttonInputState = ButtonInputState.Neutral;
                }
                break;
            default:
                break;
        }

        switch (smashInputState)
        {
            case SmashInputState.Neutral:
                if (joystickMagnitude >= 1.0f)
                {
                    if (isButtonPressed)
                    {
                        smashInputState = SmashInputState.Charging;
                        context.Started();
                    }
                    else
                    {
                        smashInputState = SmashInputState.WaitingForButtonPress;
                        context.SetTimeout(maxButtonPressWindow);
                    }
                    break;
                }
                if (joystickMagnitude > 0.0f)
                {
                    smashInputState = SmashInputState.WaitingForJoystickTap;
                    context.SetTimeout(maxTapDuration);
                }
                break;
            case SmashInputState.WaitingForJoystickTap:
                if (joystickMagnitude >= 1.0f)
                {
                    if (isButtonPressed)
                    {
                        smashInputState = SmashInputState.Charging;
                        context.Started();
                        break;
                    }
                    smashInputState = SmashInputState.WaitingForButtonPress;
                    context.SetTimeout(maxButtonPressWindow);
                }
                else if (context.timerHasExpired || isButtonPressed)
                {
                    smashInputState = SmashInputState.WaitingForNeutral;
                }
                break;
            case SmashInputState.WaitingForButtonPress:
                if (isButtonPressed)
                {
                    smashInputState = SmashInputState.Charging;
                    context.Started();
                }
                else if (context.timerHasExpired)
                {
                    smashInputState = SmashInputState.WaitingForNeutral;
                }
                break;
            case SmashInputState.Charging:
                if (button <= 0.0f)
                {
                    smashInputState = SmashInputState.Smash;
                    context.Performed();
                }
                break;
            case SmashInputState.Smash:
                smashInputState = SmashInputState.WaitingForNeutral;
                break;
            case SmashInputState.WaitingForNeutral:
                if (joystickMagnitude <= 0.0f && !isButtonPressed)
                {
                    smashInputState = SmashInputState.Neutral;
                }
                break;
            default:
                break;
        }
    }

    public void Reset() { }

    static GamepadSmashInteraction()
    {
        InputSystem.RegisterInteraction<GamepadSmashInteraction>();
    }
}