using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    protected Rigidbody2D unitRigidbody;
    protected VelocityComponent velocityComponent;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    protected virtual void FixedUpdate()
    {
        Tick();
    }

    protected virtual void Tick() { }
}
