using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class DoubleTapButton : IInputInteraction
{
    private enum TapPhase
    {
        None,
        WaitingForNextRelease,
        WaitingForNextPress,
    }

    public float maxBufferDuration = 0.2f;

    private TapPhase tapPhase;

    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }
        bool isPressed = context.ControlIsActuated();

        switch (tapPhase)
        {
            case TapPhase.None:
                if (isPressed)
                {
                    tapPhase = TapPhase.WaitingForNextRelease;
                    context.Started();
                    context.SetTimeout(maxBufferDuration);
                }
                break;
            case TapPhase.WaitingForNextRelease:
                if (isPressed)
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
            case TapPhase.WaitingForNextPress:
                if (!isPressed)
                {
                    break;
                }
                context.Performed();
                break;
        }
    }

    public void Reset()
    {
        tapPhase = TapPhase.None;
    }

    static DoubleTapButton()
    {
        InputSystem.RegisterInteraction<DoubleTapButton>();
    }
}