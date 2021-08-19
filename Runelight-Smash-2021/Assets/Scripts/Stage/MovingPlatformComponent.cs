using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VelocityComponent))]

public class MovingPlatformComponent : MonoBehaviour
{
    // Required Components
    private Rigidbody2D unitRigidbody;
    private VelocityComponent velocityComponent;

    // Platform Variables
    private HashSet<VelocityComponent> passengers = new HashSet<VelocityComponent>();

    // Start is called before the first frame update
    void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (VelocityComponent passenger in passengers)
        {
            passenger.AddVelocityModifier(velocityComponent.velocity);
        }
    }

    public void AddToPassengers(VelocityComponent passenger)
    {
        passengers.Add(passenger);
    }

    public void RemoveFromPassengers(VelocityComponent passenger)
    {
        passengers.Remove(passenger);
    }
}
