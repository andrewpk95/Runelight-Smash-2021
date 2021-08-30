using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUpdateComponent : Singleton<PhysicsUpdateComponent>
{
    void Start()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;
    }

    void FixedUpdate()
    {
        Physics2D.Simulate(Time.fixedDeltaTime);
    }
}
