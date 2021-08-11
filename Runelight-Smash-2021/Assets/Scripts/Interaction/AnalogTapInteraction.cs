using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class AnalogTapInteraction : IInputInteraction
{
    public float maxTapDuration = 0.1f;
    private bool wasJoystickBackToNeutral = false;

    public void Process(ref InputInteractionContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        float joystickMagnitude = direction.SqrMagnitude();

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (joystickMagnitude > 0 && wasJoystickBackToNeutral)
                {
                    context.Started();
                    if (joystickMagnitude >= 1)
                    {
                        context.PerformedAndStayPerformed();
                    }
                    else
                    {
                        context.SetTimeout(maxTapDuration);
                        wasJoystickBackToNeutral = false;
                    }

                }
                else if (joystickMagnitude <= 0 && !wasJoystickBackToNeutral)
                {
                    wasJoystickBackToNeutral = true;
                }
                break;

            case InputActionPhase.Started:
                if (joystickMagnitude >= 1)
                {
                    context.PerformedAndStayPerformed();
                }
                else if (context.timerHasExpired)
                {
                    context.Canceled();
                }
                break;
            case InputActionPhase.Performed:
                if (joystickMagnitude <= 0)
                {
                    wasJoystickBackToNeutral = true;
                    context.Canceled();
                }
                else
                {
                    context.PerformedAndStayPerformed();
                }
                break;
        }
    }

    public void Reset() { }

    static AnalogTapInteraction()
    {
        InputSystem.RegisterInteraction<AnalogTapInteraction>();
    }
}