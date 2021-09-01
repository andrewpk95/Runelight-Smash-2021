using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public bool IsObjectEnabled { get { return isObjectEnabled; } }

    private bool isObjectEnabled = true;

    void FixedUpdate()
    {
        if (!isObjectEnabled)
        {
            return;
        }

        Tick();
    }

    protected abstract void Tick();

    public void SetEnabled(bool enabled)
    {
        isObjectEnabled = enabled;
    }

    public abstract void Reset();
}
