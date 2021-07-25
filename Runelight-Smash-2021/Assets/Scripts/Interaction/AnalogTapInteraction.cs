using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class AnalogTapInteraction : IInputInteraction
{
    public float maxTapDuration = 0.1f;
    private bool isTimerStarted = false;

    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }
        Vector2 direction = context.ReadValue<Vector2>();

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (direction.SqrMagnitude() <= 0)
                {
                    context.Started();
                }
                break;

            case InputActionPhase.Started:
                if (direction.SqrMagnitude() > 0 && !isTimerStarted)
                {
                    context.SetTimeout(maxTapDuration);
                    isTimerStarted = true;
                }
                if (direction.SqrMagnitude() >= 1)
                {
                    context.Performed();
                }
                break;
        }
    }

    public void Reset()
    {
        isTimerStarted = false;
    }

    static AnalogTapInteraction()
    {
        InputSystem.RegisterInteraction<AnalogTapInteraction>();
    }
}