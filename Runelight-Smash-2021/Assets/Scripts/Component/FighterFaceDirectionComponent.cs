using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]

public class FighterFaceDirectionComponent : MonoBehaviour
{
    // Required Components
    private InputComponent inputComponent;
    private ModelComponent modelComponent;

    void Start()
    {
        modelComponent = GetComponentInChildren<ModelComponent>();
        inputComponent = GetComponent<InputComponent>();

        if (!modelComponent)
        {
            Debug.LogError("Model Component is required, but does not exist.");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (inputComponent.joystick.x == 0)
        {
            return;
        }
        bool isFacingRight = inputComponent.joystick.x >= 0;

        modelComponent.UpdateSpriteDirection(isFacingRight);
    }
}
