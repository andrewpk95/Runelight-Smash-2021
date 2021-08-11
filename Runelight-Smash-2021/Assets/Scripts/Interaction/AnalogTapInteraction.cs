using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class AnalogTapInteraction : IInputInteraction
{
    public float maxTapDuration = 0.05f;
    public float maxNeutralWaitDuration = 0.05f;

    private bool wasJoystickBackToNeutral = false;
    private bool neutralWaitStarted = false;
    private double neutralWaitStartTime;

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
                    if (!neutralWaitStarted)
                    {
                        neutralWaitStarted = true;
                        neutralWaitStartTime = context.time;
                    }
                    else if (context.time - neutralWaitStartTime >= maxNeutralWaitDuration)
                    {
                        context.Canceled();
                        break;
                    }
                }
                else
                {
                    neutralWaitStarted = false;
                    neutralWaitStartTime = 0;
                }
                context.PerformedAndStayPerformed();
                break;
        }
    }

    public void Reset()
    {
        neutralWaitStarted = false;
        neutralWaitStartTime = 0;
    }

    static AnalogTapInteraction()
    {
        InputSystem.RegisterInteraction<AnalogTapInteraction>();
    }
}