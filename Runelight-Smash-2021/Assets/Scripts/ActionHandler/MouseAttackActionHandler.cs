using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Change to Custom Composite Interaction
public class MouseAttackActionHandler : MonoBehaviour
{
    public float MAX_DRAG_DURATION = 0.1f;
    private Coroutine dragTimer;

    private bool isClicked = false;
    private Vector2 accumulatedDragVector = new Vector2();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void HandleClick(InputAction.CallbackContext input, System.Action OnRelease)
    {
        if (input.performed)
        {
            if (dragTimer != null)
            {
                return;
            }
            isClicked = true;
            dragTimer = StartCoroutine(DragTimer(OnRelease));
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (input.canceled)
        {
            if (dragTimer != null)
            {
                StopCoroutine(dragTimer);
                OnRelease();
                Reset();
            }
        }
    }

    public void HandleLeftClick(InputAction.CallbackContext input)
    {
        System.Action OnRelease = () =>
        {
            Debug.Log($"Attack {GetDirectionAngle(accumulatedDragVector)} degree {GetMagnitude(accumulatedDragVector)} strength");

        };

        HandleClick(input, OnRelease);
    }

    public void HandleRightClick(InputAction.CallbackContext input)
    {
        System.Action OnRelease = () =>
        {
            Debug.Log($"Special {GetDirectionAngle(accumulatedDragVector)} degree {GetMagnitude(accumulatedDragVector)} strength");
        };

        HandleClick(input, OnRelease);
    }

    public void HandleDrag(InputAction.CallbackContext input)
    {
        if (!input.performed)
        {
            return;
        }
        if (isClicked)
        {
            accumulatedDragVector += input.ReadValue<Vector2>();
        }
    }

    private float GetDirectionAngle(Vector2 direction)
    {
        return Vector2.SignedAngle(Vector2.right, direction);
    }

    private float GetMagnitude(Vector2 direction)
    {
        return Vector2.SqrMagnitude(direction);
    }

    private void Reset()
    {
        accumulatedDragVector = new Vector2();
        isClicked = false;
        dragTimer = null;
    }

    IEnumerator DragTimer(System.Action OnRelease)
    {
        yield return new WaitForSeconds(MAX_DRAG_DURATION);
        OnRelease();
        Reset();
    }
}
