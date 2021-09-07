using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class GamepadFlickInteraction : IInputInteraction
{
    public float minMagnitudeThreshold = 0.5f;

    private bool wasJoystickBackToNeutral;

    public void Process(ref InputInteractionContext context)
    {
        Vector2 joystick = context.ReadValue<Vector2>();
        float joystickMagnitude = joystick.magnitude;

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (joystickMagnitude >= minMagnitudeThreshold)
                {
                    context.PerformedAndStayPerformed();
                }
                break;
            case InputActionPhase.Performed:
                if (joystickMagnitude < minMagnitudeThreshold)
                {
                    context.Canceled();
                }
                break;
        }
    }

    public void Reset() { }

    static GamepadFlickInteraction()
    {
        InputSystem.RegisterInteraction<GamepadFlickInteraction>();
    }
}