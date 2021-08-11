using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class DoubleTapHold : IInputInteraction
{
    private enum TapPhase
    {
        None,
        WaitingForNextRelease,
        WaitingForNextPress,
    }

    public float maxBufferDuration = 0.2f;

    private TapPhase tapPhase;
    private float bufferDirection;

    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }
        float direction = context.ReadValue<float>();

        switch (tapPhase)
        {
            case TapPhase.None:
                if (direction == 0)
                {
                    break;
                }
                tapPhase = TapPhase.WaitingForNextRelease;
                bufferDirection = direction;
                context.Started();
                context.SetTimeout(maxBufferDuration);
                break;
            case TapPhase.WaitingForNextPress:
                if (direction == 0)
                {
                    break;
                }
                tapPhase = TapPhase.WaitingForNextRelease;
                if (direction * bufferDirection > 0)
                {
                    context.PerformedAndStayPerformed();
                }
                else
                {
                    bufferDirection = direction;
                    context.SetTimeout(maxBufferDuration);
                }
                break;
            case TapPhase.WaitingForNextRelease:
                if (direction != 0)
                {
                    break;
                }
                if (context.phase == InputActionPhase.Performed)
                {
                    context.Canceled();
                }
                else
                {
                    tapPhase = TapPhase.WaitingForNextPress;
                }
                break;
        }
    }

    public void Reset()
    {
        tapPhase = TapPhase.None;
        bufferDirection = 0;
    }

    static DoubleTapHold()
    {
        InputSystem.RegisterInteraction<DoubleTapHold>();
    }
}