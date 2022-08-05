using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
