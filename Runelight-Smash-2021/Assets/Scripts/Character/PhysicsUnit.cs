using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsUnit : BaseUnit
{
    // Required Components
    protected GravityComponent gravityComponent;
    protected SlopeComponent slopeComponent;

    protected override void Start()
    {
        base.Start();
        gravityComponent = GetComponent<GravityComponent>();
        slopeComponent = GetComponent<SlopeComponent>();
    }
}
