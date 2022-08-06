using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For any gameObject which is intended to behave as a Boid, it must utilize this class
public class Boid : MonoBehaviour
{
    Vector3 velocity;
    bool isPerching;


    // Some basic getters and setters for the boid
    public void SetVelocity(Vector3 newVelocity)
    {
        this.velocity = newVelocity;
    }

    public Vector3 GetVelocity()
    {
        return this.velocity;
    }

    public void SetPerching(bool isPerching)
    {
        this.isPerching = isPerching;
    }

    public bool GetPerching()
    {
        return this.isPerching;
    }
}
